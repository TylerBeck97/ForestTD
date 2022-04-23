namespace SpaceMarines_TD.Source.Input
{
    class HighScores
    {
        public int[] Scores { get; set; }

        public static HighScores Default => new HighScores
        {
            Scores = new[] {0, 0, 0, 0, 0}
        };
    }
}
