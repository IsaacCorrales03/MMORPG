using Shared.Tipos;

namespace Server.Mundo
{
    public class World
    {
        private readonly Dictionary<int, PlayerState> players = new();
        
        public void AddPlayer(int playerId, Vector2 position)
            {
                players[playerId] = new PlayerState(
                playerId,
                position
            );
        }

        public void RemovePlayer(int playerId)
        {
            players.Remove(playerId);
        }

        public PlayerState? GetPlayer(int playerId)
        {
            players.TryGetValue(playerId, out var player);
            return player;
        }
    }
}