using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YG;

namespace Shooter.ScriptableObjects
{
    [CreateAssetMenu(fileName = "SaveData", menuName = "CustomObjects/SaveData", order = 1)]
    public class SaveData : ScriptableObject
    {
        public int m_CheckpointNumber = 0;
        public int m_LastUnlockedLevel = 0;
        public int m_GemCount = 0;

        public void Save()
        {
            PlayerPrefs.SetInt("m_CheckpointNumber", m_CheckpointNumber);
            PlayerPrefs.SetInt("m_GemCount", m_GemCount);
            PlayerPrefs.Save();

            // Сохранение m_GemCount в облачное хранилище через PluginYG
            YandexGame.savesData.m_GemCount = m_GemCount;
            YandexGame.SaveProgress();

            // Запись в лидерборд
            YandexGame.NewLeaderboardScores("gemslb", m_GemCount); // Запись в лидерборд gemslb
        }

        public void Load()
        {
            m_CheckpointNumber = PlayerPrefs.GetInt("m_CheckpointNumber", 0);
            m_GemCount = PlayerPrefs.GetInt("m_GemCount", 0);
        }
    }
}
