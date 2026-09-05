using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

// TODO(wam): delete once the entry point is proven. Nothing here reads content or shaders, it is
// only asking whether FNA comes up on SDLThread at all. docs/android-fna-plan.md
internal sealed class SpikeGame : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private int _frames;

    public SpikeGame()
    {
        _graphics = new GraphicsDeviceManager(this)
        {
            IsFullScreen = true,
            SynchronizeWithVerticalRetrace = true,
        };
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        base.Draw(gameTime);
    }
}
