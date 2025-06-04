using System;
using UnityEngine;
using UnityEngine.Audio;

namespace GameDevFishy.Audio
{
    /// <summary>
    /// Represents a single sound entry with settings for playback, volume, pitch, and loop.
    /// </summary>
    [Serializable]
    public class Sound
    {
        /// <summary>
        /// The AudioSource component used to play this sound at runtime. Automatically assigned by AudioManager.
        /// </summary>
        [HideInInspector]
        public AudioSource source;

        /// <summary>
        /// The enum identifier for this sound. Used to look up and play the correct AudioClip.
        /// </summary>
        public SoundName name;

        /// <summary>
        /// The actual AudioClip asset that will be played by this Sound.
        /// </summary>
        public AudioClip clip;

        /// <summary>
        /// The AudioMixerGroup this sound should be routed through.
        /// </summary>
        public AudioMixerGroup mixer;

        /// <summary>
        /// The playback volume (0 to 1). Default is 1 (full volume).
        /// </summary>
        [Range(0, 1)]
        public float volume = 1;

        /// <summary>
        /// The playback pitch multiplier (0.1 to 3). Default is 1 (original pitch).
        /// </summary>
        [Range(0.1f, 3)]
        public float pitch = 1;

        [Space(5)]
        /// <summary>
        /// If true, this sound will play automatically on Start (after Awake).
        /// </summary>
        public bool playOnAwake;

        /// <summary>
        /// If true, the sound will loop when played.
        /// </summary>
        public bool loop;

        /// <summary>
        /// Checks if the provided SoundName matches this sound's enum name.
        /// </summary>
        /// <param name="name">The SoundName to compare.</param>
        /// <returns>True if names match; otherwise false.</returns>
        public bool CompareName(SoundName name)
        {
            return name == this.name;
        }

        /// <summary>
        /// Plays the sound using its configured AudioSource. Logs an error if the AudioSource is missing.
        /// </summary>
        public void Play()
        {
            if (source != null)
            {
                source.Play();
            }
            else
            {
                Debug.LogError($"{name}: Audio Source has not been assigned.");
            }
        }

        /// <summary>
        /// Pauses playback of this sound. Logs an error if the AudioSource is missing.
        /// </summary>
        public void Pause()
        {
            if (source != null)
            {
                source.Pause();
            }
            else
            {
                Debug.LogError($"{name}: Audio Source has not been assigned.");
            }
        }

        /// <summary>
        /// Stops playback of this sound. Logs an error if the AudioSource is missing.
        /// </summary>
        public void Stop()
        {
            if (source != null)
            {
                source.Stop();
            }
            else
            {
                Debug.LogError($"{name}: Audio Source has not been assigned.");
            }
        }

        /// <summary>
        /// Unpauses playback of this sound if it was previously paused. Logs an error if the AudioSource is missing.
        /// </summary>
        public void UnPause()
        {
            if (source != null)
            {
                source.UnPause();
            }
            else
            {
                Debug.LogError($"{name}: Audio Source has not been assigned.");
            }
        }

        /// <summary>
        /// Checks whether this sound is currently playing.
        /// </summary>
        /// <returns>True if the AudioSource exists and is playing; otherwise false.</returns>
        public bool IsPlaying()
        {
            return source != null && source.isPlaying;
        }

        /// <summary>
        /// Plays the sound once (OneShot). Logs an error if the AudioSource is missing.
        /// </summary>
        public void PlayOneShot()
        {
            if (source != null)
            {
                source.PlayOneShot(clip);
            }
            else
            {
                Debug.LogError($"{name}: Audio Source has not been assigned.");
            }
        }
    }

    /// <summary>
    /// Manages all Sound objects in the scene, ensures a single instance, and handles playback requests.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        /// <summary>
        /// Singleton instance of the AudioManager. Access via AudioManager.instance.
        /// </summary>
        public static AudioManager instance;

        /// <summary>
        /// Reference to the master AudioMixer. Allows runtime control of mixer parameters (volumes, groups, etc.).
        /// </summary>
        public AudioMixer AudioMixer;

        /// <summary>
        /// Array of Sound objects. Each element corresponds to one SoundName enum entry.
        /// </summary>
        public Sound[] sounds;

        #region Unity Methods

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// Initializes the singleton, sets up AudioSource components for each Sound, and applies initial settings.
        /// </summary>
        private void Awake()
        {
            // If no instance exists yet, assign this as the singleton
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                // If another instance already exists, destroy this duplicate
                Destroy(gameObject);
                return;
            }

