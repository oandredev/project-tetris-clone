namespace Features.Data
{
    public static class SettingsService
    {
        public static bool GetAudioEnabled()
        {
            return SaveSystem.Load().audioEnabled;
        }

        public static void SetAudioEnabled(bool enabled)
        {
            SaveData data = SaveSystem.Load();
            data.audioEnabled = enabled;
            SaveSystem.Save(data);
        }
    }
}