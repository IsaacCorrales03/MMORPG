using Shared.Tipos;
using MessagePack;

namespace Shared.Paquetes
{
    [MessagePackObject]
    public class PaqueteMovimiento: IPaquete
    {
        [Key(0)]
        public long Sequence {get; set;}
        [Key(1)]
        public Vector2 Input {get; set;}
        [Key(2)]
        public int Consecutive {get; set;}
        [Key(3)]
        public Vector2 ReportedPosition {get; set; }
        [Key(4)]
        public bool Moved {get; set;}
        [IgnoreMember]
        public TipoPaquete Tipo => TipoPaquete.Movimiento;
    }
}
