using Godot;
public partial class Camera : Camera2D
{
    private const float MinZoom = 2f;
    private const float MaxZoom = 6.0f;
    private const float ZoomStep = 0.2f;
    private const float FollowSpeed = 10.0f;
    private const float SnapThresholdSq = 0.01f; // ~0.1 px

    private Node2D player;
    private bool cameraLocked = true;

    public override void _Ready()
    {
        player = GetParent<Node2D>();
        TopLevel = true;
        GlobalPosition = player.GlobalPosition.Round();
    }

    public override void _Process(double delta)
    {
        // SOLO input aquí, nunca movimiento de cámara
        if (Input.IsActionJustPressed("lock_camera"))
        {
            cameraLocked = !cameraLocked;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!cameraLocked)
            return;

        float weight = 1f - Mathf.Exp(-FollowSpeed * (float)delta);
        var target = player.GlobalPosition;
        var newPos = GlobalPosition.Lerp(target, weight);

        if (newPos.DistanceSquaredTo(target) < SnapThresholdSq)
            newPos = target;

        GlobalPosition = newPos.Round();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
        {
            if (mouseEvent.ButtonIndex == MouseButton.WheelUp)
                ChangeZoom(ZoomStep);
            else if (mouseEvent.ButtonIndex == MouseButton.WheelDown)
                ChangeZoom(-ZoomStep);
        }
    }

    private void ChangeZoom(float amount)
    {
        float newZoom = Mathf.Clamp(Zoom.X + amount, MinZoom, MaxZoom);
        Zoom = new Vector2(newZoom, newZoom);
    }
}