using UnityEngine;
using UnityEngine.Serialization;

namespace SilkTouch.Managers.Data
{
    using Localization;

    public abstract class BaseStaticData : ScriptableObject, IStaticData
    {
        [SerializeField] protected string dynamicDataLocation = "dynamicData";
        [FormerlySerializedAs("defaultGameData")]
        [SerializeField] protected BaseGameDataScriptableWrapper defaultBaseGameData;
        [SerializeField] protected LocalizationData localizationData;
        [SerializeField] protected BaseGameBalanceData gameBalanceData;

        public string DynamicDataLocation => dynamicDataLocation;
        public BaseGameDataScriptableWrapper DefaultBaseGameData => defaultBaseGameData;
        public LocalizationData LocalizationData => localizationData;
        public BaseGameBalanceData Balance => gameBalanceData;
    }

}