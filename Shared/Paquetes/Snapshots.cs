using MessagePack;
using Shared.Tipos;

namespace Shared.Paquetes
{
    [MessagePackObject]
    public class PaqueteSnapshots : IPaquete
    {
        [Key(0)]
        public long Tick { get; set; }

        [Key(1)]
        public List<PlayerSnapshot> Players { get; set; } = new();
        [Key(2)]
        public string Nombre { get; set; } = "";
        [IgnoreMember]
        public TipoPaquete Tipo => TipoPaquete.Snapshot;
    }
}