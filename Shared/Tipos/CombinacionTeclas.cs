namespace Shared.Magia
{
    public class CombinacionTeclas
    {
        public List<Tecla> Simple { get; protected set; }
        public List<Tecla> Normal { get; protected set; }
        public List<Tecla> Compleja { get; protected set; }

        public CombinacionTeclas(List<Tecla> simple, List<Tecla> normal, List<Tecla> compleja)
        {
            Simple = simple;
            Normal = normal;
            Compleja = compleja;
        }

        public static CombinacionTeclas Concatenar(CombinacionTeclas anterior, Tecla nuevaTecla)
        {
            var simple = new List<Tecla>(anterior.Normal);

            var normal = new List<Tecla>(anterior.Compleja);

            var compleja = new List<Tecla>(anterior.Compleja)
            {
                nuevaTecla
            };

            return new CombinacionTeclas(
                simple,
                normal,
                compleja
            );
        }
    }
}