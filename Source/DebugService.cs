using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpaceMarines_TD.Source;

namespace TestGame.Services
{
    public class DebugService
    {
        private readonly DrawingService _drawingService;

        // Debug features
        private readonly List<DebugGeometry> _debugGeometry;
        private readonly List<DebugText> _debugText;

        // Assets
        private SpriteFont _debugFont;

        private const int PointSize = 2; // Diameter (in pixels)
        private Texture2D _pointTexture;

        private TimeSpan _lastTime;

        public bool DebugOverlayVisible { get; set; }
        public bool DrawBounds { get; set; }

        private SpriteBatch SpriteBatch => _drawingService.SpriteBatch;

        public DebugService(DrawingService drawingService)
        {
            _drawingService = drawingService;
            _lastTime = TimeSpan.Zero;

            _debugGeometry = new List<DebugGeometry>();
            _debugText = new List<DebugText>();
        }

        public void LoadContent(ContentManager contentManager)
        {
            _debugFont = contentManager.Load<SpriteFont>("Fonts/OpenSans36");

            _pointTexture = new Texture2D(SpriteBatch.GraphicsDevice, PointSize, PointSize);
            var colorData = new Color[PointSize * PointSize];
            for (var x = 0; x < PointSize; x++)
            {
                for (var y = 0; y < PointSize; y++)
                {
                    var relative = new Vector2(x - (float)PointSize / 2, y - (float)PointSize / 2);
                    colorData[x * PointSize + y] = relative.Length() <= (float)PointSize / 2
                       ? Color.White
                       : Color.Transparent;
                }
            }
            _pointTexture.SetData(colorData);
        }

        public void ClearGeometry()
        {
            _debugGeometry.Clear();
        }

        public void MarkPoint(Vector2 point, Color color, TimeSpan lifetime = default)
        {
            if (lifetime == default) lifetime = TimeSpan.FromDays(999);
            _debugGeometry.Add(new DebugPoint(point, color, _lastTime + lifetime));
        }

        public void MarkPoint(Vector2 point, Color color, double lifetime)
        {
            MarkPoint(point, color, TimeSpan.FromSeconds(lifetime));
        }

        public void MarkLine(Vector2 point1, Vector2 point2, Color color, TimeSpan lifetime = default)
        {
            if (lifetime == default) lifetime = TimeSpan.FromDays(999);
            _debugGeometry.Add(new DebugLine(point1, point2, color, _lastTime + lifetime));
        }

        public void MarkLine(Vector2 point1, Vector2 point2, Color color, double lifetime)
        {
            MarkLine(point1, point2, color, TimeSpan.FromSeconds(lifetime));
        }

        public void MarkPolygon(Vector2[] points, Color color, TimeSpan lifetime = default)
        {
            for (var i = 0; i < points.Length; i++)
            {
                MarkLine(points[i], points[(i + 1) % points.Length], color, lifetime);
            }
        }

        public DebugText CreateDebugText(string message = null)
        {
            var text = new DebugText() { Message = message };
            _debugText.Add(text);
            return text;
        }

        public void RemoveDebugText(DebugText text)
        {
            _debugText.Remove(text);
        }

        public void Draw(GameTime time)
        {
            if (!DebugOverlayVisible) { return; }

            var rasterState = new RasterizerState();
            rasterState.MultiSampleAntiAlias = true;

            // TODO This is a bit hacky - ideally we'd let the Drawing handle the camera matrix and spriteBatch state
            SpriteBatch.Begin(rasterizerState: rasterState, sortMode: SpriteSortMode.BackToFront, blendState: BlendState.AlphaBlend);

            foreach (var geometry in _debugGeometry)
            {
                // Handle expired geometry (can't modify collection while in loop, so defer removal)
                if (time.TotalGameTime > geometry.ExpireTime)
                {
                    continue;
                }

                // Draw geometry
                switch (geometry)
                {
                    case DebugPoint point:
                        SpriteBatch.Draw(_pointTexture, point.Point, null, point.Color,
                           0, new Vector2(PointSize) / 2, Vector2.One * 3, SpriteEffects.None, 0);
                        break;

                    case DebugLine line:
                        _drawingService.DrawLine(line.Point1, line.Point2, line.Color, 1);
                        break;
                }
            }

            // Cleanup expired
            // _debugGeometry.RemoveAll(g => g.ExpireTime < time.TotalGameTime);

            // DEBUG
            _debugGeometry.Clear();

            SpriteBatch.End();

            // Draw text in a new batch, since we don't want the camera transform
            SpriteBatch.Begin(rasterizerState: rasterState);

            var debugText = string.Join(Environment.NewLine,
               _debugText.Where(t => !string.IsNullOrEmpty(t.Message))
                         .Select(t => t.Message));

            SpriteBatch.DrawString(_debugFont, debugText, new Vector2(50), Color.White);

            SpriteBatch.End();

            _lastTime = time.TotalGameTime;
        }

        private class DebugGeometry
        {
            public DebugGeometry(Color color, TimeSpan expireTime)
            {
                Color = color;
                ExpireTime = expireTime;
            }

            public Color Color { get; set; }

            public TimeSpan ExpireTime { get; set; }
        }

        private class DebugPoint : DebugGeometry
        {
            public DebugPoint(Vector2 point, Color color, TimeSpan expireTime)
               : base(color, expireTime)
            {
                Point = point;
            }

            public Vector2 Point { get; set; }
        }

        private class DebugLine : DebugGeometry
        {
            public DebugLine(Vector2 point1, Vector2 point2, Color color, TimeSpan expireTime) : base(color, expireTime)
            {
                Point1 = point1;
                Point2 = point2;
            }

            public Vector2 Point1 { get; set; }
            public Vector2 Point2 { get; set; }
        }
    }

    public class DebugText
    {
        public string Message { get; set; }
    }
}