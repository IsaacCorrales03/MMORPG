namespace Shared.Magia
{
    public static class Tomos
    {
        public static readonly Tomo Basico = new(
            "Tomo Básico",
            Elemento.Ninguno,
            Elemento.Ninguno
        );

        public static readonly Tomo Infernal = new(
            "Tomo Infernal",
            Elemento.Fuego,
            Elemento.Ninguno
        );
    }
}