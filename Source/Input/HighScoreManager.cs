using System;
using System.IO;
using System.IO.IsolatedStorage;
using Newtonsoft.Json;

namespace SpaceMarines_TD.Source.Input
{
    class HighScoreManager
    {
        private const string BindingFileName = @"HighScores.json";

        public event EventHandler SettingsChanged;

        public HighScores LeaderBoard { get; set; }

        public void Load()
        {
            var storage = IsolatedStorageFile.GetUserStoreForApplication();
            if (storage.FileExists(BindingFileName))
            {
                using (var file = new StreamReader(storage.OpenFile(BindingFileName, FileMode.Open)))
                {
                    var text = file.ReadToEnd();
                    LeaderBoard = JsonConvert.DeserializeObject<HighScores>(text);
                }
            }
            else
            {
                LeaderBoard = HighScores.Default;
            }

            SettingsChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Store()
        {
            var storage = IsolatedStorageFile.GetUserStoreForApplication();
            var text = JsonConvert.SerializeObject(LeaderBoard);

            using (var file = new StreamWriter(storage.CreateFile(BindingFileName)))
            {
                file.Write(text);
            }

            SettingsChanged?.Invoke(this, EventArgs.Empty);
        }

        public void SubmitScore(int score)
        {
            bool scoreAccepted = false;
            int displacedScore = 0;
            for (int i = 0; i < LeaderBoard.Scores.Length; i++)
            {
                if (!scoreAccepted && score > LeaderBoard.Scores[i])
                {
                    displacedScore = LeaderBoard.Scores[i];
                    LeaderBoard.Scores[i] = score;
                    scoreAccepted = true;
                }

                if (scoreAccepted && displacedScore > LeaderBoard.Scores[i])
                {
                    (LeaderBoard.Scores[i], displacedScore) = (displacedScore, LeaderBoard.Scores[i]);
                }
            }
            Store();
        }
    }
}
