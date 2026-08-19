namespace Shared.Magia
{
    public class Tomo
    {
        public string Nombre { get; protected set; } = "";
        public int Nivel { get; protected set; } = 1;

        public int EspacioMaximo => Nivel switch
        {
            1 => 4,
            2 => 6,
            3 => 8,
            _ => 0
        };

        public Elemento AfinidadElemental { get; protected set; }

        public Elemento IncompatibilidadElemental { get; protected set; }

        public List<Hechizo> Hechizos { get; protected set; }

        public Tomo( string nombre, Elemento afinidadElemental, Elemento incompatibilidadElemental)
        {
            Nombre = nombre;
            AfinidadElemental = afinidadElemental;
            IncompatibilidadElemental = incompatibilidadElemental;
            Hechizos = new List<Hechizo>();
        }

        public void AsignarHechizo(Hechizo hechizo)
        {
        }

        public void RemoverHechizo(Hechizo hechizo)
        {
        }

        public void ReemplazarHechizo(int indice, Hechizo hechizo)
        {
        }

        public void SubirNivel()
        {
        }
    }
}