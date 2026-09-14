#if UI_TMP
using System;
using UnityEngine;
using UnityEngine.UI;

namespace SilkTouch.Localization
{
    using Tools;
    using Managers;
    using Managers.Data;

    [RequireComponent(typeof(Toggle))]
    public class LanguageToggle : MonoBehaviour
    {
#pragma warning disable
        [SerializeField] private GT.Language language = GT.Language.English;
        [SerializeField] private Toggle toggle;
#pragma warning restore

        public bool IsOn
        {
            get => toggle.isOn;
            set => toggle.isOn = value;
        }

        public GT.Language Language => language;

        public event Action<GT.Language> OnChoose;

        private void OnValidate()
        {
            toggle = GetComponent<Toggle>();
        }

        private void Start()
        {
            toggle.onValueChanged.AddListener(x =>
            {
                if (x)
                {
                    OnChoose?.Invoke(language);
                }
            });
        }
        public void Refresh()
        {
            toggle.SetIsOnWithoutNotify(Services.GetManager<DataManager>().DynamicData.Settings.CurrentLanguage == language);
        }
    }
}
#endif