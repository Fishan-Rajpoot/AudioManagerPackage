# 🎵 GameDevFishy Audio Manager

**A lightweight and flexible audio management solution for Unity projects.**  
Easily manage sound effects, music, and audio mixer settings with a user-friendly interface and custom editor tools.

---

## 🌟 Features

- **Centralized Audio Management**: Control all your game's audio clips (sound effects and music) from a single `AudioManager` script.
- **Custom SoundName Enum Editor**: Add, remove, or modify sound names via an intuitive Unity Editor window.
- **Audio Mixer Integration**: Seamlessly adjust master, music, and SFX volumes with a linear-to-decibel conversion for natural audio control.
- **Prefab Ready**: Includes a pre-configured `AudioManager` prefab for instant setup in your Unity project.
- **Searchable Sound Inspector**: Filter and manage sounds in the Unity Inspector with ease.
- **Play, Pause, Stop, and More**: Control audio playback with simple method calls (e.g., `Play`, `PlayOneShot`, `Pause`, `Stop`).
- **Customizable Sound Properties**: Adjust volume, pitch, looping, and play-on-awake settings for each sound.

---

## 📦 Installation

1. **Download the Package**:
   - Clone or download the repository from [GitHub](#) (replace with your repository link).
   - Alternatively, import the package via Unity's Package Manager (if distributed as a UPM package).

2. **Add to Unity**:
   - Copy the `AudioManager` folder into your Unity project's `Assets` directory.
   - The folder structure includes:
     ```
     Assets/
     └── AudioManager/
         ├── Prefabs/
         │   └── AudioManager.prefab
         ├── Scripts/
         │   ├── AudioManager.cs
         │   ├── AudioManagerEditor.cs
         │   ├── SoundNameEditorWindow.cs
         │   └── SoundNames.cs
         └── Mixer/
             └── AudioMixer.mixer
     ```

3. **Set Up the Prefab**:
   - Drag the `AudioManager.prefab` from `Assets/AudioManager/Prefabs/` into your Unity scene.
   - The prefab is pre-configured with the `AudioManager` script and an `AudioMixer` reference.

4. **Configure Audio Mixer**:
   - Ensure the `AudioMixer.mixer` file in `Assets/AudioManager/Mixer/` is correctly referenced.
   - If not assigned, use the "Get Mixer" button in the `AudioManager` Inspector to link it automatically.

---

## 🚀 Quick Start

1. **Add Sounds**:
   - Open the `AudioManager` Inspector in your scene.
   - Click **Add Sound** to create a new sound entry.
   - Assign an `AudioClip`, set the `SoundName` (from the `SoundName` enum), and configure properties like volume, pitch, and looping.

2. **Edit SoundName Enum**:
   - Click **Edit SoundName Enums** in the `AudioManager` Inspector to open the `SoundName Editor` window.
   - Add or remove enum values to match your project's sound names (e.g., `Background`, `Explosion`, `Click`).

3. **Control Audio in Code**:
   ```csharp
   // Play a sound
   AudioManager.instance.Play(SoundName.Background);

   // Adjust volume
   AudioManager.instance.SetVolume(SoundName.Background, 0.5f);

   // Adjust master volume
   AudioManager.instance.SetMasterVolume(0.8f);
   ```

4. **Search Sounds**:
   - Use the search bar in the `AudioManager` Inspector to filter sounds by their `SoundName`.

---

## 🎨 Editor Features

### SoundName Editor Window
- **Access**: `Window > SoundName Editor`
- **Functionality**:
  - View, add, or remove `SoundName` enum values.
  - Validates enum names to ensure they are valid C# identifiers.
  - Saves changes to `SoundNames.cs` and refreshes the Unity Asset Database.

### AudioManager Inspector
- **Searchable Sound List**: Filter sounds by name for quick access.
- **Add/Remove Sounds**: Easily manage your sound array with styled buttons.
- **Audio Mixer Auto-Setup**: Automatically assign the `AudioMixer` if missing.
- **Color-Coded UI**: Orange buttons for removing sounds and purple buttons for adding/editing for a visually distinct experience.

---

## 🔧 Usage Examples

### Playing a Sound
```csharp
// Play a looping background music track
AudioManager.instance.Play(SoundName.Music);

// Play a one-shot sound effect
AudioManager.instance.PlayOneShot(SoundName.Test);
```

### Adjusting Audio Settings
```csharp
// Set music volume to 50%
AudioManager.instance.SetMusicVolume(0.5f);

// Change pitch of a sound effect
AudioManager.instance.SetPitch(SoundName.Test, 1.5f);
```

### Checking Playback
```csharp
if (AudioManager.instance.IsSoundPlaying(SoundName.Background))
{
    Debug.Log("Background music is playing!");
}
```

---

## 🛠️ Customization

- **Audio Mixer Groups**:
  - Assign sounds to specific `AudioMixerGroup`s (e.g., Master, Music, SFX) for fine-tuned control.
  - Adjust volumes via `SetMasterVolume`, `SetMusicVolume`, or `SetSFXVolume`.

- **Sound Properties**:
  - Configure `volume` (0–1), `pitch` (0. mijn1–3), `loop`, and `playOnAwake` per sound.
  - Use the Unity Inspector to tweak these settings visually.

- **Extending SoundName Enum**:
  - Add new sound names via the `SoundName Editor` window to keep your audio organized.

---

## 📋 Requirements

- **Unity Version**: Unity 2019.4 or later.
- **Dependencies**:
  - Unity's `Audio` module.
  - `UnityEngine.Audio` and `UnityEditor` (for editor scripts).
- **Optional**: A pre-configured `AudioMixer` with exposed parameters (`MasterVolume`, `MusicVolume`, `SFXVolume`).

---

## 🐞 Troubleshooting

- **Audio Mixer Not Found**:
  - Ensure `AudioMixer.mixer` exists in `Assets/AudioManager/Mixer/`.
  - Use the "Get Mixer" button in the `AudioManager` Inspector to assign it.

- **Sound Not Playing**:
  - Verify the `AudioClip` is assigned and the `SoundName` matches an enum value.
  - Check the console for error logs (e.g., "Audio Source has not assigned").

- **Invalid Enum Names**:
  - Enum names must start with a letter or underscore and contain only letters, digits, or underscores.

---

## 🌈 Contributing

We welcome contributions to improve the Audio Manager package! To contribute:
1. Fork the repository.
2. Create a new branch for your feature or bugfix.
3. Submit a pull request with a clear description of your changes.

Please ensure your code follows the existing style and includes appropriate documentation.

---

## 📜 License

This package is licensed under the **MIT License**.

---

## 🎉 Acknowledgements

Built with ❤️ by the GameDevFishy team.  
Special thanks to the Unity community for inspiration and feedback.

---

<p align="center">
  <strong>Happy audio managing! 🎧</strong>
</p>
