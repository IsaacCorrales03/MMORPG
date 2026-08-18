using LiteNetLib;
using Server.Mundo;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

public static class ServerConsole
{
    private static readonly object ConsoleLock = new();
    private static readonly StringBuilder inputBuffer = new();

    // Número FIJO de líneas que ocupa el dashboard (debe coincidir siempre
    // con lo que imprime Redibujar, o el prompt se desalinea).
    private const int DashboardLines = 12;
    private static int statusLines = DashboardLines;

    // --- Métricas ---
    private static readonly Process proc = Process.GetCurrentProcess();
    private static DateTime lastNetCheck = DateTime.UtcNow;

    private static long lastBytesIn = 0;
    private static long lastBytesOut = 0;

    private static double kbpsIn = 0;
    private static double kbpsOut = 0;

    private static readonly Stopwatch uptime = Stopwatch.StartNew();


    // =========================================================
    // ANSI (multiplataforma: Windows / Linux / macOS)
    // =========================================================

    // En Windows, la consola necesita habilitar explícitamente el modo
    // ENABLE_VIRTUAL_TERMINAL_PROCESSING para interpretar secuencias ANSI.
    // En Linux/macOS la terminal ya las soporta de forma nativa, así que
    // esto simplemente no se ejecuta ahí.
    private static void HabilitarAnsiSiWindows()
    {
        if (!OperatingSystem.IsWindows())
            return;

        try
        {
            var handle = GetStdHandle(STD_OUTPUT_HANDLE);

            if (!GetConsoleMode(handle, out uint modo))
                return;

            modo |= ENABLE_VIRTUAL_TERMINAL_PROCESSING;

            SetConsoleMode(handle, modo);
        }
        catch
        {
            // Si falla (terminales muy viejas de Windows), seguimos igual;
            // en el peor caso se ven los códigos de escape como texto.
        }
    }

    private const int STD_OUTPUT_HANDLE = -11;
    private const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;

    [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GetStdHandle(int nStdHandle);

    [System.Runtime.InteropServices.DllImport("kernel32.dll")]
    private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

    [System.Runtime.InteropServices.DllImport("kernel32.dll")]
    private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

    private static void AnsiClearScreen()
    {
        // Limpia toda la pantalla y mueve el cursor a (0,0).
        Console.Write("\x1b[2J\x1b[H");
    }

    private static void AnsiSetCursor(int col, int row)
    {
        // ANSI usa coordenadas basadas en 1, no en 0.
        Console.Write($"\x1b[{row + 1};{col + 1}H");
    }

    private static void AnsiOcultarCursor()
    {
        Console.Write("\x1b[?25l");
    }

    private static void AnsiMostrarCursor()
    {
        Console.Write("\x1b[?25h");
    }

    // Ancho de consola de forma segura: Console.WindowWidth puede lanzar
    // excepción si la salida no es una consola real (ej. redirigida a
    // archivo o pipe en Linux).
    private static int AnchoConsola()
    {
        try
        {
            int w = Console.WindowWidth;
            return w > 0 ? w : 80;
        }
        catch
        {
            return 80;
        }
    }


    // =========================================================
    // INICIO
    // =========================================================

    public static void IniciarLoopConsola(GameServer servidor)
    {
        // Usamos únicamente secuencias ANSI para limpiar/posicionar el
        // cursor (funciona igual en Windows Terminal, cmd.exe moderno,
        // PowerShell, Linux y macOS). Evitamos SetWindowSize/SetBufferSize
        // porque esas APIs son exclusivas de Windows y lanzan
        // PlatformNotSupportedException en Linux/macOS.
        HabilitarAnsiSiWindows();

        AnsiClearScreen();
        AnsiOcultarCursor();

        Thread render = new(() =>
        {
            while (servidor.Running)
            {
                ActualizarRed(servidor);
                Redibujar(servidor);

                Thread.Sleep(500);
            }
        });

        render.IsBackground = true;
        render.Start();


        // Input
        while (servidor.Running)
        {
            if (!Console.KeyAvailable)
            {
                Thread.Sleep(15);
                continue;
            }

            ConsoleKeyInfo key = Console.ReadKey(intercept: true);

            lock (ConsoleLock)
            {
                if (key.Key == ConsoleKey.Enter)
                {
                    string comando = inputBuffer.ToString();

                    inputBuffer.Clear();

                    EjecutarComando(comando, servidor);
                }
                else if (key.Key == ConsoleKey.Backspace)
                {
                    if (inputBuffer.Length > 0)
                        inputBuffer.Remove(inputBuffer.Length - 1, 1);
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    inputBuffer.Append(key.KeyChar);
                }

                RedibujarPrompt();
            }
        }
    }


    // =========================================================
    // RED
    // =========================================================

    private static void ActualizarRed(GameServer servidor)
    {
        var stats = servidor.Red.Estadisticas;

        if (stats == null)
            return;

        DateTime now = DateTime.UtcNow;

        double elapsedSec =
            (now - lastNetCheck).TotalSeconds;

        if (elapsedSec <= 0)
            return;

        long bytesIn = (long)stats.BytesReceived;
        long bytesOut = (long)stats.BytesSent;

        kbpsIn =
            (bytesIn - lastBytesIn)
            / 1024.0
            / elapsedSec;

        kbpsOut =
            (bytesOut - lastBytesOut)
            / 1024.0
            / elapsedSec;

        lastBytesIn = bytesIn;
        lastBytesOut = bytesOut;

        lastNetCheck = now;
    }


    // =========================================================
    // UTILIDADES
    // =========================================================

    private static string ObtenerIpLocal()
    {
        try
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    return ip.ToString();
            }
        }
        catch
        {
        }

