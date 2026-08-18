using Server.Mundo;
using Server.Red;

public class GameServer
{
    public readonly World _mundo;
    private readonly NetworkServer _servidorRed;

    /// <summary>Expone el servidor de red para consumo del dashboard / consola.</summary>
    public NetworkServer Red => _servidorRed;

    public bool Running { get; private set; }

    private const double TickRate = 1.0 / 20.0;
    private double _tickAccumulator;

    public GameServer()
    {
        _mundo = new World();
        _servidorRed = new NetworkServer(10, 8455, _mundo);
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

        while (_tickAccumulator >= TickRate)
        {
            _mundo.Tick();

            _tickAccumulator -= TickRate;
            
        }
    }
}