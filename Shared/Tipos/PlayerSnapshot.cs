using MessagePack;
using Shared.Tipos;

namespace Shared.Paquetes
{
    [MessagePackObject]
    public class PlayerSnapshot
    {
        [Key(0)]
        public int PlayerId { get; set; }

        [Key(1)]
        public Vector2 Position { get; set; }

        [Key(2)]
        public string Nombre { get; set; } = "";

        [Key(3)]
        public Vector2 Direction { get; set; }

        [Key(4)]
        public bool Moving { get; set; }
    }
}