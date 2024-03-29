using System;
using System.Collections.Generic;
using System.Text;
using SpaceMarines_TD.Source.Input;
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
        public int Wave { get; set; }

        public bool isLevelActive { get; set; }
        public bool isGameOver { get; set; }

        public const int NumOfWaves = 5;

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

            isGameOver = false;
            isLevelActive = false;
            Level = 0;
            Wave = 0;
            Lives = 20;
            Money = 60;
            Score = 0;
        }

        public void SubtractLives(int lives)
        {
            Lives = Math.Max(0, Lives - lives);

            if (Lives == 0)
            {
                isGameOver = true;
                isLevelActive = false;
            }
        }
    }
}
