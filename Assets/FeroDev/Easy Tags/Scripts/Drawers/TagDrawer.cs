using UnityEngine;
using UnityEditor;

/**
 * TagAttribute is used to display a custom property for singular tags
 * These classes are used to display strings as drop down field consisting of the defined tags in Unity
 * 
 * @author (FeroDev)
 * @version 1.0
 * @date 26/06/2024
 */

namespace EasyTags
{
    public class TagAttribute : PropertyAttribute { }

    [CustomPropertyDrawer(typeof(TagAttribute))]
    public class TagDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.String)
            {
                EditorGUI.BeginProperty(position, label, property);

                string[] tags = UnityEditorInternal.InternalEditorUtility.tags;
                int selectedIndex = -1;
                string currentValue = property.stringValue;

                // Find the index of the current tag in the list of tags
                for (int i = 0; i < tags.Length; i++)
                {
                    if (tags[i] == currentValue)
                    {
                        selectedIndex = i;
                        break;
                    }
                }

                // If the current tag is not found, default to the first tag
                if (selectedIndex == -1)
                    selectedIndex = 0;

                // Display the dropdown menu for selecting tags
                selectedIndex = EditorGUI.Popup(position, label.text, selectedIndex, tags);

                // Set the property value to the selected tag
                property.stringValue = tags[selectedIndex];

                EditorGUI.EndProperty();
            }
            else
            {
                EditorGUI.PropertyField(position, property, label);
            }
        }
    }
}
