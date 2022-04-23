using System;
using SpaceMarines_TD.Source;

namespace SpaceMarines_TD
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new GameLoop())
                game.Run();
        }
    }
}
