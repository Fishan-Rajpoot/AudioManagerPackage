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
        private const string FilePath = "Assets/AudioManager/Scripts/SoundNames.cs";
        private bool allEnumValid = false;

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

            PrintEnumFields();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Add New Enum Value", EditorStyles.boldLabel);
            newEnumValue = EditorGUILayout.TextField("New Value", newEnumValue);
            if (GUILayout.Button("Add Value") &&
                !string.IsNullOrEmpty(newEnumValue) &&
                !enumValues.Contains(newEnumValue))
            {
                enumValues.Add(newEnumValue);
                newEnumValue = "";
            }

            EditorGUILayout.Space();
            if (allEnumValid)
            {
                if (GUILayout.Button("Save Changes"))
                {
                    SaveEnumToFile();
                }
            }
            else
            {
                // Show a warning if any enum name is invalid
                EditorGUILayout.HelpBox(
                    "One or more enum names are invalid.\n" +
                    "• Must start with a letter or underscore.\n" +
                    "• Can only contain letters, digits, or underscores.\n" +
                    "Please fix the highlighted entries before saving.",
                    MessageType.Warning
                );
            }

            EditorGUILayout.EndScrollView();
        }

        private void PrintEnumFields()
        {
            EditorGUILayout.LabelField("SoundName Enum Values", EditorStyles.boldLabel);
            allEnumValid = true;

            for (int i = 0; i < enumValues.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                // If this entry is not a valid C# identifier, highlight it red
                if (!IsValidEnumName(enumValues[i]))
                {
                    GUI.backgroundColor = Color.red;
                    allEnumValid = false;
                }

                enumValues[i] = EditorGUILayout.TextField("Enum Value", enumValues[i]);
                if (GUILayout.Button("Remove", GUILayout.Width(60)))
                {
                    enumValues.RemoveAt(i);
                    i--; // step back one index because we removed the current entry
                }

                GUI.backgroundColor = Color.white;
                EditorGUILayout.EndHorizontal();
            }
        }

        private void SaveEnumToFile()
        {
            // Generate enum entries with explicit indices, e.g. "Test = 0,"
            var lines = new List<string>();
            for (int i = 0; i < enumValues.Count; i++)
            {
                // You can leave a trailing comma on the last entry; C# allows it.
                lines.Add($"{enumValues[i]} = {i}");
            }

            string enumBody = string.Join($",{System.Environment.NewLine}        ", lines);

            // Wrap in namespace + enum declaration
            string enumCode =
                "namespace GameDevFishy.Audio\n" +
                "{\n" +
                "    public enum SoundName\n" +
                "    {\n" +
                $"        {enumBody}\n" +
                "    }\n" +
                "}";

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
