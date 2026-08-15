using System.Diagnostics;

GameServer servidor = new();
servidor.Start();

Console.Clear();
Console.CursorVisible = false;

Thread consola = new(() => ServerConsole.IniciarLoopConsola(servidor));
consola.IsBackground = true;
consola.Start();

Stopwatch reloj = Stopwatch.StartNew();
double anterior = reloj.Elapsed.TotalSeconds;

while (servidor.Running)
{
    double actual = reloj.Elapsed.TotalSeconds;
    double delta = actual - anterior;
    anterior = actual;
    servidor.Update(delta);
    Thread.Sleep(1);
}

consola.Join();