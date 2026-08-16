using MessagePack;
using Shared.Tipos;

namespace Shared.Paquetes
{
    [MessagePackObject]
    public class PaqueteRespuestaAparecerJugador: IPaquete
    {
        [Key(0)]
        public Vector2 Posicion {get; set;}
        [Key(1)]
        public bool Exitoso {get; set;}
        [Key(2)]
        public string MensajeDeError {get; set;} = "";
        [IgnoreMember]
        public TipoPaquete Tipo  => TipoPaquete.RespuestaAparecerJugador;

    }
}