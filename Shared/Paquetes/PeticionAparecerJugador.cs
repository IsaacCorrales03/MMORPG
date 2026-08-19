using MessagePack;
using Shared.Tipos;

namespace Shared.Paquetes
{
    [MessagePackObject]
    public class PaquetePeticionAparecerJugador : IPaquete
    {
        [Key(0)]
        public int JugadorId {get; set;}
        [Key(1)]
        public Vector2 Position {get; set;}

        [IgnoreMember]
        public TipoPaquete Tipo => TipoPaquete.PeticionAparecerJugador;
        

    }
}