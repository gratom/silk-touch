using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace SilkTouch.Sounds
{
    using Managers;
    using Managers.Data;
    
    public class SoundManager : BaseManager
    {
        public override Type ManagerType => typeof(SoundManager);
        public override bool IsFunctional => true;
        public List<SoundSetting> soundSettings;

        private Dictionary<SoundType, AudioClip> soundsDictionary;
        private Dictionary<SoundType, float> volumeDictionary;
        private Dictionary<SoundType, List<AudioSource>> sourcesByType = new Dictionary<SoundType, List<AudioSource>>();
        private AmbientController ambient = new AmbientController();

        protected override async Task<bool> OnInit()
        {
            soundsDictionary = soundSettings.ToDictionary(x => x.type, y => y.clip);
            volumeDictionary = soundSettings.ToDictionary(x => x.type, y => y.volume);
            Tools.Pause.OnPaused += OnPaused;
            Tools.Pause.OnUnpaused += OnUnpaused;
            return true;
        }

        private void OnPaused()
        {
            foreach (List<AudioSource> list in sourcesByType.Values)
            foreach (AudioSource source in list)
            {
                if (source.isPlaying)
                {
                    source.Pause();
                }
            }
            ambient.Pause();
        }

        private void OnUnpaused()
        {
            if (Services.GetManager<DataManager>().DynamicData.Settings.sounds)
            {
                foreach (List<AudioSource> list in sourcesByType.Values)
                foreach (AudioSource source in list)
                {
                    if (!source.isPlaying)
                    {
                        source.UnPause();
                    }
                }
            }
            if (Services.GetManager<DataManager>().DynamicData.Settings.music)
            {
                ambient.UnPause();
            }
        }

        public void CheckMusic()
        {
            if (Services.GetManager<DataManager>().DynamicData.Settings.music)
            {
                if (!ambient.UnPause())
                {
                    Play(SoundType.musicDay1, PlayingSetting.Ambient);
                }
            }
            else
            {
                ambient.Pause();
            }
        }

        public void Play(SoundType type, PlayingSetting setting = PlayingSetting.Once, bool forceCreateNewInstance = false)
        {
            if (!soundsDictionary.TryGetValue(type, out AudioClip clip) || clip == null)
            {
                return;
            }

            if (setting == PlayingSetting.Ambient)
            {
                if (Services.GetManager<DataManager>().DynamicData.Settings.music)
                {
                    ambient.Play(clip, transform, volume: volumeDictionary[type]);
                }
                return;
            }

            if (!sourcesByType.TryGetValue(type, out List<AudioSource> list))
            {
                list = new List<AudioSource>();
                sourcesByType[type] = list;
            }

            if (!forceCreateNewInstance)
            {
                foreach (AudioSource source in list)
                {
                    if (source.clip == clip && source.isPlaying)
                    {
                        return;
                    }
                }
            }

            foreach (AudioSource source in list)
            {
                if (!source.isPlaying)
                {
                    PlaySource(source, clip, setting, volumeDictionary[type]);
                    return;
                }
            }

            AudioSource newSource = CreateNewSource(type);
            list.Add(newSource);
            PlaySource(newSource, clip, setting, volumeDictionary[type]);
        }

        public void StopAll(SoundType type)
        {
            if (!sourcesByType.TryGetValue(type, out List<AudioSource> list))
            {
                return;
            }

            foreach (AudioSource source in list)
            {
                if (source.isPlaying)
                {
                    source.Stop();
                }
            }
        }

        private AudioSource CreateNewSource(SoundType type)
        {
            GameObject go = new GameObject($"Audio_{type}_{sourcesByType[type].Count}");
            go.transform.SetParent(transform);
            AudioSource source = go.AddComponent<AudioSource>();
            return source;
        }

        private void PlaySource(AudioSource source, AudioClip clip, PlayingSetting setting, float volume)
        {
            if (Services.GetManager<DataManager>().DynamicData.Settings.sounds)
            {
                source.clip = clip;
                source.loop = setting == PlayingSetting.Loop;
                source.volume = volume;
                source.Play();
            }
        }

        private class AmbientController
        {
            private readonly GameObject[] slots = new GameObject[2];
            private readonly AudioSource[] sources = new AudioSource[2];
            private int activeIndex = -1;
            private CancellationTokenSource fadeToken;

            public async void Play(AudioClip clip, Transform transform, float fadeDuration = 1f, float volume = 1)
            {
                fadeToken?.Cancel();
                fadeToken = new CancellationTokenSource();
                CancellationToken token = fadeToken.Token;

                int newIndex = (activeIndex + 1) % 2;

                if (sources[newIndex] == null)
                {
                    slots[newIndex] = new GameObject($"AmbientSlot{newIndex}");
                    slots[newIndex].transform.SetParent(transform);
                    sources[newIndex] = slots[newIndex].AddComponent<AudioSource>();
                }

                AudioSource newSource = sources[newIndex];
                newSource.clip = clip;
                newSource.loop = true;
                newSource.volume = 0f;
                newSource.Play();

                if (activeIndex != -1)
                {
                    AudioSource oldSource = sources[activeIndex];
                    _ = FadeOut(oldSource, fadeDuration, token);
                }

                _ = FadeIn(newSource, fadeDuration, token, volume);
                activeIndex = newIndex;
            }

            public void Pause()
            {
                if (activeIndex >= 0)
                {
                    sources[activeIndex]?.Pause();
                }
            }

            public bool UnPause()
            {
                if (activeIndex >= 0 && !sources[activeIndex].isPlaying)
                {
                    sources[activeIndex]?.UnPause();
                    return true;
                }
                return false;
            }

            private async Task FadeIn(AudioSource source, float duration, CancellationToken token, float volume)
            {
                float time = 0f;
                while (time < duration)
                {
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }
                    time += Time.unscaledDeltaTime;
                    source.volume = Mathf.Lerp(0f, volume, time / duration);
                    await Task.Yield();
                }
                source.volume = volume;
            }

            private async Task FadeOut(AudioSource source, float duration, CancellationToken token)
            {
                float start = source.volume;
                float time = 0f;
                while (time < duration)
                {
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }
                    time += Time.unscaledDeltaTime;
                    source.volume = Mathf.Lerp(start, 0f, time / duration);
                    await Task.Yield();
                }
                source.Stop();
            }
        }
    }

    public enum PlayingSetting
    {
        Once,
        Loop,
        Ambient
    }

    [Serializable]
    public class SoundSetting
    {
        public AudioClip clip;
        public SoundType type;
        public float volume;
    }

    public enum SoundType
    {
        none = 0,
        musicDay1 = 1,
        musicNight1 = 11,

        attack = 21,
        attack2 = 22,
        attack3 = 23,

        soundCraft = 31,
        soundForged = 32,
        chicken = 33,
        coal = 34,
        cow = 35,
        diamond = 36,
        enchantment = 37,
        grass = 38,
        metal = 39,
        rock = 40,
        skeleton = 41,
        spider = 42,
        wood = 43,
        zombie = 44,
        trade = 45,

        buySmth = 60,
        click = 61,
        equip = 62,
        error = 63,
        achieve = 64
    }
}