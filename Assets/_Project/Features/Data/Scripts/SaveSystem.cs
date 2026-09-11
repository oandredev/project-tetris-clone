using System.IO;
using UnityEngine;

namespace Features.Data
{
    public static class SaveSystem
    {
        private static readonly string FilePath = Path.Combine(Application.persistentDataPath, "progress_tetris.json");

        public static SaveData Load()
        {
            if (!File.Exists(FilePath))
                return new SaveData();

            string json = File.ReadAllText(FilePath);
            return JsonUtility.FromJson<SaveData>(json);
        }

        public static void Save(SaveData data)
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(FilePath, json);
        }
    }
}