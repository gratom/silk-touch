#if UI_TMP
using UnityEngine;
using UnityEngine.UI;

namespace SilkTouch.Sounds
{
    using Managers;
    
    [RequireComponent(typeof(Button))]
    public class SoundActivator : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private SoundType soundToPlay = SoundType.click;
        [SerializeField] private PlayingSetting playSetting = PlayingSetting.Once;

        [Header("Optional (if null, gets from this GO)")]
        [SerializeField] private Button targetButton;

        private void Start()
        {
            if (targetButton == null)
            {
                targetButton = GetComponent<Button>();
            }

            if (targetButton != null)
            {
                targetButton.onClick.AddListener(PlaySound);
            }
            else
            {
                Debug.LogWarning($"[SoundActivator] No Button found on {gameObject.name}");
            }
        }

        private void OnDestroy()
        {
            if (targetButton != null)
            {
                targetButton.onClick.RemoveListener(PlaySound);
            }
        }

        public void PlaySound()
        {
            SoundManager soundManager = Services.GetManager<SoundManager>();

            if (soundManager != null)
            {
                soundManager.Play(soundToPlay, playSetting);
            }
            else
            {
                Debug.LogError("SoundManager not found in Services!");
            }
        }
    }
}
#endif