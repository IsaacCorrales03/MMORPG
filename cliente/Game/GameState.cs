using Godot;
using Shared.Paquetes;
using Shared.Utils;

namespace Client.Juego
{
    public partial class GameState : Node
    {
        public static GameState Instance { get; private set; }
        public static string Token { get; set; }
        public static string NombreUsuario {get; set;}
        public static int? IdUsuario {get; set;}
        
        public static bool Conectado {get; set;}
        public static bool SesionReanudada {get; set;}
        [Signal] public delegate void RegistroExitosoEventHandler();
        [Signal] public delegate void RegistroFallidoEventHandler(string mensaje);

        [Signal] public delegate void SesionReanudadaExitosaEventHandler();
        [Signal] public delegate void SesionReanudadaFallidaEventHandler(string mensaje);
        
        [Signal] public delegate void InicioSesionExitosoEventHandler();
        [Signal] public delegate void InicioSesionFallidoEventHandler(string mensaje);
        
        public string TokenSesion;

        public override void _Ready() => Instance = this;

        public void IniciarJuego()
        {
            PaquetePeticionAparecerJugador peticion = new()
            {
                JugadorId = GameState.IdUsuario ?? 0,
                position = new Shared.Tipos.Vector2()
            };
            PacketSender.EnviarTCP(Cliente.Instancia.Peer, peticion);
        }
    }
}