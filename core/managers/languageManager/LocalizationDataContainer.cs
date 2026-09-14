using System;
using System.Collections.Generic;
using System.Linq;
using SilkTouch.Tools;
using UnityEngine;

namespace SilkTouch.Localization
{
    [Serializable]
    public class LocalizationDataContainer
    {
        [SerializeField] private string key;
        [SerializeField] private List<LanguageContentContainer> languageContainers = new List<LanguageContentContainer>();

        public string Key
        {
            get => key;
#if UNITY_EDITOR
            set => key = value;
#endif
        }

#if UNITY_EDITOR
        public List<LanguageContentContainer> LanguageContainers
        {
            get => languageContainers;
            set => languageContainers = value;
        }
#endif
        public string GetTextByLanguage(GT.Language language)
        {
            LanguageContentContainer cont = languageContainers.FirstOrDefault(container => container.Language == language);
            return cont != null ? cont.Text : languageContainers.FirstOrDefault(container => container.Language == GT.Language.English)?.Text;
        }
#if UNITY_EDITOR
        public void TranslateAll()
        {
            for (int i = 1; i < languageContainers.Count; i++)
            {
                languageContainers[i].Text = GT.Translate(languageContainers[0].Text, languageContainers[0].Language, languageContainers[i].Language);
            }
        }
#endif
    }
}