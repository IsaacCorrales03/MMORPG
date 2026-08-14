namespace Shared.Tipos
{
    public class PlayerState
    {
        public int PlayerId;
        public Vector2 Position;

        public PlayerState(int playerId, Vector2 position)
        {
            PlayerId = playerId;
            Position = position;
        }

    }
}