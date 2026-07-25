namespace Tipos
{
    public struct Tile
    {
        public char Textura;
        public bool esTransitable;

        public Tile(bool transitable, char textura)
        {
            esTransitable = transitable;
            Textura = textura;
        }
        public Tile()
        {
            esTransitable = true;
            Textura = '.';
        }
    }
}