namespace Server.Managers
{
    public static class ServerLog
    {
        public static event Action<string>? OnLog;
        public static void Log(string mensaje) => OnLog?.Invoke(mensaje);
    }
}