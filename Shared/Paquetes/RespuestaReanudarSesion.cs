namespace Shared.Paquetes
{
    public class PaqueteRespuestaReanudarSesion : IPaquete
    { 
        public bool Exitoso {get; set;}
        public string? Token {get; set;}= "";
        public string? NombreUsuario { get; set; }
        public int? IdUsuario { get; set; }

        public string? MensajeError { get; set;}
    }    
}
