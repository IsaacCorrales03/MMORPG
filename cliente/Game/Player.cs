using Client.Juego;
using Godot;
using Shared.Paquetes;
using Shared.Utils;
using System;
using System.Collections.Generic;

public partial class Player : CharacterBody2D
{
	public const float MoveSpeed = 200.0f;
	private readonly HashSet<object> _bloqueadoresMovimiento = new();

	public bool MovimientoBloqueado => _bloqueadoresMovimiento.Count > 0;

	public void BloquearMovimiento(object solicitante) => _bloqueadoresMovimiento.Add(solicitante);
	public void DesbloquearMovimiento(object solicitante) => _bloqueadoresMovimiento.Remove(solicitante);


	private long sequence = 0;

	[Export] public AnimatedSprite2D sprite;
	[Export] public Area2D hurtbox;
	[Export] public Label nombre;
	[Export] public Camera2D camera;
	public CollisionShape2D hurtBoxShape;

	private Vector2 lastDirection = Vector2.Down;
	private Vector2 lastInput = Vector2.Zero;

	private bool isAttacking = false;

	private Queue<PaqueteMovimiento> inputsPendientes = new();

	public bool EsLocal { get; private set; }
	public int PlayerId { get; private set; }

	public bool isMultiplayerAuthority;
	public bool isMovingInTheSameDirection;

	public int consecutive = 1;

	// ---------- Interpolación remota ----------

	private Vector2 targetPosition;
	private bool hasTargetPosition = false;

	// Velocidad de seguimiento visual.
	private const float RemoteInterpolationSpeed = 8.0f;


	public override void _Ready()
	{
		CrearClase();
		isMultiplayerAuthority = IsMultiplayerAuthority();

		sprite.AnimationFinished += OnAnimationFinished;

		hurtBoxShape = hurtbox.GetChild<CollisionShape2D>(0);

		if (nombre == null)
			nombre = GetNode<Label>("Etiqueta/Nombre");
	}
	private void CrearClase()
	{

		PackedScene escenaMago = GD.Load<PackedScene>("res://Clases/Mago/Mago.tscn");

		Mago mago = escenaMago.Instantiate<Mago>();

		AddChild(mago);
	}

	// ---------- Configuración ----------

	public void Configurar(int playerId, bool esLocal, string nombreJugador)
	{
		PlayerId = playerId;
		EsLocal = esLocal;

		nombre.Text = nombreJugador;
		camera.Enabled = esLocal;
	}

	public void LimpiarInputs(long lastSequenceProcessed)
	{
		while (inputsPendientes.Count > 0 && inputsPendientes.Peek().Sequence <= lastSequenceProcessed)
		{
			inputsPendientes.Dequeue();
		}
	}

	// ---------- Snapshots ----------

	public void AplicarSnapshot(Vector2 posicion)
	{
		targetPosition = posicion;

		if (!hasTargetPosition)
		{
			Position = posicion;
			hasTargetPosition = true;
		}
	}
	public void AplicarCorrecciónDePosicion(Vector2 posicionDelServer, long lastSequenceProcessed)
	{
		LimpiarInputs(lastSequenceProcessed);
		Position = posicionDelServer;
		foreach (PaqueteMovimiento input in inputsPendientes)
		{
			ReaplicarInput(input);
		}
	}
	private void ReaplicarInput(PaqueteMovimiento movimiento)
	{
		Vector2 input = new(
			movimiento.Input.X,
			movimiento.Input.Y
		);

		Move(input);
		MoveAndSlide();
	}

	public void AplicarEstadoRemoto(Vector2 direction, bool moving)
	{
		if (!moving)
		{
			Velocity = Vector2.Zero;

			switch (lastDirection)
			{
				case var d when d == Vector2.Left:
					sprite.Play("idle_left");
					break;

				case var d when d == Vector2.Right:
					sprite.Play("idle_right");
					break;

				case var d when d == Vector2.Up:
					sprite.Play("idle_up");
					break;

				default:
					sprite.Play("idle_down");
					break;
			}

			return;
		}

		lastDirection = direction.Normalized();

		if (lastDirection == Vector2.Left)
			sprite.Play("walk_left");
		else if (lastDirection == Vector2.Right)
			sprite.Play("walk_right");
		else if (lastDirection == Vector2.Up)
			sprite.Play("walk_up");
		else if (lastDirection == Vector2.Down)
			sprite.Play("walk_down");
	}


