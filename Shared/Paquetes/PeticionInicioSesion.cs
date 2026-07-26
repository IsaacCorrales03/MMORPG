namespace Shared.Paquetes
{
    public class PaquetePeticionInicioSesion : IPaquete
    {
        public string Usuario { get; set; } = "";
        public string Clave {get; set; } = "";

    }
}