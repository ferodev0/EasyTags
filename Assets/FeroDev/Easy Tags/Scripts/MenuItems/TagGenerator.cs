using UnityEditor;
using System.IO;
using System.Text;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;

/**
 * TagGenerator is used to generate accessible tags and update them accordingly.
 * This class generates another class called Tags and adds string constants for every tag defined in Unity to make them accessible by their actual defined names.
 * This class also has functionalities to add or remove tags, and detect user defined tags.
 * It adds a menu item for its functionality.
 * 
 * @author (FeroDev)
 * @version 1.0
 * @date 26/06/2024
 */

namespace EasyTags
{
    [InitializeOnLoad]
    public class TagGenerator : AssetPostprocessor
    {
        private static readonly string outputPath = "Assets/FeroDev/Easy Tags/Scripts/Tags/Tags.cs";

        // HashSet of predefined tags
        private static readonly HashSet<string> predefinedTags = new()
    {
        "RedTag",
        "GreenTag",
        "YellowTag",
        "BlueTag",
        "WhiteTag",
        "EnemyTag",
        "FriendTag",
        "PassiveTag"
    };

        // HashSet of default Unity tags
        private static readonly HashSet<string> defaultTags = new()
    {
        "Untagged",
        "Respawn",
        "Finish",
        "EditorOnly",
        "MainCamera",
        "Player",
        "GameController"
    };

        // HashSet of user defined tags
        private static readonly HashSet<string> userDefinedTags = new();

        private static string PlayerPrefsKey
        {
            get { return $"BetterTags_CheckTags_{GetProjectHash()}"; }
        }

        private static bool CheckTags
        {
            get { return PlayerPrefs.GetInt(PlayerPrefsKey, 1) == 1; } // Default to true if key doesn't exist
            set { PlayerPrefs.SetInt(PlayerPrefsKey, value ? 1 : 0); }
        }

        static TagGenerator()
        {
            // Run the tag check and generation when the project loads
            if (CheckTags) CheckAndAddPredefinedTags();
            GenerateTagsClass();
        }

        // Update the tags class when a new tag is created with asset refresh (like importing, moving, deleting an asset or saving the project)
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            if (ArrayContainsTagManager(importedAssets) || ArrayContainsTagManager(deletedAssets) || ArrayContainsTagManager(movedAssets))
            {
                if (CheckTags) CheckAndAddPredefinedTags();
                GenerateTagsClass();
            }
        }

        // Checks and adds predefined tags that are used in the Easy Tags Demo scene for testing
        public static void CheckAndAddPredefinedTags()
        {
            foreach (string tag in predefinedTags)
            {
                if (!IsTagDefined(tag))
                    AddTag(tag);
            }
            CheckTags = false;
        }

        private static bool IsTagDefined(string tag)
        {
            string[] tags = UnityEditorInternal.InternalEditorUtility.tags;
            foreach (string existingTag in tags)
            {
                if (existingTag == tag)
                    return true;
            }
            return false;
        }

        private static bool ArrayContainsTagManager(string[] array)
        {
            foreach (string item in array)
            {
                if (item.Contains("ProjectSettings/TagManager.asset"))
                {
                    return true;
                }
            }
            return false;
        }

        // Creates the Tags class
        [MenuItem("Tools/Easy Tags/Generate Accessible Tags", false, 30)]
        public static void GenerateTagsClass()
        {
            string[] tags = UnityEditorInternal.InternalEditorUtility.tags;

            // Writing the code for Tags class
            StringBuilder sb = new();
            sb.AppendLine("namespace EasyTags");
            sb.AppendLine("{");
            sb.AppendLine("public static class Tags");
            sb.AppendLine("{");

            // Adding a constant for each tag
            foreach (string tag in tags)
            {
                sb.AppendLine($"    public const string {SanitizeTag(tag)} = \"{tag}\";");
            }

            sb.AppendLine("}");
            sb.AppendLine("}");

            // Creating the script for Tags class
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
            File.WriteAllText(outputPath, sb.ToString());

            AssetDatabase.Refresh();
        }

        // Adds a tag in Unity and updates the Tags class
        public static void AddTag(string tag)
        {
            if (!Array.Exists(UnityEditorInternal.InternalEditorUtility.tags, existingTag => existingTag == tag))
            {
                // Getting the defined tags
                SerializedObject tagManager = new(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
                SerializedProperty tagsProp = tagManager.FindProperty("tags");

                // Adding the tag in correct position
                int index = tagsProp.arraySize;
                tagsProp.InsertArrayElementAtIndex(index);
                SerializedProperty sp = tagsProp.GetArrayElementAtIndex(index);
                sp.stringValue = tag;

                // Updating the changes
                tagManager.ApplyModifiedProperties();
                GenerateTagsClass();
                AssetDatabase.SaveAssets();  // Save the assets
                AssetDatabase.Refresh();     // Refresh the asset database
            }
            else
            {
                EditorUtility.DisplayDialog("Warning", $"'{tag}' is already created!", "OK");
            }
        }

        // Deletes a tag and updates the Tags class
        public static void DeleteTag(string tag)
        {
            // Getting the defined tags
            SerializedObject tagManager = new(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty tagsProp = tagManager.FindProperty("tags");

            // Iterating through the defined tags
            for (int i = 0; i < tagsProp.arraySize; i++)
            {
                // Checking if we found the tag to be deleted
                SerializedProperty sp = tagsProp.GetArrayElementAtIndex(i);
                if (sp.stringValue == tag)
                {
                    tagsProp.DeleteArrayElementAtIndex(i);
                    break;
                }
            }

            // Updating the changes
            tagManager.ApplyModifiedProperties();
            GenerateTagsClass();
            AssetDatabase.SaveAssets();  // Save the assets
            AssetDatabase.Refresh();     // Refresh the asset database
        }

        // Converts the raw tag to a plain string
        private static string SanitizeTag(string tag)
        {
            return tag.Replace(" ", "").Replace("-", "").Replace(".", "");
        }

        // Gets the user defined tags from the TagManager in ProjectSettings (change the path only if needed)
        public static HashSet<string> GetUserDefinedTags()
        {
            // Loading all the tags
            SerializedObject tagManager = new(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty tagsProp = tagManager.FindProperty("tags");

            // Getting the user defined tags (TagManager only contains user defined tags)
            for (int i = 0; i < tagsProp.arraySize; i++)
            {
                string tag = tagsProp.GetArrayElementAtIndex(i).stringValue;
                userDefinedTags.Add(tag);
            }

            return userDefinedTags;
        }

        public static bool IsUserDefinedTag(string tag)
        {
            GetUserDefinedTags();
            return userDefinedTags.Contains(tag);
        }

        public static bool IsUnityDefaultTag(string tag)
        {
            return defaultTags.Contains(tag);
        }

        // Generates a hash from the project path
        private static string GetProjectHash()
        {
            string projectPath = Application.dataPath;
            using SHA256 sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(projectPath));
            StringBuilder builder = new();
            foreach (byte b in bytes)
                builder.Append(b.ToString("x2"));
            return builder.ToString();
        }
    }
}
