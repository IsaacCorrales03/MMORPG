using Tipos;

namespace Mundo
{
    public class Mapa
    {
        private int X = 10;
        private int Y = 10;
        private Tile Base_Tile = new Tile(true, '-');
        private Tile Player_Tile = new Tile(true, 'P');
        private Tile[,] Tiles;
        public Mapa(int y, int x)
        {
            Y = y;
            X = x;
            Tiles = new Tile[Y, X];
            renderizar();
        }
        public void cambiar_tile(Vector2 posicion, Tile tile)
        {
            Tiles[posicion.Y, posicion.X] = tile;
        }
        public void plantar_jugador(Vector2 posicion)
        {
            Tiles[posicion.Y, posicion.X] = Player_Tile;
        }
        public void redibujar(Vector2 posicion)
        {
            Tiles[posicion.Y, posicion.X] = Base_Tile;
        }
        public bool esPosicionValida(Vector2 posicion)
        {
            // 0, -1 es arriba, que se evalua? la posicion y no debe ser más que el eje y, entonces:
            // si Y = 20, el desplazamiento va de 0-19, comparamos:
            bool cumple_limite_vertical = posicion.Y < Y && posicion.Y >= 0;
            bool cumple_limite_horizontal = posicion.X < X && posicion.X >= 0;
            bool posicion_valida = cumple_limite_vertical && cumple_limite_horizontal;
            bool tile_valido;
            if (posicion_valida)
            {
                tile_valido = Tiles[posicion.Y, posicion.X].esTransitable;
            }
            else
            {
                tile_valido = false;
            }
            return posicion_valida && tile_valido;
        }
        public void renderizar()
        {
            for (int y = 0; y < Y; y++)
            {
                for (int x = 0; x < X; x++)
                {
                    Tiles[y, x] = Base_Tile;
                }
                
            }
        }

        public string dibujar()
        {
            var mapa = new System.Text.StringBuilder();
            for (int y = 0; y < Y; y++)
            {
                for (int x = 0; x < X; x++)
                {
                    mapa.Append(Tiles[y,x].Textura);
                   
                }
                mapa.Append('\n');
            }
            return mapa.ToString();
        }

    }
}