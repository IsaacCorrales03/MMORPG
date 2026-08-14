using Client.Juego;
using Godot;
using System;

public partial class MainMenu : Node
{
	public Label usuario;
	[Export] public PackedScene playerEscene;

	public override void _Ready()
	{
		CharacterBody2D player = playerEscene.Instantiate<CharacterBody2D>();
		AddChild(player);
		player.GetNode<Label>("Etiqueta/Nombre").Text = GameState.NombreUsuario;
		player.GetNode<Label>("Etiqueta/Nombre").GlobalPosition = player.GetNode<Label>("Etiqueta/Nombre").GlobalPosition.Round();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
