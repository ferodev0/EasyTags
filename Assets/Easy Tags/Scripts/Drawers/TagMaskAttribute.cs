using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/**
 * TagMaskAttribute and TagMaskDrawer are used to display a custom property in the inspector for TagMask
 * These classes are used to display integers as a bitmask field for the defined tags in Unity
 * 
 * @author Ferhat Cemoglu (FeroDev)
 * @version 1.0
 * @date 26/06/2024
 */

namespace EasyTags
{
    public class TagMaskAttribute : PropertyAttribute { }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(TagMask))]
    public class TagMaskDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty maskProperty = property.FindPropertyRelative("mask");
            if (maskProperty != null && maskProperty.propertyType == SerializedPropertyType.Integer)
            {
                EditorGUI.BeginChangeCheck();

                int mask = 0;
                string[] tags = UnityEditorInternal.InternalEditorUtility.tags;

                // Get the current mask value
                if (maskProperty.intValue != 0)
                {
                    for (int i = 0; i < tags.Length; i++)
                    {
                        if ((maskProperty.intValue & (1 << i)) != 0)
                        {
                            mask |= 1 << System.Array.IndexOf(tags, tags[i]);
                        }
                    }
                }

                // Draw the mask field with the tag names
                mask = EditorGUI.MaskField(position, label, mask, tags);

                // Convert the mask value back to the integer value for storing
                int value = 0;
                for (int i = 0; i < tags.Length; i++)
                {
                    if ((mask & (1 << i)) != 0)
                    {
                        value |= 1 << System.Array.IndexOf(tags, tags[i]);
                    }
                }

                // Apply changes to the serialized property
                if (EditorGUI.EndChangeCheck())
                {
                    maskProperty.intValue = value;
                }
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "Use TagMask with an integer.");
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property.FindPropertyRelative("mask"));
        }
    }
}
#endif
