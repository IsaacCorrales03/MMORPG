using Godot;
using System;

public partial class Mago : Node2D
{
	[Export] private TomosManager _tomoManager;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("MG");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
