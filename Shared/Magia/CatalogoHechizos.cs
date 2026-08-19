namespace Shared.Magia
{
    public static class CombinacionesRunas
    {
        public static readonly CombinacionTeclas IS = new(
            new List<Tecla>
            {
                Tecla.A
            },
            new List<Tecla>
            {
                Tecla.A,
                Tecla.D
            },
            new List<Tecla>
            {
                Tecla.A,
                Tecla.D,
                Tecla.S
            }
        );

        public static readonly CombinacionTeclas IA = new(
            new List<Tecla>
            {
                Tecla.A,
                Tecla.R
            },
            new List<Tecla>
            {
                Tecla.A,
                Tecla.R,
                Tecla.Q
            },
            new List<Tecla>
            {
                Tecla.A,
                Tecla.R,
                Tecla.Q,
                Tecla.W
            }
        );

        public static readonly CombinacionTeclas ARA = new(
            new List<Tecla>
            {
                Tecla.A,
                Tecla.R,
                Tecla.W
            },
            new List<Tecla>
            {
                Tecla.A,
                Tecla.R,
                Tecla.W,
                Tecla.E
            },
            new List<Tecla>
            {
                Tecla.A,
                Tecla.R,
                Tecla.W,
                Tecla.E,
                Tecla.Q
            }
        );

        public static readonly CombinacionTeclas ARAION = new(
            new List<Tecla>
            {
                Tecla.A,
                Tecla.R,
                Tecla.W,
                Tecla.E
            },
            new List<Tecla>
            {
                Tecla.A,
                Tecla.R,
                Tecla.W,
                Tecla.E,
                Tecla.Q
            },
            new List<Tecla>
            {
                Tecla.A,
                Tecla.R,
                Tecla.W,
                Tecla.E,
                Tecla.Q,
                Tecla.S
            }
        );

        public static readonly CombinacionTeclas AERONIS = new(
            new List<Tecla>
            {
                Tecla.A,
                Tecla.R,
                Tecla.W,
                Tecla.E,
                Tecla.Q
            },
            new List<Tecla>
            {
                Tecla.A,
                Tecla.R,
                Tecla.W,
                Tecla.E,
                Tecla.Q,
                Tecla.S
            },
            new List<Tecla>
            {
                Tecla.A,
                Tecla.R,
                Tecla.W,
                Tecla.E,
                Tecla.Q,
                Tecla.S,
                Tecla.D
            }
        );

        public static readonly CombinacionTeclas AERAVON = new(
            new List<Tecla>
            {
                Tecla.A,
                Tecla.R,
                Tecla.W,
                Tecla.E,
                Tecla.Q,
                Tecla.S
            },
            new List<Tecla>
            {
                Tecla.A,
                Tecla.R,
                Tecla.W,
                Tecla.E,
                Tecla.Q,
                Tecla.S,
                Tecla.D
            },
            new List<Tecla>
            {
                Tecla.A,
                Tecla.R,
                Tecla.W,
                Tecla.E,
                Tecla.Q,
                Tecla.S,
                Tecla.D,
                Tecla.D
            }
        );
    }
}