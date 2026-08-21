using Godot;

public static class TeclaMapper
{
    public static bool TryMapear(Key keycode, out Tecla tecla)
    {
        switch (keycode)
        {
            case Key.W: tecla = Tecla.W; return true;
            case Key.A: tecla = Tecla.A; return true;
            case Key.S: tecla = Tecla.S; return true;
            case Key.D: tecla = Tecla.D; return true;
            case Key.R: tecla = Tecla.R; return true;
            case Key.E: tecla = Tecla.E; return true;
            case Key.Q: tecla = Tecla.Q; return true;
            default:
                tecla = default;
                return false;
        }
    }
}