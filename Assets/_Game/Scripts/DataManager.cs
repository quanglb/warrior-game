using UnityEngine;

namespace _Game.Scripts
{
    public static class DataManager
    {
        public static int Level
        {
            get => PlayerPrefs.GetInt(DataKey.PlayerLevel.ToString(), 1);
            set
            {
                PlayerPrefs.SetInt(DataKey.PlayerLevel.ToString(), value);
                PlayerPrefs.Save();
            }
        }

        public static int TowerLevel
        {
            get => PlayerPrefs.GetInt(DataKey.TowerLevel.ToString(), 1);
            set
            {
                PlayerPrefs.GetInt(DataKey.TowerLevel.ToString(), value);
                PlayerPrefs.Save();
            }
        }

        public static float MeatRegenSpeed
        {
            get => PlayerPrefs.GetFloat(DataKey.MeatRegenSpeed.ToString(), 0.2f);
            set
            {
                PlayerPrefs.SetFloat(DataKey.MeatRegenSpeed.ToString(), value);
                PlayerPrefs.Save();
            }
        }
    }

    public enum DataKey
    {
        PlayerLevel,
        TowerLevel,
        MeatRegenSpeed
    }
}