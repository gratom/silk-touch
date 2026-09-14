#if UI_TMP
using UnityEngine;

namespace SilkTouch.Localization
{
    using Tools;
    public class TextLocalizer : BaseMonoLocalizable
    {
#pragma warning disable
        [SerializeField] private TXT text;
#pragma warning restore2

        protected override void LanguageChangeAction(string newValue)
        {
            text.text = newValue;
        }
    }
}
#endif