	// ---------- Ataque ----------

	public void Attack()
	{
		if (isAttacking)
			return;

		isAttacking = true;

		hurtBoxShape.Disabled = false;

		Velocity = Vector2.Zero;

		switch (lastDirection)
		{
			case var d when d == Vector2.Left:
				sprite.Play("attack_left");
				hurtbox.Position = new Vector2(-20, 0);
				break;

			case var d when d == Vector2.Right:
				sprite.Play("attack_right");
				hurtbox.Position = new Vector2(20, 0);
				break;

			case var d when d == Vector2.Up:
				sprite.Play("attack_up");
				hurtbox.Position = new Vector2(0, -20);
				break;

			case var d when d == Vector2.Down:
				sprite.Play("attack_down");
				hurtbox.Position = new Vector2(0, 20);
				break;
		}
	}


	private void OnAnimationFinished()
	{
		if (!isAttacking)
			return;

		isAttacking = false;

		hurtBoxShape.Disabled = true;
	}


	// ---------- Movimiento local ----------

	public void Move(Vector2 direction)
	{
		Velocity = direction * MoveSpeed;

		Vector2 currentDirection = direction.Normalized();

		isMovingInTheSameDirection =
			currentDirection == lastDirection;

		if (direction != Vector2.Zero)
			lastDirection = currentDirection;

		if (currentDirection == Vector2.Left)
			sprite.Play("walk_left");
		else if (currentDirection == Vector2.Right)
			sprite.Play("walk_right");
		else if (currentDirection == Vector2.Up)
			sprite.Play("walk_up");
		else if (currentDirection == Vector2.Down)
			sprite.Play("walk_down");
		else
		{
			if (lastDirection == Vector2.Left)
				sprite.Play("idle_left");
			else if (lastDirection == Vector2.Right)
				sprite.Play("idle_right");
			else if (lastDirection == Vector2.Up)
				sprite.Play("idle_up");
			else
				sprite.Play("idle_down");
		}
	}


	public override void _PhysicsProcess(double delta)
	{
		// Los jugadores remotos NO ejecutan movimiento local.
		if (!EsLocal)
			return;
		if (MovimientoBloqueado)
		{
			Velocity = Vector2.Zero;
			MoveAndSlide();
			return;
		}
		sequence++;

		Vector2 direction = GetInputDirection();

		bool sameInput = direction == lastInput;

		Vector2 firstPosition = Position;

		Move(direction);
		MoveAndSlide();

		Vector2 finalPosition = Position;

		bool moved = firstPosition != finalPosition;

		if (sameInput)
			consecutive += 1;
		else
			consecutive = 1;

		lastInput = direction;

		PaqueteMovimiento movimiento = new()
		{
			Sequence = sequence,

			Input = new Shared.Tipos.Vector2(
				lastInput.X,
				lastInput.Y
			),

			Consecutive = consecutive,

			ReportedPosition = new Shared.Tipos.Vector2(
				finalPosition.X,
				finalPosition.Y
			),

			Moved = moved
		};

		PacketSender.EnviarMovimiento(
			Cliente.Instancia.Peer,
			movimiento
		);
		inputsPendientes.Enqueue(movimiento);
	}


	// ---------- Interpolación remota ----------

	public override void _Process(double delta)
	{
		if (EsLocal || !hasTargetPosition)
			return;

		float weight = 1.0f - Mathf.Exp(-RemoteInterpolationSpeed * (float)delta);

		Position = Position.Lerp(
			targetPosition,
			weight
		);
	}


	// ---------- Input ----------

	public override void _Input(InputEvent @event)
	{
		if (!EsLocal)
			return;

		if (@event is InputEventMouseButton mouseEvent &&
			mouseEvent.ButtonIndex == MouseButton.Left &&
			mouseEvent.Pressed)
		{
			Attack();
		}
	}


	private Vector2 GetInputDirection()
	{
		return Input.GetVector(
			"ui_left",
			"ui_right",
			"ui_up",
			"ui_down"
		);
	}
}