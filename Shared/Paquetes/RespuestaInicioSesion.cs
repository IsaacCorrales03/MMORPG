namespace Shared.Paquetes
{
    public class PaqueteRespuestaInicioSesion : IPaquete
    {
        public bool Exitoso {get; set;} = false;
        public string MensajeError {get; set;} = "";
        public int IDJugador { get; set; } = -1;
    }
}