using System;
using Godot;
using Shared.Paquetes;
using Shared.Utils;

namespace Client.Juego
{
    public partial class GameState : Node
    {
        public static GameState Instance { get; private set; }
        public static string Token { get; set; }
        public static string NombreUsuario { get; set; }
        public static int? IdUsuario { get; set; }

        public static bool Conectado { get; set; }
        public static bool SesionReanudada { get; set; }

        public event Action<PaqueteSnapshots> SnapshotRecibido;

        [Signal] public delegate void RegistroExitosoEventHandler();
        [Signal] public delegate void RegistroFallidoEventHandler(string mensaje);

        [Signal] public delegate void SesionReanudadaExitosaEventHandler();
        [Signal] public delegate void SesionReanudadaFallidaEventHandler(string mensaje);

        [Signal] public delegate void InicioSesionExitosoEventHandler();
        [Signal] public delegate void InicioSesionFallidoEventHandler(string mensaje);
        [Signal] public delegate void AparecerJugadorEventHandler();

        public string TokenSesion;

        public override void _Ready() => Instance = this;
        public void RecibirSnapshot(PaqueteSnapshots paquete)
        {
            SnapshotRecibido?.Invoke(paquete);
        }
        public void IniciarJuego()
        {
            GetTree().ChangeSceneToFile("res://Escenas/world.tscn");
            PaquetePeticionAparecerJugador peticion = new()
            {
                JugadorId = IdUsuario ?? 0,
                Position = new Shared.Tipos.Vector2()
            };
            PacketSender.EnviarOrdenado(Cliente.Instancia.Peer, peticion);
        }
    }
}