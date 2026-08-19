namespace Shared.Clases
{
    public abstract class Clase
    {
        public string Nombre { get; protected set; } = "";
        public int Nivel { get; protected set; }

        // recursos
        public int VidaBase { get; protected set; }
        public int EstaminaBase { get; protected set; }
        public int ManaBase { get; protected set; }

        // Estadisticas ofensivas
        public int AtaqueBasico { get; protected set; }
        public int PoderDeHabilidad { get; protected set; }

        // Estadisticas defensivas
        public int ArmaduraBase { get; protected set; }
        public int ResistenciaMagica { get; protected set; }

        // Otras estadisticas
        public float VelocidadDeMovimiento { get; protected set; }
    }
}