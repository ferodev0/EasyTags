using UnityEditor;
using UnityEngine;

/**
 * AddMultipleTagsToSelection lets you to add the MultipleTags component to selected game objects in the hierarchy
 * It also adds whatever tag the game obejct has to the mask
 * It add a menu item for its functionality
 * 
 * @author Ferhat Cemoglu (FeroDev)
 * @version 1.0
 * @date 26/06/2024
 */

namespace EasyTags
{
    public class AddMultipleTagsToSelection : MonoBehaviour
    {
        // Add MultipleTags component to selected GameObjects
        [MenuItem("Easy Tags/Add MultipleTags to Selected", false, 10)]
        private static void AddMultipleTags()
        {
            // Iterate through selected GameObjects
            foreach (GameObject obj in Selection.gameObjects)
            {
                // Add the MultipleTags component if it doesn't exist
                if (obj.GetComponent<MultipleTags>() == null)
                {
                    obj.AddComponent<MultipleTags>();
                }
                obj.GetComponent<MultipleTags>().AddTagToMask(obj.tag);
            }
        }

        // Enable menu item only if at least one GameObject is selected
        [MenuItem("Easy Tags/Add MultipleTags to Selected", true)]
        private static bool ValidateAddMultipleTags()
        {
            return Selection.gameObjects.Length > 0;
        }

        // Add MultipleTags component to all GameObjects in the hierarchy
        [MenuItem("Easy Tags/Add MultipleTags to All", false, 11)]
        private static void AddMultipleTagsToAll()
        {
            // Show confirmation dialog
            if (EditorUtility.DisplayDialog("Confirm Add MultipleTags to All",
                "Are you sure you want to add MultipleTags to all GameObjects in the hierarchy?",
                "Yes", "No"))
            {
                // Iterate through all GameObjects in the hierarchy
                foreach (GameObject obj in FindObjectsByType<GameObject>(FindObjectsSortMode.None))
                {
                    // Add the MultipleTags component if it doesn't exist
                    if (obj.GetComponent<MultipleTags>() == null)
                    {
                        obj.AddComponent<MultipleTags>();
                    }
                    obj.GetComponent<MultipleTags>().AddTagToMask(obj.tag);
                }

                // Notify user of completion
                EditorUtility.DisplayDialog("MultipleTags Added",
                    "MultipleTags component has been added to all GameObjects.",
                    "OK");
            }
        }
    }
}
