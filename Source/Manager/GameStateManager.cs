using System;
using System.Collections.Generic;
using System.Text;
using SpaceMarines_TD.Source.Objects;

namespace SpaceMarines_TD.Source.Manager
{
    public class GameStateManager
    {
        // Player / game state.
        public int Level { get; set; }
        public int Lives { get; set; }
        public int Money { get; set; }
        public int Score { get; set; }


        // Game objects.
        public List<Creep> Creeps { get; }
        public  List<Projectile> Projectiles { get; }
        public List<Tower> Towers { get; }

        public GameStateManager()
        {
            Creeps = new List<Creep>();
            Projectiles = new List<Projectile>();
            Towers = new List<Tower>();

            Reset();
        }

        public void Reset()
        {
            Creeps.Clear();
            Projectiles.Clear();
            Towers.Clear();

            Level = 1;
            Lives = 20;
            Money = 500;
            Score = 0;
        }

        public void SubtractLives(int lives)
        {
            Lives = Math.Max(0, Lives - lives);

            if (Lives == 0)
            {
                // TODO Die
            }
        }
    }
}
