using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;

var listener = new TcpListener(IPAddress.Any, 7000);
listener.Start();

Console.WriteLine("Servidor iniciado en el puerto 7000");

// Diccionario thread-safe: TcpClient -> info del cliente
var clientes = new ConcurrentDictionary<TcpClient, ClienteInfo>();

while (true)
{
    TcpClient cliente = await listener.AcceptTcpClientAsync();
    _ = ManejarClienteAsync(cliente);
}

async Task ManejarClienteAsync(TcpClient cliente)
{
    NetworkStream stream = cliente.GetStream();
    var reader = new StreamReader(stream, Encoding.UTF8);
    var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

    // El primer mensaje que manda el cliente es su nombre.
    // Se guarda aparte, NO se reenvía como si fuera un chat.
    string? nombre = await reader.ReadLineAsync();
    if (string.IsNullOrWhiteSpace(nombre))
    {
        cliente.Close();
        return;
    }

    var info = new ClienteInfo(nombre, writer, new SemaphoreSlim(1, 1));
    clientes[cliente] = info;

    Console.WriteLine($"{nombre} se ha conectado.");
    await BroadcastAsync($"* {nombre} se ha unido al chat *", cliente);

    try
    {
        while (true)
        {
            string? mensaje = await reader.ReadLineAsync();
            if (mensaje == null)
                break; // el cliente cerró la conexión

            if (string.IsNullOrWhiteSpace(mensaje))
                continue;

            Console.WriteLine($"{nombre}: {mensaje}");
            await BroadcastAsync($"{nombre}: {mensaje}", null);
        }
    }
    catch (IOException)
    {
        // Desconexión abrupta, se maneja abajo igual
    }
    finally
    {
        clientes.TryRemove(cliente, out _);
        cliente.Close();
        Console.WriteLine($"{nombre} se ha desconectado.");
        await BroadcastAsync($"* {nombre} se ha desconectado *", null);
    }
}

async Task BroadcastAsync(string mensaje, TcpClient? excluir)
{
    foreach (var (cli, info) in clientes)
    {
        if (cli == excluir || !cli.Connected)
            continue;

        // Un semáforo por cliente evita que dos hilos escriban
        // al mismo tiempo en el mismo stream (eso causaba los
        // mensajes superpuestos/mezclados).
        await info.EscrituraLock.WaitAsync();
        try
        {
            await info.Writer.WriteLineAsync(mensaje);
        }
        catch (IOException)
        {
            // Se detectará y limpiará cuando ese cliente falle al leer
        }
        finally
        {
            info.EscrituraLock.Release();
        }
    }
}

record ClienteInfo(string Nombre, StreamWriter Writer, SemaphoreSlim EscrituraLock);