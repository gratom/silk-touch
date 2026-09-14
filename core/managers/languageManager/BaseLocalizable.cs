using System;
using UnityEngine;

namespace SilkTouch.Localization
{
    using Tools;
    using Managers;
    using Managers.Data;

    [Serializable]
    public abstract class BaseLocalizable
    {
        [SerializeField] protected string key = "";

        public void Init()
        {
            Services.GetManager<DataManager>().DynamicData.Settings.OnLanguageChange += OnLanguageChange;
            LanguageChangeAction(Services.GetManager<LanguageManager>().GetTextByID(key));
        }

        public void UnInit()
        {
            Services.GetManager<DataManager>().DynamicData.Settings.OnLanguageChange -= OnLanguageChange;
        }

        private void OnLanguageChange(GT.Language language)
        {
            LanguageChangeAction(Services.GetManager<LanguageManager>().GetTextByID(key));
        }

        protected abstract void LanguageChangeAction(string newValue);

        public string ForceGetTranslation()
        {
            return Services.GetManager<LanguageManager>().GetTextByID(key);
        }
    }
}