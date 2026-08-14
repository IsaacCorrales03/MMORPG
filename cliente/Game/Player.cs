using Godot;
using Shared.Paquetes;
using System;

public partial class Player : CharacterBody2D
{
	public const float Speed = 200.0f;

	[Export] public AnimatedSprite2D sprite;
	[Export] public Area2D hurtbox;
	public CollisionShape2D hurtBoxShape;
	private Vector2 lastDirection = Vector2.Down;
	private bool isAttacking = false;
	public bool isMultiplayerAuthority;
	


	public override void _Ready()
	{
		isMultiplayerAuthority = IsMultiplayerAuthority();

		sprite.AnimationFinished += OnAnimationFinished;
		CollisionShape2D hurtBoxShape = hurtbox.GetChild<CollisionShape2D>(0);
	}


	public void Attack()
	{
		if (isAttacking)
			return;

		isAttacking = true;
		hurtbox.GetChild<CollisionShape2D>(0).Disabled = false;

		Velocity = Vector2.Zero;

		switch (lastDirection)
		{
			case var d when d == Vector2.Left:
				sprite.Play("attack_left");
				hurtbox.Position = new Vector2 {X = -20, Y = 0};
				
				break;

			case var d when d == Vector2.Right:
				sprite.Play("attack_right");
				hurtbox.Position = new Vector2 {X = 20, Y = 0};
				break;

			case var d when d == Vector2.Up:
				sprite.Play("attack_up");
				hurtbox.Position = new Vector2 {X = 0, Y = -20};
				break;

			case var d when d == Vector2.Down:
				sprite.Play("attack_down");
				hurtbox.Position = new Vector2 {X = 0, Y = 20};
				
				break;
		}
	}


	private void OnAnimationFinished()
	{
		if (!isAttacking)
			return;

		isAttacking = false;
		hurtbox.GetChild<CollisionShape2D>(0).Disabled = true;
	}


	public void Move(Vector2 direction)
	{
		if (isAttacking)
		{
			Velocity = Vector2.Zero;
			return;
		}

		if (direction == Vector2.Zero)
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

				case var d when d == Vector2.Down:
					sprite.Play("idle_down");
					break;
			}

			return;
		}

		Velocity = direction * Speed;
		
		PaqueteMovimiento paqueteMovimiento = new()
		{
			ReportedPosition = new Shared.Tipos.Vector2(Position.X, Position.Y)
		};

		if (direction.X < 0)
		{
			if (lastDirection == Vector2.Left)
			{
				
			}
			lastDirection = Vector2.Left;
			sprite.Play("walk_left");
		}
		else if (direction.X > 0)
		{
			lastDirection = Vector2.Right;
			sprite.Play("walk_right");
		}
		else if (direction.Y < 0)
		{
			lastDirection = Vector2.Up;
			sprite.Play("walk_up");
		}
		else if (direction.Y > 0)
		{
			lastDirection = Vector2.Down;
			sprite.Play("walk_down");
		}
	}


	public override void _PhysicsProcess(double delta)
	{
		if (!IsMultiplayerAuthority())
        return;
		Vector2 direction = GetInputDirection();

		Move(direction);
		MoveAndSlide();
	}


	public override void _Input(InputEvent @event)
	{
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