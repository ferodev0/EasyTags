using UnityEngine;
using UnityEditor;

/**
 * TagManagerWindow is a menu element that renders a custom window for tag for you to manage.
 * It automatically detects the Unity default tags.
 * It lets you to add new tags.
 * It lets you to delete the tags you created.
 * 
 * @author (FeroDev)
 * @version 1.0
 * @date 26/06/2024
 */

namespace EasyTags
{
    public class TagManagerWindow : EditorWindow
    {
        private static readonly bool IS_DEFAULT_TAGS_DYNAMIC = true;

        private string newTag = "";
        private Vector2 tagScrollPosition = Vector2.zero;
        private GUIStyle headerStyle;
        private GUIStyle defaultTagStyle;
        private GUIStyle tagStyle;
        private GUIStyle buttonStyle;
        private GUIStyle deleteButtonStyle;
        private GUIStyle addButtonStyle;

        private static TagManagerWindow instance;
        private bool isModified = false;

        [MenuItem("Tools/Easy Tags/Tag Manager", false, 50)]
        public static void ShowWindow()
        {
            // Show the tag manager window
            instance = GetWindow<TagManagerWindow>("Tag Manager");
            instance.minSize = new Vector2(300, 400);
        }

        public static void RefreshWindow()
        {
            if (instance != null)
            {
                instance.Close();
                ShowWindow();
                instance.Repaint(); // Repaint the window to update UI
            }
        }

        void OnGUI()
        {
            InitializeStyling();

            // Header
            AddHeader("All Tags");

            // Display all tags in a scroll view
            tagScrollPosition = EditorGUILayout.BeginScrollView(tagScrollPosition, GUILayout.Height(position.height - 120));
            string[] tags = UnityEditorInternal.InternalEditorUtility.tags;

            // Iterate through all defined tags
            for (int i = 0; i < tags.Length; i++)
            {
                if (isModified)
                {
                    tags = UnityEditorInternal.InternalEditorUtility.tags;
                    isModified = false;
                }

                string tag = tags[i];

                // Check if the tag is a default Unity tag
                bool isDefaultTag = IsTagUnityDefault(tag);

                // Set alternating background colors
                GUI.backgroundColor = i % 2 == 0 ? Color.white : new Color(0.9f, 0.9f, 0.9f);

                // Start of a tag row
                EditorGUILayout.BeginHorizontal(GUI.skin.box);

                // Add delete button for non-default tags
                if (!isDefaultTag)
                {
                    // Display the tag
                    GUILayout.Box(tag, tagStyle, GUILayout.ExpandWidth(true));

                    // Display the button
                    if (GUILayout.Button("Delete", deleteButtonStyle, GUILayout.MaxWidth(100)))
                    {
                        // Double checking before deleting a tag
                        if (EditorUtility.DisplayDialog("Delete Tag", $"Are you sure you want to delete the tag '{tag}'?", "Yes", "No"))
                        {
                            TagGenerator.DeleteTag(tag);
                            isModified = true;
                            break; // Exit the loop after deleting
                        }
                    }
                }
                else
                {
                    // Display the tag
                    GUILayout.Box(tag, defaultTagStyle, GUILayout.ExpandWidth(true));

                    // Disable the button if Unity default tag
                    GUI.enabled = false;
                    if (GUILayout.Button("Default", buttonStyle, GUILayout.MaxWidth(100)))
                    {
                        // Still handle deleting a default tag just in case
                        if (EditorUtility.DisplayDialog("Default Tag", "Unity default tags cannot be deleted.", "OK"))
                        {
                            Debug.Log("Unity default tags cannot be deleted.");
                        }
                    }
                    GUI.enabled = true;
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();

            // Input area for adding new tags
            AddHeader("Add New Tag");
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(10);
            EditorGUILayout.LabelField("New Tag:", GUILayout.Width(80));
            GUILayout.FlexibleSpace();
            newTag = EditorGUILayout.TextField(newTag, GUILayout.MinWidth(120));

            // Adding a new tag
            if (GUILayout.Button("Add Tag", addButtonStyle))
            {
                if (!string.IsNullOrEmpty(newTag))
                {
                    TagGenerator.AddTag(newTag);
                    isModified = true;
                    newTag = "";
                }
                else
                {
                    EditorUtility.DisplayDialog("Error", "Tag name cannot be empty!", "OK");
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        // Styles for UI elements
        private void InitializeStyling()
        {
            headerStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 14
            };
            headerStyle.normal.textColor = Color.cyan;

            defaultTagStyle = new GUIStyle(GUI.skin.box);
            defaultTagStyle.normal.textColor = Color.white;
            defaultTagStyle.hover.textColor = Color.white;
            defaultTagStyle.active.textColor = Color.white;
            defaultTagStyle.padding = new RectOffset(5, 5, 5, 5);
            defaultTagStyle.margin = new RectOffset(5, 5, 0, 0);
            defaultTagStyle.border = new RectOffset(2, 2, 2, 2);

            tagStyle = new GUIStyle(GUI.skin.box);
            tagStyle.normal.textColor = Color.cyan;
            tagStyle.hover.textColor = Color.cyan;
            tagStyle.active.textColor = Color.cyan;
            tagStyle.padding = new RectOffset(5, 5, 5, 5);
            tagStyle.margin = new RectOffset(5, 5, 0, 0);
            tagStyle.border = new RectOffset(2, 2, 2, 2);

            deleteButtonStyle = new GUIStyle(GUI.skin.button);
            deleteButtonStyle.normal.textColor = Color.red;
            deleteButtonStyle.hover.textColor = Color.red;
            deleteButtonStyle.active.textColor = Color.red;
            deleteButtonStyle.padding = new RectOffset(10, 10, 3, 3);
            deleteButtonStyle.margin = new RectOffset(10, 10, 0, 0);

            addButtonStyle = new GUIStyle(GUI.skin.button);
            addButtonStyle.normal.textColor = Color.green;
            addButtonStyle.hover.textColor = Color.green;
            addButtonStyle.active.textColor = Color.green;
            addButtonStyle.padding = new RectOffset(10, 10, 3, 3);
            addButtonStyle.margin = new RectOffset(10, 10, 0, 0);

            buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.normal.textColor = Color.white;
            buttonStyle.hover.textColor = Color.white;
            buttonStyle.active.textColor = Color.white;
            buttonStyle.padding = new RectOffset(10, 10, 3, 3);
            buttonStyle.margin = new RectOffset(10, 10, 0, 0);
        }

        private void AddHeader(string header)
        {
            GUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(10);
            EditorGUILayout.LabelField(header, headerStyle);
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(10);
        }

        // Check if a tag is Unity default based on a dynamic or a static method
        private bool IsTagUnityDefault(string tag)
        {
            if (IS_DEFAULT_TAGS_DYNAMIC)
                return !TagGenerator.IsUserDefinedTag(tag);
            else
                return TagGenerator.IsUnityDefaultTag(tag);
        }
    }
}
