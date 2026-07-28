namespace Shared.Paquetes
{
    public class PaqueteRespuestaRegistro : IPaquete
    {
        public bool Exitoso {get; set;} = false;
        public string MensajeError {get; set;} = "";
        public string Token {get; set;} = "";
    }
}