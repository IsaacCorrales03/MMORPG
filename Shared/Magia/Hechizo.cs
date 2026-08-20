namespace Shared.Magia
{
    public class Hechizo
    {
        public string Nombre { get; protected set; } = "";
        public int Nivel { get; protected set; }
        public int CostoMana { get; protected set; }
        public string Descripcion { get; protected set; } = "";
        public List<Elemento> Elementos { get; protected set; }

        public RunaElemental Runa { get; protected set; }
        public CombinacionTeclas Combinacion { get; protected set; }
        public float TiempoMinimo { get; protected set; }
        public float TiempoMaximo { get; protected set; }
        public Hechizo(string nombre, int nivel, int costoMana, string descripcion, List<Elemento> elementos, RunaElemental runa, CombinacionTeclas combinacion, float tiempoMinimo, float tiempoMaximo)
        {
            Nombre = nombre;
            Nivel = nivel;
            CostoMana = costoMana;
            Descripcion = descripcion;
            Elementos = elementos;
            Runa = runa;
            Combinacion = combinacion;
            TiempoMinimo = tiempoMinimo;
            TiempoMaximo = tiempoMaximo;
        }

        public List<Tecla> ObtenerCombinacion(int nivelUsuario)
        {
            if (Nivel > nivelUsuario + 1)
            {
                throw new InvalidOperationException(
                    $"El usuario de nivel {nivelUsuario} no puede utilizar el hechizo {Nombre} de nivel {Nivel}."
                );
            }

            if (Nivel == nivelUsuario + 1)
            {
                return Combinacion.Compleja;
            }

            if (Nivel == nivelUsuario)
            {
                return Combinacion.Normal;
            }

            return Combinacion.Simple;
        }
        public List<Tecla> ObtenerSecuencia(int nivelUsuario)
        {
            return Runa.Teclas.Concat(ObtenerCombinacion(nivelUsuario)).ToList();
        }
    }
}