namespace Shared.Paquetes
{
    public class PaquetePeticionRegistro : IPaquete
    {
        public string Usuario { get; set; } = "";
        public string Clave {get; set; } = "";
        public string Email { get; set; } = "";
    }
}