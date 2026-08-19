using Shared.Clases;

namespace Shared.Tipos
{
    public class PlayerState
    {
        public int PlayerId;
        public Vector2 Position;
        public float MoveSpeed;
        public ChunkPosition chunkPosition;
        public Vector2 LastDirection { get; set; } = new Vector2();
        public bool IsMoving { get; set; }
        public string Nombre;
        public long LastSequenceProcessed { get; set; }
        public Clase Clase {get; set;}
        public PlayerState(int playerId, Vector2 position, string nombre, Clase clase)
        {
            PlayerId = playerId;
            Position = position;
            MoveSpeed = 200f;
            chunkPosition = ChunkPosition.FromPosition(position);
            Nombre = nombre;
            Clase = clase;
        }

    }
}