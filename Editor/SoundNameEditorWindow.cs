using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GameDevFishy.Audio
{
    public class SoundNameEditorWindow : EditorWindow
    {
        private List<string> enumValues = new List<string>();
        private string newEnumValue = "";
        private Vector2 scrollPosition;
        private const string FilePath = "Assets/AudioManager/SoundNames.cs"; // Adjust path as needed

        [MenuItem("Window/SoundName Editor")]
        public static void ShowWindow()
        {
            GetWindow<SoundNameEditorWindow>("SoundName Enum Editor");
        }

        private void OnEnable()
        {
            // Load current enum values
            enumValues = System.Enum.GetNames(typeof(SoundName)).ToList();
        }

        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            EditorGUILayout.LabelField("SoundName Enum Values", EditorStyles.boldLabel);
            for (int i = 0; i < enumValues.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                enumValues[i] = EditorGUILayout.TextField("Enum Value", enumValues[i]);
                if (GUILayout.Button("Remove"))
                {
                    enumValues.RemoveAt(i);
                    i--;
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Add New Enum Value", EditorStyles.boldLabel);
            newEnumValue = EditorGUILayout.TextField("New Value", newEnumValue);
            if (GUILayout.Button("Add Value") && !string.IsNullOrEmpty(newEnumValue) && !enumValues.Contains(newEnumValue))
            {
                enumValues.Add(newEnumValue);
                newEnumValue = "";
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("Save Changes"))
            {
                SaveEnumToFile();
            }

            EditorGUILayout.EndScrollView();
        }

        private void SaveEnumToFile()
        {
            // Validate enum names
            foreach (string value in enumValues)
            {
                if (!IsValidEnumName(value))
                {
                    EditorUtility.DisplayDialog("Error", $"Invalid enum name: {value}. Enum names must be valid C# identifiers.", "OK");
                    return;
                }
            }

            // Generate enum code
            string enumCode = "namespace GameDevFishy.Audio\n{\n    public enum SoundName\n    {\n        " + string.Join(",\n        ", enumValues) + "\n    }\n}";
            File.WriteAllText(FilePath, enumCode);
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("Success", "SoundName enum updated successfully!", "OK");
        }

        private bool IsValidEnumName(string name)
        {
            if (string.IsNullOrEmpty(name)) return false;
            if (!char.IsLetter(name[0]) && name[0] != '_') return false;
            return name.All(c => char.IsLetterOrDigit(c) || c == '_');
        }
    }
}