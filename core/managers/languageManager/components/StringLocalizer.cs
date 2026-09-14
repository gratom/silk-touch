using System;
using UnityEngine;

namespace SilkTouch.Localization
{
    [Serializable]
    public class StringLocalizer : BaseLocalizable
    {
        public string Text { get; private set; }

        protected override void LanguageChangeAction(string newValue)
        {
            Text = newValue;
        }

        public static implicit operator string(StringLocalizer s)
        {
            return s.Text;
        }

        public override string ToString()
        {
            return Text;
        }
    }
}