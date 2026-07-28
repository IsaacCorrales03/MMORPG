namespace Server.Red.Sesiones
{
    public class TokenGenerator
    {
        public static string Generar()
        {
            byte[] bytes = System.Security.Cryptography.RandomNumberGenerator.GetBytes(32);
            return Convert.ToBase64String(bytes);
        }
    }
}