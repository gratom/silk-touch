using UnityEngine;

namespace SilkTouch.Managers.Data
{
    using Localization;

    [CreateAssetMenu(fileName = "StaticData", menuName = "Scriptables/Static data", order = 51)]
    public class StaticData : ScriptableObject
    {
        [SerializeField] private string dynamicDataLocation = "dynamicData";
        [SerializeField] private GameDataScriptableWrapper defaultGameData;
        [SerializeField] private LocalizationData localizationData;
        [SerializeField] private GameBalanceData gameBalanceData;

        public string DynamicDataLocation => dynamicDataLocation;
        public GameDataScriptableWrapper DefaultGameData => defaultGameData;
        public LocalizationData LocalizationData => localizationData;
        public GameBalanceData Balance => gameBalanceData;
    }

}