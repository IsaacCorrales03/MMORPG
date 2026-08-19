namespace Shared.Magia
{
    public class Hechizo
    {
        public string Nombre { get; protected set; } = "";
        public int Nivel { get; protected set; }
        public int CostoMana { get; protected set; }

        public List<Elemento> Elementos { get; protected set; }

        public CombinacionTeclas Combinacion { get; protected set; }

        public Hechizo(string nombre, int nivel, int costoMana, List<Elemento> elementos, CombinacionTeclas combinacion)
        {
            Nombre = nombre;
            Nivel = nivel;
            CostoMana = costoMana;
            Elementos = elementos;
            Combinacion = combinacion;
        }
    }
}