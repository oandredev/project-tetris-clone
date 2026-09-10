namespace Features.Data
{
    public static class ScoreService
    {
        private const string HighScoreKey = "tetris_highscore";
        private const string HighLinesKey = "tetris_highlines";
        private const string TotalScoreKey = "tetris_totalscore";
        private const string TotalLinesKey = "tetris_totallines";

        public static int GetHighScore() => PrefsService.GetInt(HighScoreKey, 0);
        public static int GetHighLines() => PrefsService.GetInt(HighLinesKey, 0);
        public static int GetTotalScore() => PrefsService.GetInt(TotalScoreKey, 0);
        public static int GetTotalLines() => PrefsService.GetInt(TotalLinesKey, 0);

        public static void SaveRun(int score, int lines)
        {
            if (score > GetHighScore())
                PrefsService.SetInt(HighScoreKey, score);

            if (lines > GetHighLines())
                PrefsService.SetInt(HighLinesKey, lines);

            PrefsService.SetInt(TotalScoreKey, GetTotalScore() + score);
            PrefsService.SetInt(TotalLinesKey, GetTotalLines() + lines);

            PrefsService.Save();
        }
    }
}