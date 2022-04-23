using System;
using System.Collections.Generic;
using System.Text;

namespace SpaceMarines_TD.Source.Input
{
    class KeyBindings
    {
        public string Ground { get; set; }
        public string Air { get; set; }
        public string Mixed { get; set; }

        public string Bomb { get; set; }
        public string Upgrade { get; set; }
        public string StartLevel { get; set; }
        public string SellTower { get; set; }

        public static KeyBindings Default => new KeyBindings
        {
            // TODO: Set up default keybindings
            Ground = "G",
            Air = "A",
            Mixed = "M",
            Bomb = "B",
            Upgrade = "U",
            SellTower = "S",
            StartLevel = "Space"
        };
    }
}
