using Shared.Tipos;

namespace Shared.Paquetes
{
    public class PaqueteAparecerJugador : IPaquete
    {
        public int IDJugador {get; set;}
        public string NombreUsuario {get; set;} = "";
        
        public Vector2 Posicion {get; set;}
        public TipoPaquete Tipo {get;} = TipoPaquete.AparecerJugador;
    }
}