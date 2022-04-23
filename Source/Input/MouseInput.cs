using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SpaceMarines_TD.Source.Input
{
    class MouseInput : IInputDevice
    {
        private Dictionary<MouseEvent, CommandEntry> m_commandEntries = new Dictionary<MouseEvent, CommandEntry>();
        private MouseState m_mousePreviousState = Mouse.GetState();

        public bool Clicked { get; private set; }
        public bool MouseDown { get; private set; }
        public Vector2 Position { get; private set; }

        public void RegisterCommand(MouseEvent evt, InputDeviceHelper.CommandDelegatePosition callback)
        {
            if (m_commandEntries.ContainsKey(evt))
            {
                m_commandEntries.Remove(evt);
            }
            m_commandEntries.Add(evt, new CommandEntry(evt, callback));
        }

        public void Update(GameTime gameTime)
        {
            var state = Mouse.GetState();

            Clicked = state.LeftButton == ButtonState.Pressed && !MouseDown;
            MouseDown = state.LeftButton == ButtonState.Pressed;
            Position = new Vector2(state.X, state.Y);

            foreach (var entry in m_commandEntries.Values)
            {
                // Transitioning from mouse up to mouse down
                if (entry.evt == MouseEvent.MouseDown && state.LeftButton == ButtonState.Pressed && !MouseDown)
                {
                    entry.callback(gameTime, state.X, state.Y);
                }
                // Transitioning from mouse down to mouse up
                if (entry.evt == MouseEvent.MouseUp && state.LeftButton == ButtonState.Released && MouseDown)
                {
                    entry.callback(gameTime, state.X, state.Y);
                }
                if (entry.evt == MouseEvent.MouseMove)
                {
                    if (state.X != m_mousePreviousState.X || state.Y != m_mousePreviousState.Y)
                    {
                        entry.callback(gameTime, state.X, state.Y);
                    }
                }
            }

            MouseDown = (state.LeftButton == ButtonState.Pressed);
            m_mousePreviousState = state;
        }

        private struct CommandEntry
        {
            public CommandEntry(MouseEvent evt, InputDeviceHelper.CommandDelegatePosition callback)
            {
                this.evt = evt;
                this.callback = callback;
            }

            public MouseEvent evt;
            public InputDeviceHelper.CommandDelegatePosition callback;
        }

    }

    public enum MouseEvent
    {
        MouseDown,
        MouseUp,
        MouseMove
    }
}
