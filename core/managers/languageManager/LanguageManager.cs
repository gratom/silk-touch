using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace SilkTouch.Localization
{
    using Tools;
    using Managers;
    using Managers.Data;

    public class LanguageManager : BaseManager
    {
        public override Type ManagerType => typeof(LanguageManager);
        public override bool IsFunctional => true;

        private static SettingsData settingsData => Services.GetManager<IDataManager>().DynamicData.Settings;
        
        private static Dictionary<SystemLanguage, Action> languageAutosetActionsDictionary = new Dictionary<SystemLanguage, Action>()
        {
            { SystemLanguage.Russian, () => settingsData.CurrentLanguage = GT.Language.Russian },
            { SystemLanguage.Belarusian, () => settingsData.CurrentLanguage = GT.Language.Russian },
            { SystemLanguage.Ukrainian, () => settingsData.CurrentLanguage = GT.Language.Ukrainian },
            { SystemLanguage.French, () => settingsData.CurrentLanguage = GT.Language.French },
            { SystemLanguage.Portuguese, () => settingsData.CurrentLanguage = GT.Language.Portuguese },
            { SystemLanguage.German, () => settingsData.CurrentLanguage = GT.Language.German },
            { SystemLanguage.Arabic, () => settingsData.CurrentLanguage = GT.Language.Arabic },
            { SystemLanguage.Danish, () => settingsData.CurrentLanguage = GT.Language.Danish },
            { SystemLanguage.Finnish, () => settingsData.CurrentLanguage = GT.Language.Finnish },
            { SystemLanguage.Dutch, () => settingsData.CurrentLanguage = GT.Language.Dutch },
            { SystemLanguage.Greek, () => settingsData.CurrentLanguage = GT.Language.Greek },
            { SystemLanguage.Italian, () => settingsData.CurrentLanguage = GT.Language.Italian },
            { SystemLanguage.Japanese, () => settingsData.CurrentLanguage = GT.Language.Japanese },
            { SystemLanguage.Korean, () => settingsData.CurrentLanguage = GT.Language.Korean },
            { SystemLanguage.Norwegian, () => settingsData.CurrentLanguage = GT.Language.Norwegian },
            { SystemLanguage.Polish, () => settingsData.CurrentLanguage = GT.Language.Polish },
            { SystemLanguage.Spanish, () => settingsData.CurrentLanguage = GT.Language.Spanish },
            { SystemLanguage.Swedish, () => settingsData.CurrentLanguage = GT.Language.Swedish },
            { SystemLanguage.Turkish, () => settingsData.CurrentLanguage = GT.Language.Turkish },
            { SystemLanguage.Chinese, () => settingsData.CurrentLanguage = GT.Language.ChineseSimplified },
            { SystemLanguage.ChineseSimplified, () => settingsData.CurrentLanguage = GT.Language.ChineseSimplified },
            { SystemLanguage.ChineseTraditional, () => settingsData.CurrentLanguage = GT.Language.ChineseSimplified }
        };

#pragma warning disable

        protected override async Task<bool> OnInit()
        {
            return true;
        }

#pragma warning restore

        public void RefreshLanguages()
        {
            if (Services.GetManager<IDataManager>().DynamicData.Settings.IsFirstLangLoad)
            {
                Services.GetManager<IDataManager>().DynamicData.Settings.IsFirstLangLoad = false;
                if (languageAutosetActionsDictionary.ContainsKey(Application.systemLanguage))
                {
                    languageAutosetActionsDictionary[Application.systemLanguage]();
                    return;
                }
                Services.GetManager<IDataManager>().DynamicData.Settings.CurrentLanguage = GT.Language.English;
            }
        }

        public string GetTextByID(string key)
        {
            return Services.GetManager<IDataManager>().StaticData.LocalizationData.GetTextByID(key);
        }
    }

    public static class QuickLocalizationExtension
    {
        private static Lazy<LanguageManager> lm = new Lazy<LanguageManager>(Services.GetManager<LanguageManager>);
        public static string KeyToLang(this string key)
        {
            return lm.Value.GetTextByID(key);
        }
    }

}