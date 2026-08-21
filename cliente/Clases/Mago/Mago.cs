using Godot;
using System;

public partial class Mago : Node2D
{
	[Export] private TomosManager _tomoManager;
	[Export] private SistemaCasteo _sistemaCasteo;
	private Player _player;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_player = GetParent<Player>();
		_sistemaCasteo.CasteoIniciado += (_, _) => _player?.BloquearMovimiento(this);
		_sistemaCasteo.CasteoCancelado += () => _player?.DesbloquearMovimiento(this);
		_sistemaCasteo.CasteoCompletado += () => _player?.DesbloquearMovimiento(this);
		_sistemaCasteo.EntroEnEsperaMinima += () => _player?.DesbloquearMovimiento(this);

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
