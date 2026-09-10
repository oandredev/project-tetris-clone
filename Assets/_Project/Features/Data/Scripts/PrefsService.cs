using UnityEngine;
using System.Collections.Generic;

namespace Features.Data
{
    public static class PrefsService
    {
        public static void SetInt(string key, int value) => PlayerPrefs.SetInt(key, value);
        public static int GetInt(string key, int defaultValue = 0) => PlayerPrefs.GetInt(key, defaultValue);

        public static void SetFloat(string key, float value) => PlayerPrefs.SetFloat(key, value);
        public static float GetFloat(string key, float defaultValue = 0f) => PlayerPrefs.GetFloat(key, defaultValue);

        public static void SetString(string key, string value) => PlayerPrefs.SetString(key, value);
        public static string GetString(string key, string defaultValue = "") => PlayerPrefs.GetString(key, defaultValue);

        public static void SetBool(string key, bool value) => PlayerPrefs.SetInt(key, value ? 1 : 0);
        public static bool GetBool(string key, bool defaultValue = false) => PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) == 1;

        //---------------------------------------------------------------------------

        public static void SetObject<T>(string key, T value)
        {
            string json = JsonUtility.ToJson(value);
            PlayerPrefs.SetString(key, json);
        }

        public static T GetObject<T>(string key, T defaultValue = default)
        {
            if (!PlayerPrefs.HasKey(key)) return defaultValue;
            string json = PlayerPrefs.GetString(key);
            return JsonUtility.FromJson<T>(json);
        }

        //---------------------------------------------------------------------------

        [System.Serializable]
        private class ListWrapper<T>
        {
            public List<T> items;
        }

        //---------------------------------------------------------------------------

        public static void SetList<T>(string key, List<T> list)
        {
            var wrapper = new ListWrapper<T> { items = list };
            string json = JsonUtility.ToJson(wrapper);
            PlayerPrefs.SetString(key, json);
        }

        public static List<T> GetList<T>(string key)
        {
            if (!PlayerPrefs.HasKey(key)) return new List<T>();
            string json = PlayerPrefs.GetString(key);
            var wrapper = JsonUtility.FromJson<ListWrapper<T>>(json);
            return wrapper?.items ?? new List<T>();
        }

        //---------------------------------------------------------------------------

        public static bool HasKey(string key) => PlayerPrefs.HasKey(key);
        public static void DeleteKey(string key) => PlayerPrefs.DeleteKey(key);
        public static void Save() => PlayerPrefs.Save();
    }
}