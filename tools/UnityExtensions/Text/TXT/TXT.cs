#if UI_TMP
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Tools
{

    [System.Serializable]
    public class TXT
    {
        [SerializeField] private Text _text;
        [SerializeField] private TMP_Text _tmpText;

        public string text
        {
            get
            {
                if (_text != null)
                {
                    return _text.text;
                }
                if (_tmpText != null)
                {
                    return _tmpText.text;
                }
                throw new System.NullReferenceException("TXT: No Text or TMP_Text component assigned.");
            }
            set
            {
                if (_text != null)
                {
                    _text.text = value;
                }
                else if (_tmpText != null)
                {
                    _tmpText.text = value;
                }
                else
                {
                    throw new System.NullReferenceException("TXT: No Text or TMP_Text component assigned.");
                }
            }
        }

        public Color color
        {
            get
            {
                if (_text != null)
                {
                    return _text.color;
                }
                if (_tmpText != null)
                {
                    return _tmpText.color;
                }
                throw new System.NullReferenceException("TXT: No Text or TMP_Text component assigned.");
            }
            set
            {
                if (_text != null)
                {
                    _text.color = value;
                }
                else if (_tmpText != null)
                {
                    _tmpText.color = value;
                }
                else
                {
                    throw new System.NullReferenceException("TXT: No Text or TMP_Text component assigned.");
                }
            }
        }

        public float fontSize
        {
            get
            {
                if (_text != null)
                {
                    return _text.fontSize;
                }
                if (_tmpText != null)
                {
                    return _tmpText.fontSize;
                }
                throw new System.NullReferenceException("TXT: No Text or TMP_Text component assigned.");
            }
            set
            {
                if (_text != null)
                {
                    _text.fontSize = (int)value;
                }
                else if (_tmpText != null)
                {
                    _tmpText.fontSize = value;
                }
                else
                {
                    throw new System.NullReferenceException("TXT: No Text or TMP_Text component assigned.");
                }
            }
        }

        public bool IsAssigned => _text != null || _tmpText != null;

        public Text TextComponent => _text;
        public TMP_Text TMPComponent => _tmpText;

        public GameObject gameObject => TextComponent?.gameObject ?? TMPComponent?.gameObject;

        public bool TryInitFromGO(GameObject target, bool force = false)
        {
            if (target == null)
            {
                return false;
            }

            if (!force && IsAssigned)
            {
                return false;
            }

            Text textComp = target.GetComponent<Text>();
            TMP_Text tmpComp = target.GetComponent<TMP_Text>();

            if (textComp != null || tmpComp != null)
            {
                _text = textComp;
                _tmpText = tmpComp;
                return true;
            }

            return false;
        }
    }
}
#endif