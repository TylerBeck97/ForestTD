using Microsoft.Xna.Framework;
namespace SpaceMarines_TD.Source.Input
{
    public interface IInputDevice
    {
        void Update(GameTime gameTime);
    }

    public class InputDeviceHelper
    {
        public delegate void CommandDelegate(GameTime gameTime, float value);
        public delegate void CommandDelegatePosition(GameTime GameTime, int x, int y);
    }
}
