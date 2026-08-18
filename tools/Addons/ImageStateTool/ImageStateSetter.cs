using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tools.Img
{
    public interface IImageStateSetter
    {
        void SetState(string stateTag);

        void SetStateDynamic(Sprite sprite);

        void SetStateDynamic(Sprite sprite, Color color);
    }

    [RequireComponent(typeof(Image))]
    public class ImageStateSetter : MonoBehaviour, IImageStateSetter
    {
        private Dictionary<string, ImageState> statesDictionary;

#pragma warning disable

        [SerializeField] private MonoBehaviour mainUserTarget;
        [SerializeField] private List<ImageState> states;
        [SerializeField] private Image image;

#pragma warning restore

        public void SetState(string stateTag)
        {
            statesDictionary.InvokeIfContain(stateTag, state =>
            {
                if (!state.ignoreSprite)
                {
                    image.sprite = statesDictionary[stateTag].sprite;
                }
            });

            statesDictionary.InvokeIfContain(stateTag, state =>
            {
                if (!state.ignoreColor)
                {
                    image.color = statesDictionary[stateTag].color;
                }
            });
        }

        public void SetStateDynamic(Sprite sprite)
        {
            image.sprite = sprite;
            image.color = Color.white;
        }

        public void SetStateDynamic(Sprite sprite, Color color)
        {
            image.sprite = sprite;
            image.color = color;
        }

        private void OnValidate()
        {
            image = GetComponent<Image>();
        }

        private void Awake()
        {
            InitDictionary();
        }

        private void InitDictionary()
        {
            statesDictionary = new Dictionary<string, ImageState>();
            if (states != null && states.Count > 0)
            {
                foreach (ImageState state in states)
                {
                    statesDictionary.Add(state.stateTag, state);
                }
            }
        }
    }
}