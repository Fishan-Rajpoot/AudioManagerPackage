using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

namespace GameDevFishy.Audio
{
    [CustomEditor(typeof(AudioManager))]
    internal class AudioManagerEditor : Editor
    {
        private string searchQuery = "";
        private Vector2 scrollPosition;

        private string AudioMixerFolderPath = "Assets/AudioManager/Mixer";
        private string audioMixerName = "AudioMixer.mixer";

        public override void OnInspectorGUI()
        {
            AudioManager audioManager = (AudioManager)target;
            serializedObject.Update();

            AudioMixerProperties();

            EditEnum();
            // Search bar
            ShowSounds(audioManager);

            serializedObject.ApplyModifiedProperties();
        }

        private void ShowSounds(AudioManager audioManager)
        {
            EditorGUILayout.LabelField("Search Sounds", EditorStyles.boldLabel);
            searchQuery = EditorGUILayout.TextField("Search by SoundName", searchQuery);

            // Sounds array
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Sounds", EditorStyles.boldLabel);
            SerializedProperty soundsProp = serializedObject.FindProperty("sounds");

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            for (int i = 0; i < soundsProp.arraySize; i++)
            {
                SerializedProperty soundProp = soundsProp.GetArrayElementAtIndex(i);
                string soundName = soundProp.FindPropertyRelative("name").enumDisplayNames[soundProp.FindPropertyRelative("name").enumValueIndex];

                // Filter by search query
                if (string.IsNullOrEmpty(searchQuery) || soundName.ToLower().Contains(searchQuery.ToLower()))
                {
                    // Begin horizontal for left margin only if the sound is rendered
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(10); // Add a 10-pixel left margin
                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    EditorGUILayout.PropertyField(soundProp, new GUIContent(soundName), true);

                    // Sound controls
                    Sound sound = audioManager.sounds[i];

                    //GUI.backgroundColor = Color.red;
                    GUI.backgroundColor = new Color32(252, 125, 40, 255);

                    // Remove button
                    if (GUILayout.Button("Remove"))
                    {
                        soundsProp.DeleteArrayElementAtIndex(i);
                        serializedObject.ApplyModifiedProperties();
                    }
                    GUI.backgroundColor = Color.white;
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();
                }
            }
            EditorGUILayout.EndScrollView();

            // Add button
            EditorGUILayout.BeginHorizontal();
            GUI.backgroundColor = new Color32(180, 80, 255, 255);
            if (GUILayout.Button("Add Sound"))
            {
                soundsProp.arraySize++;
                SerializedProperty newSoundProp = soundsProp.GetArrayElementAtIndex(soundsProp.arraySize - 1);
                newSoundProp.FindPropertyRelative("volume").floatValue = 1f;
                newSoundProp.FindPropertyRelative("pitch").floatValue = 1f;
            }
            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();
        }

        private static void EditEnum()
        {
            GUILayout.Space(10);
            GUI.backgroundColor = new Color32(180, 80, 255, 255);
            // Button to open SoundName editor
            if (GUILayout.Button("Edit SoundName Enums"))
            {
                SoundNameEditorWindow.ShowWindow();
            }
            GUI.backgroundColor = Color.white;
        }

        private void AudioMixerProperties()
        {
            SerializedProperty mixerProp = serializedObject.FindProperty("AudioMixer");
            EditorGUILayout.PropertyField(mixerProp);

            if (mixerProp.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox("Audio Mixer is not assigned!", MessageType.Warning);

                if (GUILayout.Button("Get Mixer"))
                {
                    string fullPath = System.IO.Path.Combine(AudioMixerFolderPath, audioMixerName);
                    AudioMixer mixer = AssetDatabase.LoadAssetAtPath<AudioMixer>(fullPath);

                    if (mixer != null)
                    {
                        mixerProp.objectReferenceValue = mixer;
                        serializedObject.ApplyModifiedProperties();
                        Debug.Log("Audio Mixer assigned successfully!");
                    }
                    else
                    {
                        Debug.LogWarning("Audio Mixer not found at: " + fullPath);
                    }
                }
            }
        }
    }
}