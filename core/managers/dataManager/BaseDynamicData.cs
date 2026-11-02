using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace SilkTouch.Managers.Data
{

    [Serializable]
    public abstract class BaseDynamicData : IDynamicData
    {
        [SerializeField][Storage(Storage.StoragePathType.applicationFolder, "saves")]
        protected GameData gameData;

        [SerializeField][Storage(Storage.StoragePathType.playerPrefs, "Settings")]
        protected SettingsData settings = new SettingsData();

        public GameData GameData
        {
            get => gameData;
            set => gameData = value;
        }

        public SettingsData Settings => settings;
    }
}