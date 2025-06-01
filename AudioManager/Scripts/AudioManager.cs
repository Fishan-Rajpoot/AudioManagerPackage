using System;
using UnityEngine;
using UnityEngine.Audio;

namespace GameDevFishy.Audio
{
    [Serializable]
    public class Sound
    {
        [HideInInspector]
        public AudioSource source;
        public SoundName name;
        public AudioClip clip;
        public AudioMixerGroup mixer;

        [Range(0, 1)]
        public float volume = 1;
        [Range(0.1f, 3)]
        public float pitch = 1;

        [Space(5)]
        public bool playOnAwake;
        public bool loop;

        public bool CompareName(SoundName name)
        {
            return name == this.name;
        }

        public void Play()
        {
            if (source != null)
            {
                source.Play();
            }
            else
            {
                Debug.LogError(name + ": Audio Source has not assigned");
            }
        }

        public void Pause()
        {
            if (source != null)
            {
                source.Pause();
            }
            else
            {
                Debug.LogError(name + ": Audio Source has not assigned");
            }
        }

        public void Stop()
        {
            if (source != null)
            {
                source.Stop();
            }
            else
            {
                Debug.LogError(name + ": Audio Source has not assigned");
            }
        }

        public void UnPause()
        {
            if (source != null)
            {
                source.UnPause();
            }
            else
            {
                Debug.LogError(name + ": Audio Source has not assigned");
            }
        }

        public bool IsPlaying()
        {
            return source != null && source.isPlaying;
        }

        public void PlayOneShot()
        {
            if (source != null)
            {
                source.PlayOneShot(clip);
            }
            else
            {
                Debug.LogError(name + ": Audio Source has not assigned");
            }
        }
    }

    public class AudioManager : MonoBehaviour
    {
        public static AudioManager instance;
        public AudioMixer AudioMixer;
        public Sound[] sounds;

        #region Unity Methods
        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            foreach (Sound s in sounds)
            {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;
                s.source.pitch = s.pitch;
                s.source.volume = s.volume;
                s.source.loop = s.loop;
                s.source.outputAudioMixerGroup = s.mixer;
            }
        }

        private void Start()
        {
            foreach (Sound s in sounds)
            {
                if (s.playOnAwake)
                {
                    Play(s.name);
                }
            }
        }
        #endregion

        #region Player Methods
        public void Play(SoundName name)
        {
            Sound sound = GetSound(name);
            sound?.Play();
        }

        public void PlayOneShot(SoundName name)
        {
            Sound sound = GetSound(name);
            sound?.PlayOneShot();
        }

        public void Pause(SoundName name)
        {
            GetSound(name)?.Pause();
        }

        public void UnPause(SoundName name)
        {
            GetSound(name)?.UnPause();
        }

        public void Stop(SoundName name)
        {
            Sound sound = GetSound(name);
            sound?.Stop();
        }

        public bool IsSoundPlaying(SoundName name)
        {
            Sound sound = GetSound(name);
            return sound?.IsPlaying() ?? false;
        }

        public void SetVolume(SoundName name, float volume)
        {
            Sound sound = GetSound(name);
            if (sound != null)
            {
                sound.volume = Mathf.Clamp01(volume);
                sound.source.volume = sound.volume;
            }
        }

        public void SetPitch(SoundName name, float pitch)
        {
            Sound sound = GetSound(name);
            if (sound != null)
            {
                sound.pitch = Mathf.Clamp(pitch, 0.1f, 3f);
                sound.source.pitch = sound.pitch;
            }
        }
        #endregion

        #region Mixer Managers
        /// <summary>
        /// Sets the master volume level on the AudioMixer by converting a linear volume value (0 to 1)
        /// to a decibel scale (-80 dB to 0 dB) and applying it to the "SoundVolume" exposed parameter.
        /// </summary>
        /// <param name="value">
        /// The linear volume value, expected to be in the range [0, 1],
        /// where 0 represents mute and 1 represents full volume.
        /// </param>
        public void SetMasterVolume(float value)
        {
            // Convert linear 0-1 volume to decibel scale (-80 dB to 0 dB)
            float dB = Mathf.Lerp(-80f, 0f, value); // map 0-1 to -80 to 0 dB

            AudioMixer.SetFloat("MasterVolume", dB);
        }



        /// <summary>
        /// Sets the music volume level on the AudioMixer by converting a linear volume value (0 to 1)
        /// to a decibel scale (-80 dB to 0 dB) and applying it to the "MusicVolume" exposed parameter.
        /// </summary>
        /// <param name="value">
        /// The linear volume value, expected to be in the range [0, 1],
        /// where 0 represents mute and 1 represents full volume.
        /// </param>
        public void SetMusicVolume(float value)
        {
            float dB = Mathf.Lerp(-80f, 0f, value);
            AudioMixer.SetFloat("MusicVolume", dB);
        }



        /// <summary>
        /// Sets the sound effects (SFX) volume level on the AudioMixer by converting a linear volume value (0 to 1)
        /// to a decibel scale (-80 dB to 0 dB) and applying it to the "SFXVolume" exposed parameter.
        /// </summary>
        /// <param name="value">
        /// The linear volume value, expected to be in the range [0, 1],
        /// where 0 represents mute and 1 represents full volume.
        /// </param>
        public void SetSFXVolume(float value)
        {
            float dB = Mathf.Lerp(-80f, 0f, value);
            AudioMixer.SetFloat("SFXVolume", dB);
        }


        #endregion

        Sound GetSound(SoundName name)
        {
            foreach (Sound s in sounds)
            {
                if (s.CompareName(name))
                {
                    return s;
                }
            }
            Debug.LogError($"Sound {name} not found!");
            return null;
        }
    }
}