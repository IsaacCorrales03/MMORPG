using MessagePack;
using Shared.Tipos;

namespace Shared.Paquetes
{
    [MessagePackObject]
    public class PaqueteCorrecionMovimiento: IPaquete
    {
        [Key(0)]
        public Vector2 Posicion {get; set;}
        [Key(1)]
        public long LastSequenceProcessed { get; set; }
        [Key(2)]
        public int PlayerId {get; set;}
        [IgnoreMember]
        public TipoPaquete Tipo => TipoPaquete.CorrecionMovimiento;
    }
}