        return "0.0.0.0";
    }


    private static string FormatBytes(long bytes)
    {
        double mb = bytes / (1024.0 * 1024.0);

        if (mb >= 1024)
            return $"{mb / 1024.0:F2} GB";

        return $"{mb:F1} MB";
    }


    private static string FormatKbps(double kbps)
    {
        if (kbps >= 1024)
            return $"{kbps / 1024.0:F2} MB/s";

        return $"{kbps:F1} KB/s";
    }


    private static string FormatUptime(TimeSpan t)
    {
        if (t.TotalDays >= 1)
        {
            return $"{(int)t.TotalDays}d " +
                   $"{t.Hours:D2}:" +
                   $"{t.Minutes:D2}:" +
                   $"{t.Seconds:D2}";
        }

        return $"{t.Hours:D2}:" +
               $"{t.Minutes:D2}:" +
               $"{t.Seconds:D2}";
    }


    // Escribe una línea completa, rellenando con espacios hasta el ancho de
    // la consola para borrar cualquier resto de texto anterior en esa fila.
    private static void EscribirLinea(string texto)
    {
        int width = Math.Max(0, AnchoConsola() - 1);

        if (texto.Length > width)
            texto = texto.Substring(0, width);

        Console.Write(texto.PadRight(width));
        Console.Write("\n");
    }


    // =========================================================
    // DASHBOARD
    // =========================================================

    private static void Redibujar(GameServer servidor)
    {
        lock (ConsoleLock)
        {
            proc.Refresh();

            var stats = servidor.Red.Estadisticas;
            bool statsDisponibles = stats != null;

            AnsiSetCursor(0, 0);

            var lineas = new List<string>
            {
                "================================",
                "        ASTERA SERVER",
                "================================",
                "",
                $"IP          : {ObtenerIpLocal()}:{servidor.Red.Puerto}",
                $"Uptime      : {FormatUptime(uptime.Elapsed)}",
                $"Jugadores   : {servidor.Red.JugadoresConectados} / {servidor.Red.MaximoJugadores}",
                "",
                statsDisponibles
                    ? $"Bajada      : {FormatKbps(kbpsIn),10}"
                    : "Bajada      : N/A",
                statsDisponibles
                    ? $"Subida      : {FormatKbps(kbpsOut),10}"
                    : "Subida      : N/A",
                $"RAM usada   : {FormatBytes(proc.WorkingSet64),10}",
                "",
            };

            // Si por alguna razón el número de líneas cambiara, ajustamos
            // DashboardLines dinámicamente para que el prompt no se desalinee.
            statusLines = lineas.Count;

            foreach (var linea in lineas)
                EscribirLinea(linea);

            RedibujarPrompt();
        }
    }


    // =========================================================
    // PROMPT
    // =========================================================

    private static void RedibujarPrompt()
    {
        int promptRow = statusLines;

        AnsiSetCursor(0, promptRow);

        string linea = "> " + inputBuffer;

        Console.Write(
            linea.PadRight(
                Math.Max(0, AnchoConsola() - 1)
            )
        );

        AnsiSetCursor(2 + inputBuffer.Length, promptRow);

        AnsiMostrarCursor();
    }


    // =========================================================
    // COMANDOS
    // =========================================================

    public static void EjecutarComando(
        string comandoCrudo,
        GameServer servidor)
    {
        string comando = comandoCrudo.Trim();

        string[] partes = comando.Split(
            ' ',
            2,
            StringSplitOptions.RemoveEmptyEntries
        );

        string cmd =
            partes.Length > 0
                ? partes[0].ToLowerInvariant()
                : "";

        string args =
            partes.Length > 1
                ? partes[1]
                : "";


        switch (cmd)
        {
            case "stop":

                servidor.Stop();

                break;


            case "paquetes":

                MostrarPaquetes(servidor);

                break;


            case "help":

                MostrarHelp();

                break;


            case "clear":

                AnsiClearScreen();

                break;


            case "":

                break;


            default:

                break;
        }
    }


    // =========================================================
    // PAQUETES
    // =========================================================
    public static void Log(string mensaje)
    {
        // Los logs ya no se muestran en el dashboard.
    }
    private static void MostrarPaquetes(GameServer servidor)
    {
        lock (ConsoleLock)
        {
            AnsiClearScreen();
            AnsiOcultarCursor();

            var stats = servidor.Red.Estadisticas;

            if (stats == null)
            {
                Console.WriteLine(
                    "Estadísticas de red no disponibles."
                );
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("PAQUETES");
                Console.WriteLine("--------------------------------");

                Console.WriteLine(
                    $"Recibidos : {stats.PacketsReceived}"
                );

                Console.WriteLine(
                    $"Enviados  : {stats.PacketsSent}"
                );

                Console.WriteLine(
                    $"Perdidos  : {stats.PacketLoss}"
                );
            }

            Console.WriteLine();

            Console.WriteLine(
                "Presiona ENTER para continuar..."
            );

            Console.ReadLine();

            AnsiClearScreen();
        }
    }


    // =========================================================
    // HELP
    // =========================================================

    private static void MostrarHelp()
    {
        lock (ConsoleLock)
        {
            AnsiClearScreen();
            AnsiOcultarCursor();

            Console.WriteLine();
            Console.WriteLine("COMANDOS");
            Console.WriteLine("--------------------------------");
            Console.WriteLine("stop       Detiene el servidor");
            Console.WriteLine("paquetes   Estadísticas de paquetes");
            Console.WriteLine("clear      Limpia la consola");
            Console.WriteLine("help       Muestra esta ayuda");
            Console.WriteLine();
            Console.WriteLine(
                "Presiona ENTER para continuar..."
            );

            Console.ReadLine();

            AnsiClearScreen();
        }
    }
}