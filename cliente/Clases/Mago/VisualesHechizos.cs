using Godot;

// Una entrada del mapeo: nombre del hechizo -> textura de su área de efecto
[GlobalClass]
public partial class VisualHechizo : Resource
{
    [Export] public string NombreHechizo { get; set; }
    [Export] public Texture2D Textura { get; set; }
}

// Nodo (asignalo como Autoload o child en la escena del Mago) que guarda
// la lista de VisualHechizo y resuelve la textura por nombre.
public partial class VisualesHechizos : Node
{
    [Export] public VisualHechizo[] Visuales { get; set; } = System.Array.Empty<VisualHechizo>();

    private System.Collections.Generic.Dictionary<string, Texture2D> _mapa;

    public override void _Ready()
    {
        _mapa = new System.Collections.Generic.Dictionary<string, Texture2D>();
        foreach (var v in Visuales)
        {
            if (v == null || string.IsNullOrEmpty(v.NombreHechizo)) continue;
            _mapa[v.NombreHechizo] = v.Textura;
        }
    }

    public Texture2D ObtenerTextura(string nombreHechizo)
    {
        if (_mapa != null && _mapa.TryGetValue(nombreHechizo, out var textura))
            return textura;

        return null; 
    }
}
