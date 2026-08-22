using Server.Managers;
using Server.Host;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();
builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();
app.UseDefaultFiles();
app.UseStaticFiles(); // sirve wwwroot/index.html
app.UseCors();
app.MapHub<MonitorHub>("/monitorHub");

var gameServer = new GameServer();
var hubContext = app.Services.GetRequiredService<IHubContext<MonitorHub>>();

// Reenviar logs del core al hub
ServerLog.OnLog += msg => hubContext.Clients.All.SendAsync("Log", msg);

gameServer.Start();

// Loop de simulación en background
_ = Task.Run(async () =>
{
    var sw = System.Diagnostics.Stopwatch.StartNew();
    double last = 0;
    while (true)
    {
        double now = sw.Elapsed.TotalSeconds;
        double delta = now - last;
        last = now;

        gameServer.Update(delta);
        await Task.Delay(5);
    }
});

// Push periódico de stats (cada 1s)
_ = Task.Run(async () =>
{
    long lastSent = 0, lastReceived = 0;

    while (true)
    {
        await Task.Delay(1000);

        long sentNow = gameServer.Red.Estadisticas?.BytesSent ?? 0;
        long receivedNow = gameServer.Red.Estadisticas?.BytesReceived ?? 0;
        long sentPerSec = sentNow - lastSent;
        long receivedPerSec = receivedNow - lastReceived;
        lastSent = sentNow;
        lastReceived = receivedNow;

        var stats = new
        {
            jugadores = gameServer.Red.JugadoresConectados,
            tick = gameServer._mundo.CurrentTick,
            tps = gameServer.CurrentTps,
            kbpsEnviado = sentPerSec / 1024.0,
            kbpsRecibido = receivedPerSec / 1024.0,
            running = gameServer.Running,
            players = gameServer.GetPlayerSnapshotForMonitor()
        };

        await hubContext.Clients.All.SendAsync("Stats", stats);
    }
});

app.Run("http://localhost:5080");