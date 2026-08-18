using System;
using UnityEngine;

namespace Tools.Img
{
    [Serializable]
    public class ImageState
    {
        public Sprite sprite;
        public bool ignoreSprite;
        public Color color;
        public bool ignoreColor;
        public string stateTag;
    }
}