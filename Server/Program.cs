GameServer servidor = new();

servidor.Start();

Thread consola = new(() =>
{
    while (servidor.Running)
    {
        Console.Write("> ");
        string? comando = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(comando))
            continue;

        switch (comando.Trim().ToLowerInvariant())
        {
            case "stop":
                servidor.Stop();
                break;

            default:
                Console.WriteLine($"Comando desconocido: {comando}");
                break;
        }
    }
});

consola.IsBackground = true;
consola.Start();

while (servidor.Running)
{
    servidor.Update();
    Thread.Sleep(1);
}

consola.Join();