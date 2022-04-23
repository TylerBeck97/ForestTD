using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace SpaceMarines_TD.Source.Views
{
    interface IGameState
    {
        void initialize(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics);
        void loadContent(ContentManager contentManager);
        GameStateEnum processInput(GameTime gameTime);
        void update(GameTime gameTime);
        void render(GameTime gameTime);
    }
}
