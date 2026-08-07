using Client.Juego;
using Godot;
using System;

public partial class MainMenu : Node
{
	public Label usuario;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		usuario = GetNode<Label>("Usuario");
		usuario.Text = GameState.NombreUsuario;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
