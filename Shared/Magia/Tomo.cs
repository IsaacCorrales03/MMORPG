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

        public Tomo(string nombre, Elemento afinidadElemental, Elemento incompatibilidadElemental, int nivel)
        {
            Nombre = nombre;
            AfinidadElemental = afinidadElemental;
            IncompatibilidadElemental = incompatibilidadElemental;
            Hechizos = new List<Hechizo>();
            Nivel = nivel;
        }

        public void AsignarHechizo(Hechizo hechizo)
        {
            if (Hechizos.Count >= EspacioMaximo)
                throw new InvalidOperationException("El tomo está lleno.");

            if (hechizo.Elementos.Contains(IncompatibilidadElemental))
            {
                throw new InvalidOperationException(
                    $"El hechizo {hechizo.Nombre} es incompatible con este tomo."
                );
            }

            Hechizos.Add(hechizo);
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
        public Tomo Clonar()
        {
            var copia = new Tomo(Nombre, AfinidadElemental, IncompatibilidadElemental, Nivel);
            copia.Hechizos.AddRange(Hechizos);
            return copia;
        }

        // Método de apoyo, ya que Nivel es protected set y el constructor no lo recibe
        protected void SetNivel(int nivel)
        {
            Nivel = nivel;
        }
    }
}