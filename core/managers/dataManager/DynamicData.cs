using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace SilkTouch.Managers.Data
{
    [Serializable]
    public class DynamicData
    {
        [SerializeField][Storage(Storage.StoragePathType.applicationFolder, "saves")]
        private GameData gameData;

        [SerializeField][Storage(Storage.StoragePathType.playerPrefs, "Settings")]
        private SettingsData settings = new SettingsData();

        public GameData GameData
        {
            get => gameData;
            set => gameData = value;
        }

        public SettingsData Settings => settings;
    }
}