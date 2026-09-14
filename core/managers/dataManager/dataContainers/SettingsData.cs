using System;
using UnityEngine;

namespace SilkTouch.Managers.Data
{
    using Tools;
    
    [Serializable]
    public class SettingsData
    {
        [SerializeField] private GT.Language currentLanguage = GT.Language.English;
        [SerializeField] private bool isFirstLoad = true;

        public GT.Language CurrentLanguage
        {
            get => currentLanguage;
            set
            {
                if (currentLanguage != value)
                {
                    currentLanguage = value;
                    OnLanguageChange?.Invoke(currentLanguage);
                }
            }
        }

        public bool IsFirstLangLoad
        {
            get => isFirstLoad;
            set => isFirstLoad = value;
        }

        public event Action<GT.Language> OnLanguageChange;

        public bool music = true;
        public bool sounds = true;

    }
}