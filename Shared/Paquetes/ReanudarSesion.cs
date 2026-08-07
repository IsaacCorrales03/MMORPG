using MessagePack;

namespace Shared.Paquetes
{
    [MessagePackObject]
    public class PaquetePeticionReanudarSesion : IPaquete
    { 
        [Key(0)]
        public string Token {get; set;}= "";
        [IgnoreMember]
        public TipoPaquete Tipo => TipoPaquete.PeticionReanudarSesion;
    }    
}
