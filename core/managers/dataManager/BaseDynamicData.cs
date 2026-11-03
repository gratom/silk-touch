using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace SilkTouch.Managers.Data
{

    [Serializable]
    public abstract class BaseDynamicData<TGameData, TSettings> : IDynamicData
        where TGameData : BaseGameData
        where TSettings : BaseSettingsData
    {
        [SerializeField][Storage(Storage.StoragePathType.applicationFolder, "saves")]
        protected TGameData gameData;

        [SerializeField][Storage(Storage.StoragePathType.playerPrefs, "Settings")]
        protected TSettings settings;

        public BaseGameData GameData
        {
            get => gameData;
            set => gameData = (TGameData)value;
        }

        public BaseSettingsData Settings => settings;
    }
}