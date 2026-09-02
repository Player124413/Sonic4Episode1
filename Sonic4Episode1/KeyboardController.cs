using Microsoft.Xna.Framework.Input;

internal class KeyboardController : Controller
{
    public override void SetVibration(ushort left, ushort right) { }
    public override void SetVibrationEnabled(bool enabled) { }

    public override void UpdateControllerReading(ref ControllerReading reading)
    {
        var state = Keyboard.GetState();
        if (state.IsKeyDown(Keys.Up) || state.IsKeyDown(Keys.I))
            reading.direction |= BUTTON.UP;
        if (state.IsKeyDown(Keys.Down) || state.IsKeyDown(Keys.K))
            reading.direction |= BUTTON.DOWN;
        if (state.IsKeyDown(Keys.Left) || state.IsKeyDown(Keys.J))
            reading.direction |= BUTTON.LEFT;
        if (state.IsKeyDown(Keys.Right) || state.IsKeyDown(Keys.L))
            reading.direction |= BUTTON.RIGHT;
        if (state.IsKeyDown(Keys.A))
            reading.direction |= BUTTON.A;
        if (state.IsKeyDown(Keys.S))
            reading.direction |= BUTTON.B;
        if (state.IsKeyDown(Keys.D))
            reading.direction |= BUTTON.X;
        if (state.IsKeyDown(Keys.Q))
            reading.direction |= BUTTON.L;
        if (state.IsKeyDown(Keys.W))
            reading.direction |= BUTTON.Y;
        if (state.IsKeyDown(Keys.E))
            reading.direction |= BUTTON.R;
        if (state.IsKeyDown(Keys.Escape) || state.IsKeyDown(Keys.Enter))
            reading.direction |= BUTTON.START;
    }
}