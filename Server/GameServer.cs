using LiteNetLib;
using Server.Managers;
using Server.Mundo;
using Server.Red;
using Shared.Magia;

public class GameServer
{
    public record PlayerMonitorInfo(
        int PlayerId,
        string Nombre,
        float PosX,
        float PosY,
        bool Moving,
        int PingMs
    );

    public IEnumerable<PlayerMonitorInfo> GetPlayerSnapshotForMonitor()
    {
        foreach (var peer in _servidorRed.Peers)
        {
            int? id = SesionManager.ObtenerIdPorPeer(peer);
            if (id is null) continue;

            var player = _mundo.GetPlayer(id.Value);
            if (player is null) continue;

            yield return new PlayerMonitorInfo(
                player.PlayerId,
                player.Nombre,
                player.Position.X,
                player.Position.Y,
                player.IsMoving,
                peer.Ping
            );
        }
    }
    public readonly World _mundo;
    private readonly NetworkServer _servidorRed;

    /// <summary>Expone el servidor de red para consumo del dashboard / consola.</summary>
    public NetworkServer Red => _servidorRed;

    public bool Running { get; private set; }
    private int _ticksThisSecond = 0;
    private double _tpsTimer = 0;
    public int CurrentTps { get; private set; }
    private const double TickRate = 1.0 / 20.0;
    private double _tickAccumulator;

    public GameServer()
    {
        _mundo = new World();
        _servidorRed = new NetworkServer(10, 8455, _mundo);
        _servidorRed.JugadorDesconectado += OnJugadorDesconectado;
    }
    public void OnJugadorDesconectado(NetPeer peer)
    {
        int? id = SesionManager.ObtenerIdPorPeer(peer);
        if (id.HasValue)
        {
            _mundo.RemovePlayer(id.Value);
        }
    }

    public void Start()
    {

        if (Running)
            return;

        _servidorRed.Start();
        Running = true;
    }

    public void Stop()
    {
        if (!Running)
            return;

        Running = false;
        _servidorRed.Stop();
    }

    public void Update(double delta)
    {
        if (!Running)
            return;

        _servidorRed.PollEvents();
        _tickAccumulator += delta;
        _tpsTimer += delta;

        while (_tickAccumulator >= TickRate)
        {
            _mundo.Tick();
            _ticksThisSecond++;
            _tickAccumulator -= TickRate;
        }

        if (_tpsTimer >= 1.0)
        {
            CurrentTps = _ticksThisSecond;
            _ticksThisSecond = 0;
            _tpsTimer -= 1.0;
        }
    }
}