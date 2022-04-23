using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceMarines_TD.Source.UI
{
    public class Button
    {
        private ButtonState m_state;

        public event EventHandler<EventArgs> Clicked;

        public Rectangle Bounds { get; set; }
        public Texture2D ClickedTexture { get; set; }
        public SpriteFont Font { get; set; }
        public Texture2D Texture { get; set; }
        public string Text { get; set; }

        public Button(Rectangle bounds, Texture2D texture, Texture2D clickedTexture, SpriteFont font, string text)
        {
            Bounds = bounds;
            ClickedTexture = clickedTexture;
            Font = font;
            Texture = texture;
            Text = text;

            m_state = ButtonState.Normal;
        }

        public bool Update(Vector2 mousePos, bool click, bool mouseDown)
        {
            var isMouseOver = Bounds.Contains(mousePos);

            // Button state machine - we want it to stay pressed even if the mouse moves off so long as the mouse is pressed.
            switch (m_state)
            {
                case ButtonState.Normal:
                    if (isMouseOver && click)
                    {
                        m_state = ButtonState.Pressed;
                        HandleClick();
                    }

                    break;

                case ButtonState.Pressed:
                    if (!mouseDown)
                    {
                        m_state = ButtonState.Normal;
                    }

                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            return isMouseOver || m_state == ButtonState.Pressed;
        }

        private void HandleClick()
        {
            Clicked?.Invoke(this, EventArgs.Empty);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var texture = (m_state == ButtonState.Normal)
                ? Texture
                : ClickedTexture;

            spriteBatch.Draw(texture, Bounds, Color.White);

            var textSize = Font.MeasureString(Text);
            spriteBatch.DrawString(Font, Text, Bounds.Center.ToVector2() - textSize / 2, Color.Black);
        }
    }

    public enum ButtonState
    {
        Normal,
        Pressed
    }
}