            // For each Sound entry, add an AudioSource component and configure it
            foreach (Sound s in sounds)
            {
                // Create a new AudioSource on this GameObject
                s.source = gameObject.AddComponent<AudioSource>();

                // Assign the AudioClip to the AudioSource
                s.source.clip = s.clip;

                // Apply pitch and volume settings
                s.source.pitch = s.pitch;
                s.source.volume = s.volume;

                // Apply loop setting
                s.source.loop = s.loop;

                // Route the AudioSource through the specified AudioMixerGroup (if assigned)
                s.source.outputAudioMixerGroup = s.mixer;
            }
        }

        /// <summary>
        /// Start is called before the first frame update, after Awake.
        /// Automatically plays any sounds marked playOnAwake.
        /// </summary>
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

        #region Playback Methods

        /// <summary>
        /// Plays the sound corresponding to the given SoundName.
        /// </summary>
        /// <param name="name">The enum identifier of the sound to play.</param>
        public void Play(SoundName name)
        {
            Sound sound = GetSound(name);
            if (sound != null)
            {
                sound.Play();
            }
        }

        /// <summary>
        /// Plays the sound once (OneShot) without interrupting any currently playing instance.
        /// </summary>
        /// <param name="name">The enum identifier of the sound to play OneShot.</param>
        public void PlayOneShot(SoundName name)
        {
            Sound sound = GetSound(name);
            if (sound != null)
            {
                sound.PlayOneShot();
            }
        }

        /// <summary>
        /// Pauses the specified sound.
        /// </summary>
        /// <param name="name">The enum identifier of the sound to pause.</param>
        public void Pause(SoundName name)
        {
            Sound sound = GetSound(name);
            sound?.Pause();
        }

        /// <summary>
        /// Unpauses the specified sound if it was paused.
        /// </summary>
        /// <param name="name">The enum identifier of the sound to unpause.</param>
        public void UnPause(SoundName name)
        {
            Sound sound = GetSound(name);
            sound?.UnPause();
        }

        /// <summary>
        /// Stops playback of the specified sound.
        /// </summary>
        /// <param name="name">The enum identifier of the sound to stop.</param>
        public void Stop(SoundName name)
        {
            Sound sound = GetSound(name);
            sound?.Stop();
        }

        /// <summary>
        /// Returns whether the specified sound is currently playing.
        /// </summary>
        /// <param name="name">The enum identifier of the sound to check.</param>
        /// <returns>True if the sound is playing; false otherwise.</returns>
        public bool IsSoundPlaying(SoundName name)
        {
            Sound sound = GetSound(name);
            return sound?.IsPlaying() ?? false;
        }

        /// <summary>
        /// Adjusts the volume of the specified sound (both in the Sound data and its AudioSource).
        /// Clamps volume between 0 and 1.
        /// </summary>
        /// <param name="name">The enum identifier of the sound to adjust.</param>
        /// <param name="volume">Linear volume value (0.0 to 1.0).</param>
        public void SetVolume(SoundName name, float volume)
        {
            Sound sound = GetSound(name);
            if (sound != null)
            {
                // Clamp the value, then apply to both data and AudioSource
                sound.volume = Mathf.Clamp01(volume);
                sound.source.volume = sound.volume;
            }
        }

        /// <summary>
        /// Adjusts the pitch of the specified sound (both in the Sound data and its AudioSource).
        /// Clamps pitch between 0.1 and 3.
        /// </summary>
        /// <param name="name">The enum identifier of the sound to adjust.</param>
        /// <param name="pitch">Playback pitch multiplier (0.1 to 3).</param>
        public void SetPitch(SoundName name, float pitch)
        {
            Sound sound = GetSound(name);
            if (sound != null)
            {
                // Clamp the pitch, then apply to both data and AudioSource
                sound.pitch = Mathf.Clamp(pitch, 0.1f, 3f);
                sound.source.pitch = sound.pitch;
            }
        }

        #endregion

        #region Mixer Control Methods

        /// <summary>
        /// Sets the master volume level on the AudioMixer by converting a linear volume value (0 to 1)
        /// to a decibel scale (-80 dB to 0 dB) and applying it to the "MasterVolume" exposed parameter.
        /// </summary>
        /// <param name="value">
        /// The linear volume value, expected to be in the range [0, 1],
        /// where 0 represents mute and 1 represents full volume.
        /// </param>
        public void SetMasterVolume(float value)
        {
            // Map 0-1 range to -80 dB to 0 dB
            float dB = Mathf.Lerp(-80f, 0f, value);
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

        /// <summary>
        /// Finds the Sound object in the array corresponding to the given SoundName.
        /// Logs an error if no matching Sound is found.
        /// </summary>
        /// <param name="name">The SoundName to search for.</param>
        /// <returns>The matching Sound, or null if not found.</returns>
        private Sound GetSound(SoundName name)
        {
            foreach (Sound s in sounds)
            {
                if (s.CompareName(name))
                {
                    return s;
                }
            }

            Debug.LogError($"Sound '{name}' not found!");
            return null;
        }
    }
}
