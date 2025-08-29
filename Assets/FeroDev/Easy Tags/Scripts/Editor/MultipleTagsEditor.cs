using UnityEditor;
using UnityEngine;

/**
 * MultipleTagsEditor is a custom editor script for MultipleTags component
 * It displays a custom inspector menu for MultipleTags and extends what you can do in the inspector
 * It lets you to add to and remove tags from TagMask of MultipleTags in a more user friendly way
 * It also has the code to direct the users to the TagManager window
 * 
 * @author (FeroDev)
 * @version 1.0
 * @date 26/06/2024
 */

namespace EasyTags
{
    [CustomEditor(typeof(MultipleTags))]
    [CanEditMultipleObjects]
    public class MultipleTagsEditor : Editor
    {
        private GUIStyle headerStyle;
        private GUIStyle boxStyle;
        private GUIStyle tagStyle;
        private GUIStyle noTagSelectedStyle;

        private string[] selectedTags = new string[0];
        private Vector2 scrollPosition;

        public override void OnInspectorGUI()
        {
            InitializeStyles();

            // Move this component to the top
            MoveComponentToTop();

            EditorGUILayout.BeginVertical(boxStyle);

            // Header
            EditorGUILayout.LabelField("Multiple Tags", headerStyle);

            // Tag mask field
            serializedObject.Update();
            SerializedProperty tagMaskProperty = serializedObject.FindProperty("tagMask");
            EditorGUILayout.PropertyField(tagMaskProperty);

            // Get the selected tags
            MultipleTags myTarget = (MultipleTags)target;
            selectedTags = myTarget.GetSelectedTags();
            EditorGUILayout.LabelField("Selected Tags:", headerStyle);

            if (selectedTags.Length > 0)
            {
                // Display selected tags as buttons with horizontal scroll
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                EditorGUILayout.BeginHorizontal();
                foreach (string tag in selectedTags)
                {
                    // Remove tags from the mask with button click
                    if (GUILayout.Button(tag, tagStyle, GUILayout.ExpandWidth(false)))
                    {
                        myTarget.RemoveTagFromMask(tag);
                        serializedObject.ApplyModifiedProperties();
                    }
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndScrollView();
            }
            else
            {
                EditorGUILayout.LabelField("No tags selected", noTagSelectedStyle);
            }

            // Redirect to Tag Manager
            EditorGUILayout.Space();
            if (GUILayout.Button("Open Tag Manager", GUILayout.Height(30)))
            {
                TagManagerWindow.ShowWindow();
            }

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.EndVertical();
        }

        private void MoveComponentToTop()
        {
            MultipleTags script = (MultipleTags)target;

            // Ensure the component is at the top in the inspector
            while (UnityEditorInternal.ComponentUtility.MoveComponentUp(script)) { }
        }

        // Styles for the UI elements
        private void InitializeStyles()
        {
            headerStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 14
            };
            headerStyle.normal.textColor = Color.cyan;

            boxStyle = new GUIStyle(GUI.skin.box)
            {
                padding = new RectOffset(10, 10, 10, 10),
                margin = new RectOffset(10, 10, 10, 10)
            };

            tagStyle = new GUIStyle(GUI.skin.button);
            tagStyle.normal.textColor = Color.white;
            tagStyle.hover.textColor = Color.red;
            tagStyle.fontStyle = FontStyle.Bold;

            noTagSelectedStyle = new GUIStyle(tagStyle);
            noTagSelectedStyle.normal.textColor = Color.red;
        }
    }
}
