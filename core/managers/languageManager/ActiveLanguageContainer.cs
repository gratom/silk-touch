using System;
using UnityEngine;

namespace SilkTouch.Localization
{
    using Tools;
    
    [Serializable]
    public class ActiveLanguageContainer
    {
        [SerializeField] private GT.Language language;
        [SerializeField] private bool isActive;

        public bool IsActive
        {
            get => isActive;
#if UNITY_EDITOR
            set => isActive = value;
#endif
        }

        public GT.Language Language
        {
            get => language;
#if UNITY_EDITOR
            set => language = value;
#endif
        }
    }
}