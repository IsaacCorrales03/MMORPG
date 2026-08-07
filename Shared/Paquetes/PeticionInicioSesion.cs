using MessagePack;

namespace Shared.Paquetes
{
    [MessagePackObject]
    public class PaquetePeticionInicioSesion : IPaquete
    {
        [Key(0)]
        public string Usuario { get; set; } = "";
        [Key(1)]
        public string Clave {get; set; } = "";
        [IgnoreMember]
        public TipoPaquete Tipo => TipoPaquete.PeticionInicioSesion;

    }
}