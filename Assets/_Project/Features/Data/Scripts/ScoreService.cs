namespace Features.Data
{
    public static class ScoreService
    {
        private static SaveData cachedData;

        private static SaveData Data => cachedData ??= SaveSystem.Load();

        public static int GetHighScore() => Data.highScore;
        public static int GetHighLines() => Data.highLines;
        public static int GetTotalScore() => Data.totalScore;
        public static int GetTotalLines() => Data.totalLines;

        public static void SaveRun(int score, int lines)
        {
            if (score > Data.highScore)
                Data.highScore = score;

            if (lines > Data.highLines)
                Data.highLines = lines;

            Data.totalScore += score;
            Data.totalLines += lines;

            SaveSystem.Save(Data);
        }
    }
}