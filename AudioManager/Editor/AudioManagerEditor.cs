using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using System;
using System.Collections.Generic;

namespace GameDevFishy.Audio
{
    [CustomEditor(typeof(AudioManager))]
    internal class AudioManagerEditor : Editor
    {
        private string searchQuery = "";
        private Vector2 scrollPosition;

        // (Leave this path as-is)
        private string AudioMixerFolderPath = "Assets/AudioManager/Mixer";
        private string audioMixerName = "AudioMixer.mixer";

        public override void OnInspectorGUI()
        {
            // Grab reference to the target AudioManager
            AudioManager audioManager = (AudioManager)target;

            // Ensure serializedObject is in sync
            serializedObject.Update();

            // Draw the AudioMixer field + “Get Mixer” button if null
            AudioMixerProperties();

            // Draw the “Edit SoundName Enums” button
            GUILayout.Space(10);
            GUI.backgroundColor = new Color32(180, 80, 255, 255);
            if (GUILayout.Button("Edit SoundName Enums"))
            {
                SoundNameEditorWindow.ShowWindow();
            }
            GUI.backgroundColor = Color.white;

            // Draw search bar + synchronized sound list
            ShowSounds(audioManager);

            // Push changes back to the target
            serializedObject.ApplyModifiedProperties();
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

        private void ShowSounds(AudioManager audioManager)
        {
            // 1) Synchronize the array so that every SoundName enum has one Sound entry.
            //
            // We do this *before* drawing, so missing enum‐named Sounds get auto-added with default volume=1, pitch=1.
            //
            SerializedProperty soundsProp = serializedObject.FindProperty("sounds");
            SyncSoundsWithEnum(soundsProp);

            // 2) Draw a search field at the top
            GUILayout.Space(8);
            EditorGUILayout.LabelField("Search Sounds", EditorStyles.boldLabel);
            searchQuery = EditorGUILayout.TextField("Search by SoundName", searchQuery);

            // 3) Draw the “Sounds” list
            GUILayout.Space(8);
            EditorGUILayout.LabelField("Sounds", EditorStyles.boldLabel);

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            for (int i = 0; i < soundsProp.arraySize; i++)
            {
                SerializedProperty soundProp = soundsProp.GetArrayElementAtIndex(i);
                // Get the enum name string:
                string soundName = soundProp
                    .FindPropertyRelative("name")
                    .enumDisplayNames[soundProp.FindPropertyRelative("name").enumValueIndex];

                // Filter out anything that doesn’t match the search
                if (!string.IsNullOrEmpty(searchQuery) &&
                    !soundName.ToLower().Contains(searchQuery.ToLower()))
                {
                    continue;
                }

                // Begin drawing one “boxed” entry per Sound
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(10);
                EditorGUILayout.BeginVertical(GUI.skin.box);

                // Draw the Sound property, labeling the foldout with the enum name
                EditorGUILayout.PropertyField(soundProp, new GUIContent(soundName), true);

                // Draw a Remove button (in case you want to delete that Sound entry)
                Sound soundObj = audioManager.sounds[i];
                GUI.backgroundColor = new Color32(252, 125, 40, 255);
                if (GUILayout.Button("Remove"))
                {
                    soundsProp.DeleteArrayElementAtIndex(i);
                    // After deleting, we must re‐sync on the next repaint so that missing enum entries get re-created.
                }
                GUI.backgroundColor = Color.white;

                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndScrollView();

            // (We explicitly omit any “Add Sound” button here – new entries come from SyncSoundsWithEnum.)
        }

        /// <summary>
        /// Ensures that for each SoundName enum value there is exactly one element 
        /// in the serialized “sounds” array. If any enum is missing, we insert a new Sound
        /// with default volume=1 and pitch=1. 
        /// </summary>
        private void SyncSoundsWithEnum(SerializedProperty soundsProp)
        {
            // Build a lookup of which enum‐indices we already have
            HashSet<int> existingIndices = new HashSet<int>();
            for (int i = 0; i < soundsProp.arraySize; i++)
            {
                SerializedProperty element = soundsProp.GetArrayElementAtIndex(i);
                int enumIndex = element.FindPropertyRelative("name").enumValueIndex;
                existingIndices.Add(enumIndex);
            }

            // Iterate over every SoundName value
            var allEnumValues = (SoundName[])Enum.GetValues(typeof(SoundName));
            foreach (SoundName sn in allEnumValues)
            {
                int idx = (int)sn;
                if (!existingIndices.Contains(idx))
                {
                    // Insert a new array element at the end
                    int newIndex = soundsProp.arraySize;
                    soundsProp.arraySize++;
                    var newElement = soundsProp.GetArrayElementAtIndex(newIndex);

                    // Assign the enum‐field to this SoundName
                    newElement.FindPropertyRelative("name").enumValueIndex = idx;
                    // Default volume and pitch
                    newElement.FindPropertyRelative("volume").floatValue = 1f;
                    newElement.FindPropertyRelative("pitch").floatValue = 1f;
                }
            }
        }
    }
}
