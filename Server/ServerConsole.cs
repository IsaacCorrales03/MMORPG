using Server.Mundo;
using Shared.Tipos;
using System.Text;

public static class ServerConsole
{
    private static readonly object ConsoleLock = new();
    private static readonly StringBuilder inputBuffer = new();
    private static int statusLines = 0; // cuántas líneas ocupa el dashboard

    public static void IniciarLoopConsola(GameServer servidor)
    {
        // Hilo de refresco del dashboard
        Thread render = new(() =>
        {
            while (servidor.Running)
            {
                Redibujar(servidor);
                Thread.Sleep(500); // refresco cada 0.5s, ajustalo a gusto
            }
        });
        render.IsBackground = true;
        render.Start();

        // Loop de input manual (reemplaza a Console.ReadLine)
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
                    // Limpiar la línea de prompt antes de procesar
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

    private static void Redibujar(GameServer servidor)
    {
        lock (ConsoleLock)
        {
            Console.SetCursorPosition(0, 0);
            Console.CursorVisible = false;

            var sb = new StringBuilder();
            sb.AppendLine("====================================");
            sb.AppendLine("       ASTERA SERVER STATUS         ");
            sb.AppendLine("====================================");
            sb.AppendLine();
            sb.AppendLine($"Estado              : {(servidor.Running ? "RUNNING" : "STOPPED")}");
            sb.AppendLine($"Jugadores conectados: {servidor._mundo.players.Count}");
            sb.AppendLine();
            sb.AppendLine("PLAYERS");
            sb.AppendLine("------------------------------------");

            if (servidor._mundo.players.Count == 0)
            {
                sb.AppendLine("  Ningún jugador conectado.               ");
            }
            else
            {
                foreach (var entry in servidor._mundo.players)
                {
                    int id = entry.Key;
                    PlayerState player = entry.Value;
                    sb.AppendLine(
                        $"  ID: {id,-5} " +
                        $"Pos: ({player.Position.X,8:F2}, {player.Position.Y,8:F2}) " +
                        $"Speed: {player.MoveSpeed,6:F1}   "
                    );
                }
            }

            sb.AppendLine();
            sb.AppendLine("------------------------------------");
            sb.AppendLine("COMMANDS: stop | players | clear | help");
            sb.AppendLine("------------------------------------");

            string texto = sb.ToString();
            var lineas = texto.Split('\n');

            foreach (var linea in lineas)
            {
                // pad para pisar restos de texto viejo más largo
                Console.WriteLine(linea.PadRight(Console.WindowWidth - 1));
            }

            statusLines = lineas.Length;

            RedibujarPrompt();
        }
    }

    private static void RedibujarPrompt()
    {
        int promptRow = statusLines;
        Console.SetCursorPosition(0, promptRow);
        string linea = "> " + inputBuffer;
        Console.Write(linea.PadRight(Console.WindowWidth - 1));
        Console.SetCursorPosition(2 + inputBuffer.Length, promptRow);
        Console.CursorVisible = true;
    }

    public static void EjecutarComando(string comando, GameServer servidor)
    {
        switch (comando.Trim().ToLowerInvariant())
        {
            case "stop":
                servidor.Stop();
                break;
            case "players":
                MostrarJugadoresDebajo(servidor);
                break;
            case "clear":
                Console.Clear();
                break;
            case "help":
                break;
            case "":
                break;
            default:
                break;
        }
    }

    private static void MostrarJugadoresDebajo(GameServer servidor)
    {
        // Con el modelo de "un solo dashboard", esto ya se ve arriba siempre.
        // Si querés un log aparte de eventos, lo mejor es un panel de "log"
        // como tercera sección (ver nota abajo).
    }
}