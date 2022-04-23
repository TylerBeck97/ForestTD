using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TestGame.Services;

namespace SpaceMarines_TD.Source
{
    public class DrawingService
    {
        private readonly GraphicsDeviceManager _graphics;

        private Texture2D _pixelTexture;
        private Viewport _viewport;
        
        public bool IsFullscreen => _graphics.IsFullScreen;

        public bool IsZoomedOut { get; set; }

        public SpriteBatch SpriteBatch { get; private set; }

        public DrawingService(GraphicsDeviceManager graphics)
        {
            _graphics = graphics;
        }

        public void Load()
        {
            SpriteBatch = new SpriteBatch(_graphics.GraphicsDevice);
            _viewport = _graphics.GraphicsDevice.Viewport;

            _pixelTexture = new Texture2D(SpriteBatch.GraphicsDevice, 1, 1);
            var colorData = new Color[1] { Color.White };
            _pixelTexture.SetData(colorData);

            IsZoomedOut = false;

            _graphics.SynchronizeWithVerticalRetrace = false;
        }

        public void DrawPolygon(Vector2[] points)
        {
            for (var i = 0; i < points.Length; i++)
            {
                if (i != points.Length - 1)
                {
                    DrawLine(points[i], points[i + 1], Color.Blue);
                }
                else
                {
                    DrawLine(points[i], points[0], Color.Red);
                }
            }
        }

        public void DrawLine(Vector2 p1, Vector2 p2, Color color, int thickness = 3)
        {
            var length = (p2 - p1).Length();
            var angle = MathF.Atan2(p2.Y - p1.Y, p2.X - p1.X) - MathHelper.PiOver2;
            //var center = (p2 + p1) / 2;

            SpriteBatch.Draw(_pixelTexture,
                new Rectangle((int)Math.Round(p1.X), (int)Math.Round(p1.Y), thickness, (int)Math.Round(length)),
                _pixelTexture.Bounds, color, angle, new Vector2(.0f, .0f), SpriteEffects.None, 0);
        }

        public void ToggleFullscreen()
        {
            _graphics.IsFullScreen = !IsFullscreen;

            if (IsFullscreen)
            {
                _graphics.PreferredBackBufferWidth = _graphics.GraphicsDevice.DisplayMode.Width;
                _graphics.PreferredBackBufferHeight = _graphics.GraphicsDevice.DisplayMode.Height;
            }
            else
            {
                _graphics.PreferredBackBufferWidth = _graphics.GraphicsDevice.DisplayMode.Width / 2;
                _graphics.PreferredBackBufferHeight = _graphics.GraphicsDevice.DisplayMode.Height / 2;
            }

            _graphics.ApplyChanges();
        }
    }
}
