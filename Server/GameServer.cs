using Server.Red;

public class GameServer
{
    private readonly NetworkServer _servidorRed = new(10, 8455);

    public bool Running { get; private set; }

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

    public void Update()
    {
        if (!Running)
            return;

        _servidorRed.PollEvents();

        // World.Update();
        // Systems.Update();
    }
}