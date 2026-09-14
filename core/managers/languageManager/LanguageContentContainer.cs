using System;
using UnityEngine;

namespace SilkTouch.Localization
{
    using Tools;
    
    [Serializable]
    public class LanguageContentContainer
    {
        [SerializeField] private GT.Language language;
        [SerializeField] private string text;

        public GT.Language Language => language;
        public string Text
        {
            get => text;
#if UNITY_EDITOR
            set => text = value;
#endif
        }

        public LanguageContentContainer(GT.Language language, string text)
        {
            this.language = language;
            this.text = text;
        }

        public static implicit operator string(LanguageContentContainer origin)
        {
            return origin.text;
        }

        public override string ToString()
        {
            return text;
        }
    }
}