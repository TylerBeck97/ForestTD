using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;

namespace SpaceMarines_TD.Source.Input
{
    class KeyboardInput : IInputDevice
    {
        private KeyboardState m_statePrevious;

        private Dictionary<Keys, CommandEntry> m_commandEntries = new Dictionary<Keys, CommandEntry>();

        public void registerCommand(Keys key, bool keyPressOnly, InputDeviceHelper.CommandDelegate callback)
        {
            m_commandEntries[key] = new CommandEntry(key, keyPressOnly, callback);
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();
            foreach (CommandEntry entry in m_commandEntries.Values)
            {
                if (entry.keyPressOnly && keyPressed(entry.key))
                {
                    entry.callback(gameTime, 1.0f);
                }
                else if (!entry.keyPressOnly && state.IsKeyDown(entry.key))
                {
                    entry.callback(gameTime, 1.0f);
                }
            }

            //
            // Move the current state to the previous state for the next time around
            m_statePrevious = state;
        }

        internal void ClearCommands()
        {
            m_commandEntries.Clear();
        }

        private bool keyPressed(Keys key)
        {
            return (Keyboard.GetState().IsKeyDown(key) && !m_statePrevious.IsKeyDown(key));
        }

        private struct CommandEntry
        {
            public CommandEntry(Keys key, bool keyPressOnly, InputDeviceHelper.CommandDelegate callback)
            {
                this.key = key;
                this.keyPressOnly = keyPressOnly;
                this.callback = callback;
            }

            public Keys key;
            public bool keyPressOnly;
            public InputDeviceHelper.CommandDelegate callback;
        }
    }
}
