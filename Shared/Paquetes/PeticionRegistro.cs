using MessagePack;

namespace Shared.Paquetes
{
    [MessagePackObject]
    public class PaquetePeticionRegistro : IPaquete
    {
        [Key(0)]
        public string Usuario { get; set; } = "";
        [Key(1)]
        public string Clave {get; set; } = "";
        [Key(2)]
        public string Email { get; set; } = "";
        [IgnoreMember]
        public TipoPaquete Tipo => TipoPaquete.PeticionRegistro;
    }
}