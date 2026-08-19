namespace Shared.Magia
{
    public static class CatalogoHechizos
    {

        public static readonly RunaElemental Fuego = new(
            new List<Tecla>
            {
                Tecla.W,
                Tecla.A,
                Tecla.S
            }
        );

        public static readonly CombinacionTeclas IS = new(
            new List<Tecla> { Tecla.A },
            new List<Tecla> { Tecla.A, Tecla.D },
            new List<Tecla> { Tecla.A, Tecla.D, Tecla.S }
        );

        public static readonly CombinacionTeclas IA =
            CombinacionTeclas.Concatenar(IS, Tecla.W);

        public static readonly CombinacionTeclas ARA =
            CombinacionTeclas.Concatenar(IA, Tecla.R);

        public static readonly CombinacionTeclas ARAION =
            CombinacionTeclas.Concatenar(ARA, Tecla.Q);

        public static readonly CombinacionTeclas AERONIS =
            CombinacionTeclas.Concatenar(ARAION, Tecla.S);

        public static readonly CombinacionTeclas AERAVON =
            CombinacionTeclas.Concatenar(AERONIS, Tecla.D);

        public static readonly Hechizo Ignis = new("Ignis", 1, 10, new List<Elemento> { Elemento.Fuego}, Fuego,  IS);
        public static readonly Hechizo Ignia = new("Ignia", 2, 20, new List<Elemento> {Elemento.Fuego}, Fuego, IA);
        public static readonly Hechizo Ignara = new("Ignara", 3, 30, new List<Elemento> {Elemento.Fuego}, Fuego, ARA);
        
    }
}