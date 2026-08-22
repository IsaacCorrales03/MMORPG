using Godot;
using Shared.Magia;
using System;
using System.Collections.Generic;

public enum EstadoCasteo
{
	Inactivo,
	EnProgreso,
	ListoParaUsar,   // terminó teclas antes del tiempo mínimo, esperando que se cumpla
	Cancelado,
	Completado
}

public partial class SistemaCasteo : Node
{
	public event Action<int> TeclaCorrecta;       // índice de la tecla acertada
	public event Action<int> TeclaFallada;        // índice de la tecla fallada
	public event Action CasteoCancelado;
	public event Action CasteoCompletado;
	public event Action CasteoListoParaUsar;
	public event Action<string> CasteoRechazado;
	public event Action EntroEnEsperaMinima;

	private Hechizo _hechizo;
	private List<Tecla> _secuencia;
	private int _indiceActual;
	private float _tiempoTranscurrido;
	private const float PenalizacionPorFallo = 0.5f;

	public EstadoCasteo Estado { get; private set; } = EstadoCasteo.Inactivo;
	public bool BloqueaOtrosInputs => Estado == EstadoCasteo.EnProgreso;
	public Hechizo HechizoActual => _hechizo;
	private float _tiempoAlEntrarEsperaMinima;
	public IReadOnlyList<Tecla> Secuencia => _secuencia;
	public int IndiceActual => _indiceActual;

	public float ProgresoEsperaMinima()
	{
		if (_hechizo == null) return 0f;
		float restante = _hechizo.TiempoMinimo - _tiempoAlEntrarEsperaMinima;
		if (restante <= 0f) return 1f;
		return Mathf.Clamp((_tiempoTranscurrido - _tiempoAlEntrarEsperaMinima) / restante, 0f, 1f);
	}
	public override void _Ready()
	{
		CasteoListoParaUsar += OnCasteoListoParaUsar;
	}

	private void OnCasteoListoParaUsar()
	{
		GD.Print("El hechizo está listo");
	}

	public event Action<Hechizo, IReadOnlyList<Tecla>> CasteoIniciado;

	public void IniciarCasteo(Hechizo hechizo, int nivelUsuario)
	{
		if (Estado == EstadoCasteo.EnProgreso || Estado == EstadoCasteo.ListoParaUsar)
		{
			CasteoRechazado?.Invoke("Estás concentrando tu energía en un hechizo");
			return;
		}

		_hechizo = hechizo;
		_secuencia = hechizo.ObtenerSecuencia(nivelUsuario);
		_indiceActual = 0;
		_tiempoTranscurrido = 0f;
		Estado = EstadoCasteo.EnProgreso;

		CasteoIniciado?.Invoke(_hechizo, _secuencia); // <- nuevo
	}

	public override void _Process(double delta)
	{
		if (Estado == EstadoCasteo.EnProgreso)
		{
			_tiempoTranscurrido += (float)delta;

			if (_tiempoTranscurrido >= _hechizo.TiempoMaximo)
			{
				Estado = EstadoCasteo.Cancelado;
				CasteoCancelado?.Invoke();
			}
			return;
		}

		if (Estado == EstadoCasteo.ListoParaUsar)
		{
			_tiempoTranscurrido += (float)delta;

			if (_tiempoTranscurrido >= _hechizo.TiempoMinimo)
			{
				Estado = EstadoCasteo.Completado;
				CasteoListoParaUsar?.Invoke();
				CasteoCompletado?.Invoke();
			}
			return;
		}
	}
	public override void _UnhandledKeyInput(InputEvent @event)
	{
		if (Estado != EstadoCasteo.EnProgreso) return;

		if (@event is InputEventKey keyEvent && keyEvent.Pressed && !keyEvent.Echo)
		{
			if (TeclaMapper.TryMapear(keyEvent.Keycode, out Tecla tecla))
			{
				RecibirTecla(tecla);
				GetViewport().SetInputAsHandled(); // consume el evento
			}
		}
	}

	public void RecibirTecla(Tecla tecla)
	{
		if (Estado != EstadoCasteo.EnProgreso) return;

		if (_secuencia[_indiceActual] == tecla)
		{
			TeclaCorrecta?.Invoke(_indiceActual);
			_indiceActual++;

			if (_indiceActual >= _secuencia.Count)
			{
				if (_tiempoTranscurrido < _hechizo.TiempoMinimo)
				{
					Estado = EstadoCasteo.ListoParaUsar; // sigue corriendo el timer hasta el mínimo
					_tiempoAlEntrarEsperaMinima = _tiempoTranscurrido;

					EntroEnEsperaMinima?.Invoke();
				}
				else
				{
					Estado = EstadoCasteo.Completado;
					CasteoCompletado?.Invoke();
				}
			}
		}
		else
		{
			TeclaFallada?.Invoke(_indiceActual);
			_tiempoTranscurrido += PenalizacionPorFallo;
		}
	}

	public float ProgresoTiempo() =>
		_hechizo == null ? 0f : Mathf.Clamp(_tiempoTranscurrido / _hechizo.TiempoMaximo, 0f, 1f);



	public void FinalizarUso()
	{
		// Llamar después de instanciar el AreaHechizo, para permitir castear de nuevo
		Estado = EstadoCasteo.Inactivo;
		_hechizo = null;
		_secuencia = null;
		_indiceActual = 0;
		_tiempoTranscurrido = 0f;
	}
}