using Godot;
using System;
using Shared.Magia;

public partial class TomosManager : Node
{
	public Tomo Tomo1 { get; private set; } = Tomos.Basico.Clonar();
	public Tomo Tomo2 { get; private set; } = Tomos.Infernal.Clonar();
	public int TomoSeleccionado { get; private set; } = 1;
	public Tomo TomoActual => TomoSeleccionado == 1 ? Tomo1 : Tomo2;

	public event Action<Tomo> TomoCambiado;

	public void AsignarTomo(int slot, Tomo tomo)
	{
		switch (slot)
		{
			case 1:
				Tomo1 = tomo;
				break;

			case 2:
				Tomo2 = tomo;
				break;

			default:
				throw new ArgumentOutOfRangeException(nameof(slot));
		}
		TomoCambiado?.Invoke(TomoActual);
	}
	public void SeleccionarTomo(int slot)
	{
		if (slot is < 1 or > 2)
			throw new ArgumentOutOfRangeException(nameof(slot));

		TomoSeleccionado = slot;
		TomoCambiado?.Invoke(TomoActual);
	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Tomo1.AsignarHechizo(CatalogoHechizos.Fulis); // lvl 1
		Tomo1.AsignarHechizo(CatalogoHechizos.Terara); // lvl 3
		Tomo1.AsignarHechizo(CatalogoHechizos.Aquaraion); //lvl 4
		Tomo1.AsignarHechizo(CatalogoHechizos.Cryaeravon); //lvl 6
		Tomo2.AsignarHechizo(CatalogoHechizos.Sanaeronis); // lvl 5
		Tomo2.AsignarHechizo(CatalogoHechizos.Umbaeravon);
		Tomo2.AsignarHechizo(CatalogoHechizos.Aeraraion);
		Tomo2.AsignarHechizo(CatalogoHechizos.Ignaraion);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
