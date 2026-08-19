using Shared.Magia;

namespace Shared.Clases
{
    public class Mago : Clase
    {
        public Tomo Tomo1 { get; protected set; }
        public Tomo Tomo2 { get; protected set; }

        public Mago()
        {
            Nombre = "Mago";
            Nivel = 1;

            // Recursos
            VidaBase = 80;
            EstaminaBase = 100;
            ManaBase = 200;

            // Ofensivas
            AtaqueBasico = 20;
            PoderDeHabilidad = 100;

            // Defensivas
            ArmaduraBase = 10;
            ResistenciaMagica = 20;

            // Movimiento
            VelocidadDeMovimiento = 200f;

            // Tomos iniciales
            Tomo1 = Tomos.Basico;
            Tomo2 = Tomos.Basico;
        }
        public void EquiparTomo(int ranura, Tomo tomo)
        {
            if (ranura == 1)
                Tomo1 = tomo;
            else if (ranura == 2)
                Tomo2 = tomo;
            else
                throw new ArgumentOutOfRangeException(nameof(ranura));
        }
    }
}