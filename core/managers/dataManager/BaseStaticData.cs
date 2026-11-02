using UnityEngine;

namespace SilkTouch.Managers.Data
{
    using Localization;

    public abstract class BaseStaticData : ScriptableObject, IStaticData
    {
        [SerializeField] protected string dynamicDataLocation = "dynamicData";
        [SerializeField] protected GameDataScriptableWrapper defaultGameData;
        [SerializeField] protected LocalizationData localizationData;
        [SerializeField] protected GameBalanceData gameBalanceData;

        public string DynamicDataLocation => dynamicDataLocation;
        public GameDataScriptableWrapper DefaultGameData => defaultGameData;
        public LocalizationData LocalizationData => localizationData;
        public GameBalanceData Balance => gameBalanceData;
    }

}