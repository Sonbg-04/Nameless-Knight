using UnityEngine;

namespace Sonn.Nameless_Knight
{
    public static class Pref
    {
        public static bool GetBool(string key, bool defaultValue = false)
        {
            return PlayerPrefs.HasKey(key)
                 ? PlayerPrefs.GetInt(key) == 1 ? true : false
                 : defaultValue;
        }
        public static void SetBool(string key, bool value)
        {
            PlayerPrefs.SetInt(key, value ? 1 : 0);
        }
    }
}
