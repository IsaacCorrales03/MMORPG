using Client.Juego;
using Godot;
using Shared.Paquetes;
using Shared.Utils;
using System;
using System.Collections.Generic;

public partial class World : Node2D
{
	[Export]
	public PackedScene _jugadorEscena;

	private readonly Dictionary<int, Player> _players = new();
	private readonly HashSet<int> _jugadoresRecibidos = new();
	private readonly List<int> _jugadoresParaEliminar = new();

	public override void _Ready()
	{
		GameState.Instance.AparecerJugador += OnAparecerJugador;
		GameState.Instance.SnapshotRecibido += OnSnapshotRecibido;
		GameState.Instance.CorregirMovimiento += OnCorregirMovimiento;
	}


	public override void _ExitTree()
	{
		if (GameState.Instance == null)
			return;
		//PacketSender.EnviarOrdenado()
		GameState.Instance.AparecerJugador -= OnAparecerJugador;
		GameState.Instance.SnapshotRecibido -= OnSnapshotRecibido;
	}


	// =========================================================
	// JUGADOR LOCAL
	// =========================================================

	private void OnCorregirMovimiento(int playerId, long LastSequenceProcessed, Vector2 posicion)
	{
		_players.TryGetValue(playerId, out Player jugador);
		if (jugador != null)
		{
			jugador.AplicarCorrecciónDePosicion(posicion, LastSequenceProcessed);
		}
	}

	public void OnAparecerJugador()
	{
		int playerId = GameState.IdUsuario ?? 0;

		// Evitar duplicarlo.
		if (_players.ContainsKey(playerId))
			return;

		Player jugador = _jugadorEscena.Instantiate<Player>();

		AddChild(jugador);

		jugador.Configurar(
			playerId,
			true,
			GameState.NombreUsuario
		);

		_players.Add(
			playerId,
			jugador
		);

		GD.Print(
			$"Jugador local creado: {playerId}"
		);
	}


	// =========================================================
	// SNAPSHOT
	// =========================================================

	// recibimos las snapshots, las cuales tienen a los jugadores
	// al terminar de procesar jugadores, los que no estén en la lista de snapshots, se eliminan
	private void OnSnapshotRecibido(PaqueteSnapshots paquete)
	{
		int playerIdLocal = GameState.IdUsuario ?? 0;
		_jugadoresRecibidos.Clear();
		foreach (PlayerSnapshot snapshot in paquete.Players)
		{
			_jugadoresRecibidos.Add(snapshot.PlayerId);
			// El snapshot NO controla al jugador local.
			if (snapshot.PlayerId == playerIdLocal)
			{
				if (_players.TryGetValue(playerIdLocal, out Player localJugador))
				{
					localJugador.LimpiarInputs(paquete.LastSequenceProcessed);
				}

				continue;
			}


			// -----------------------------------------
			// Crear jugador remoto si no existe
			// -----------------------------------------

			if (!_players.TryGetValue(snapshot.PlayerId, out Player jugador))
			{
				jugador = _jugadorEscena.Instantiate<Player>();

				AddChild(jugador);

				jugador.Configurar(snapshot.PlayerId, false, snapshot.Nombre);

				_players.Add(snapshot.PlayerId, jugador);

				GD.Print(
					$"Jugador remoto creado: {snapshot.PlayerId}"
				);
			}


			// -----------------------------------------
			// Actualizar objetivo
			// -----------------------------------------

			Vector2 posicion = new Vector2(
				snapshot.Position.X,
				snapshot.Position.Y
			);
			Vector2 direccion = new(
				snapshot.Direction.X,
				snapshot.Direction.Y
			);


			jugador.AplicarSnapshot(posicion);
			jugador.AplicarEstadoRemoto(direccion, snapshot.Moving);
		}
		_jugadoresParaEliminar.Clear();
		foreach (int id in _players.Keys)
		{
			if (!_jugadoresRecibidos.Contains(id))
				_jugadoresParaEliminar.Add(id);
		}
		foreach (int id in _jugadoresParaEliminar)
		{
			GD.Print($"Jugador remoto eliminado - {_players[id].nombre}");
			_players[id].QueueFree();
			_players.Remove(id);
		}
	}


	// =========================================================
	// PROCESS
	// =========================================================

	public override void _Process(double delta)
	{
	}
}