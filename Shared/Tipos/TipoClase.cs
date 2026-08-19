namespace Shared.Clases
{
    public enum TipoClase
    {
        Mago = 1
    }
        public static class CatalogoClases
    {
        public static Clase Crear(int claseId)
        {
            return claseId switch
            {
                (int)TipoClase.Mago => new Mago(),
                _ => throw new ArgumentOutOfRangeException(
                    nameof(claseId),
                    $"ClaseId inválido: {claseId}"
                )
            };
        }
    }
}