using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

/**
 * TagMaskUtils is a static class to let you use TagMask Utility Functions globally
 * This class includes the all the functions to utilize MultipleTags usage like finding game objects and comparing tags
 * 
 * @author (FeroDev)
 * @version 1.0
 * @date 26/06/2024
 */

namespace EasyTags
{
    [InitializeOnLoad]
    public static class TagMaskUtils
    {
        #region Caching the Game Objects with MultipleTags

        // Dictionary for exact matches, keyed by TagMask value
        private static Dictionary<int, HashSet<GameObject>> _objectsByExactMask = new Dictionary<int, HashSet<GameObject>>();

        // Dictionary for partial matches, keyed by tag string, with HashSet of GameObjects
        private static Dictionary<string, HashSet<GameObject>> _objectsByIndividualTag = new Dictionary<string, HashSet<GameObject>>();

        /// <summary>
        /// TagMaskUtils initialization
        /// </summary>
        static TagMaskUtils()
        {
            // Subscribe to the sceneUnLoaded event
            SceneManager.sceneUnloaded += OnSceneUnLoaded;
        }

        /// <summary>
        /// Called when a scene is unloaded
        /// </summary>
        private static void OnSceneUnLoaded(Scene scene)
        {
            // Reset the dictionaries when the current scene is unloaded
            ResetDictionaries();
        }

        /// <summary>
        /// Resets the cache
        /// </summary>
        public static void ResetDictionaries()
        {
            _objectsByExactMask.Clear();
            _objectsByIndividualTag.Clear();
        }

        /// <summary>
        /// Registers a game object with a TagMask to the cache
        /// </summary>
        /// <param name="obj">Game object to be registered</param>
        /// <param name="tagMask">TagMask of the game object to be registered</param>
        public static void RegisterGameObject(GameObject obj, TagMask tagMask)
        {
            int maskValue = tagMask.Mask;

            // Register for exact matches
            if (!_objectsByExactMask.ContainsKey(maskValue))
            {
                _objectsByExactMask[maskValue] = new HashSet<GameObject>();
            }
            _objectsByExactMask[maskValue].Add(obj);

            // Register for partial matches
            string[] tags = tagMask.GetSelectedTags();
            foreach (string tag in tags)
            {
                if (!_objectsByIndividualTag.ContainsKey(tag))
                {
                    _objectsByIndividualTag[tag] = new HashSet<GameObject>();
                }
                _objectsByIndividualTag[tag].Add(obj);
            }
        }


        /// <summary>
        /// Unregisters a game object with a TagMask from the cache
        /// </summary>
        /// <param name="obj">Game object to be unregistered</param>
        /// <param name="tagMask">TagMask of the game object to be unregistered</param>
        public static void UnregisterGameObject(GameObject obj, TagMask tagMask)
        {
            int maskValue = tagMask.Mask;

            // Unregister from exact matches
            if (_objectsByExactMask.ContainsKey(maskValue))
            {
                // Remove the game object from the HashSet
                _objectsByExactMask[maskValue].Remove(obj);

                // Optionally remove the HashSet entry if no objects are left
                if (_objectsByExactMask[maskValue].Count == 0)
                {
                    _objectsByExactMask.Remove(maskValue);
                }
            }

            // Unregister from partial matches
            string[] tags = tagMask.GetSelectedTags();
            foreach (string tag in tags)
            {
                if (_objectsByIndividualTag.ContainsKey(tag))
                {
                    _objectsByIndividualTag[tag].Remove(obj);
                    // Optionally remove the tag entry if no objects are left
                    if (_objectsByIndividualTag[tag].Count == 0)
                    {
                        _objectsByIndividualTag.Remove(tag);
                    }
                }
            }
        }

        #endregion

        #region Getting Selected Tags Functions
        // These functions get the selected tags in a TagMask and return them in an array of strings.

        /// <summary>
        /// Gets the selected tags in the TagMask
        /// </summary>
        /// <param name="mask">The mask to get the selected tags from</param>
        /// <returns>An array of strings for selected tags</returns>
        public static string[] GetSelectedTags(int mask)
        {
            string[] allTags = UnityEditorInternal.InternalEditorUtility.tags;
            List<string> selectedTagsList = new();

            for (int i = 0; i < allTags.Length; i++)
            {
                if ((mask & (1 << i)) != 0)
                {
                    selectedTagsList.Add(allTags[i]);
                }
            }

            return selectedTagsList.ToArray();
        }

        /// <summary>
        /// Gets the selected tags in the TagMask
        /// </summary>
        /// <param name="mask">The mask to get the selected tags from</param>
        /// <returns>An array of strings for selected tags</returns>
        public static string[] GetSelectedTags(TagMask mask)
        {
            return GetSelectedTags(mask.Mask);
        }

        /// <summary>
        /// Gets the selected tags in the MultipleTags
        /// </summary>
        /// <param name="multipleTags">The mask to get the selected tags from</param>
        /// <returns>An array of strings for selected tags</returns>
        public static string[] GetSelectedTags(MultipleTags multipleTags)
        {
            return GetSelectedTags(multipleTags.TagMask);
        }

        #endregion

        #region Finding Game Objects Functions
        // These functions find game objects based on their selected tags on their MultipleTags component.
        // They require game objects with MultipleTags component attached.
        // They return a single or an array of game objects.

        /// <summary>
        /// Find the first GameObject with any of the tags in the mask
        /// </summary>
        /// <param name="mask">The mask to search the game objects with</param>
        /// <returns>A game object with any of the tags in the mask</returns>
        public static GameObject FindGameObjectWithTag(int mask)
        {
            string[] tags = GetSelectedTags(mask);

            foreach (string tag in tags)
            {
                if (_objectsByIndividualTag.TryGetValue(tag, out HashSet<GameObject> gameObjects))
                {
                    foreach (GameObject obj in gameObjects)
                    {
                        return obj;
                    }
                }
            }

            return null; // No game object found with the given tags
        }

        /// <summary>
        /// Find the first GameObject with any of the tags in the mask
        /// </summary>
        /// <param name="mask">The mask to search the game objects with</param>
        /// <returns>A game object with any of the tags in the mask</returns>
        public static GameObject FindGameObjectWithTag(TagMask mask)
        {
            return FindGameObjectWithTag(mask.Mask);
        }

        /// <summary>
        /// Find the first GameObject with all of the tags in the mask
        /// </summary>
        /// <param name="mask">The mask to search the game objects with</param>
        /// <returns>A game object with all the tags in the mask</returns>
        public static GameObject FindGameObjectWithTags(int mask)
        {
            if (_objectsByExactMask.TryGetValue(mask, out HashSet<GameObject> gameObjects))
            {
                return gameObjects.First();
            }

            return null;
        }

        /// <summary>
        /// Find the first GameObject with all of the tags in the mask
        /// </summary>
        /// <param name="mask">The mask to search the game objects with</param>
        /// <returns>A game object with all the tags in the mask</returns>
        public static GameObject FindGameObjectWithTags(TagMask mask)
        {
            return FindGameObjectWithTags(mask.Mask);
        }

        /// <summary>
        /// Find all GameObjects with any of the tags in the mask
        /// </summary>
        /// <param name="mask">The mask to search the game objects with</param>
        /// <returns>An array of game objects with any of the tags in the mask</returns>
        public static GameObject[] FindGameObjectsWithTag(int mask)
        {
            HashSet<GameObject> uniqueObjects = new HashSet<GameObject>();
            string[] tags = GetSelectedTags(mask);

            foreach (string tag in tags)
            {
                if (_objectsByIndividualTag.TryGetValue(tag, out HashSet<GameObject> gameObjects))
                {
                    uniqueObjects.UnionWith(gameObjects);
                }
            }

            return uniqueObjects.ToArray(); // Convert the HashSet to an array and return it
        }

        /// <summary>
        /// Find all GameObjects with any of the tags in the mask
        /// </summary>
        /// <param name="mask">The mask to search the game objects with</param>
        /// <returns>An array of game objects with any of the tags in the mask</returns>
        public static GameObject[] FindGameObjectsWithTag(TagMask mask)
        {
            return FindGameObjectsWithTag(mask.Mask);
        }

        /// <summary>
        /// Find all of the GameObjects with all of the tags in the mask
        /// </summary>
        /// <param name="mask">The mask to search the game objects with</param>
        /// <returns>An array of game objects with all the tags in the mask</returns>
        public static GameObject[] FindGameObjectsWithTags(int mask)
        {
            if (_objectsByExactMask.TryGetValue(mask, out HashSet<GameObject> objects))
            {
                return objects.ToArray(); // Return the HashSet as an array
            }

            return Array.Empty<GameObject>(); // Return an empty array if no game objects found
        }

        /// <summary>
        /// Find all of the GameObjects with all of the tags in the mask
        /// </summary>
        /// <param name="mask">The mask to search the game objects with</param>
        /// <returns>An array of game objects with all the tags in the mask</returns>
        public static GameObject[] FindGameObjectsWithTags(TagMask mask)
        {
            return FindGameObjectsWithTags(mask.Mask);
        }

        #endregion

        #region Compare Tag Functions
        // These functions compare the MultipleTags of game objects.
        // They require game objects with MultipleTags component attached.
        // Compare tags of a game object with another TagMask.
        // Compare tags of a game objects with another game object.
        // They return a boolean value.

        /// <summary>
        /// Check if the GameObject has any of the tags in the mask
        /// </summary>
        /// <param name="gameObject">The game object to compare tags with</param>
        /// <param name="mask">The mask to compare the game objects with</param>
        /// <returns>True if the game object has any of the tags in the mask</returns>
        public static bool CompareTag(GameObject gameObject, int mask)
        {
            string[] tags = GetSelectedTags(mask);
            string[] tags2 = gameObject.GetComponent<MultipleTags>().GetSelectedTags();
            foreach (string tag in tags)
            {
                foreach (string tag2 in tags2)
                {
                    if (tag.Equals(tag2))
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Check if the GameObject has any of the tags in the mask
        /// </summary>
        /// <param name="gameObject">The game object to compare tags with</param>
        /// <param name="mask">The mask to compare the game objects with</param>
        /// <returns>True if the game object has any of the tags from the mask</returns>
        public static bool CompareTag(GameObject gameObject, TagMask mask)
        {
            return CompareTag(gameObject, mask.Mask);
        }

        /// <summary>
        /// Check if the GameObject has all of the tags in the mask
        /// </summary>
        /// <param name="gameObject">The game object to compare tags with</param>
        /// <param name="mask">The mask to compare the game objects with</param>
        /// <returns>True if the game object has all the tags in the mask</returns>
        public static bool CompareTags(GameObject gameObject, int mask)
        {
            string[] maskTags = GetSelectedTags(mask); // Tags from the mask

            if (!gameObject.TryGetComponent<MultipleTags>(out var multipleTags))
            {
                Debug.LogWarning("GameObject does not have MultipleTags component.");
                return false;
            }
            string[] gameObjectTags = multipleTags.GetSelectedTags();

            foreach (string tag in maskTags)
            {
                bool found = false;
                foreach (string objTag in gameObjectTags)
                {
                    if (tag.Equals(objTag))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Check if the GameObject has all of the tags in the mask
        /// </summary>
        /// <param name="gameObject">The game object to compare tags with</param>
        /// <param name="mask">The mask to compare the game objects with</param>
        /// <returns>True if the game object has all the tags in the mask</returns>
        public static bool CompareTags(GameObject gameObject, TagMask mask)
        {
            return CompareTags(gameObject, mask.Mask);
        }

        /// <summary>
        /// Check if two GameObjects have any matching tags
        /// </summary>
        /// <param name="obj1">The game object to compare tags with</param>
        /// <param name="obj2">The game object to compare tags with</param>
        /// <returns>True if both game objects has any matching tags</returns>
        public static bool CompareGameObjectsTag(GameObject obj1, GameObject obj2)
        {
            string[] tags1 = obj1.GetComponent<MultipleTags>().GetSelectedTags();
            string[] tags2 = obj2.GetComponent<MultipleTags>().GetSelectedTags();

            foreach (string tag1 in tags1)
            {
                foreach (string tag2 in tags2)
                {
                    if (tag1.Equals(tag2))
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Check if two GameObjects have all matching tags
        /// </summary>
        /// <param name="obj1">The game object to compare tags with</param>
        /// <param name="obj2">The game object to compare tags with</param>
        /// <returns>True if both game objects has all matching tags</returns>
        public static bool CompareGameObjectsTags(GameObject obj1, GameObject obj2)
        {
            TagMask tagMask1 = obj1.GetComponent<MultipleTags>().TagMask;
            TagMask tagMask2 = obj2.GetComponent<MultipleTags>().TagMask;
            bool obj1HasAll = false;
            bool obj2HasAll = false;

            if (CompareTags(obj1, tagMask2)) obj1HasAll = true;
            if (CompareTags(obj2, tagMask1)) obj2HasAll = true;

            if (obj1HasAll && obj2HasAll) return true;
            else return false;
        }

        #endregion

        #region Has Matching Functions (Has any or all <<Game Objects>> in <<Array or List>> with similar <<Tag or Tags>>)
        // These functions check if elements from an array or a list of game objects has matching tags.
        // They require game objects with MultipleTags component attached.
        // Check if any two elements has matching tags.
        // Check if any two elements has matching TagMasks.
        // Check if all elements has matching TagMasks.
        // They return a boolean value.

        /// <summary>
        /// Check if any two GameObjects in the array have matching TagMask
        /// </summary>
        /// <param name="gameObjects">Array of game objects to compare tags with</param>
        /// <returns>True if any two game objects have any TagMask matching</returns>
        public static bool HasAnyMatchingTags(GameObject[] gameObjects)
        {
            HashSet<int> seenMasks = new HashSet<int>();

            foreach (var gameObject in gameObjects)
            {
                if (gameObject.TryGetComponent<MultipleTags>(out var tags))
                {
                    if (seenMasks.Contains(tags.TagMask.Mask))
                        return true;

                    seenMasks.Add(tags.TagMask.Mask);
                }
            }
            return false;
        }

        /// <summary>
        /// Check if any two GameObjects in the array have matching tags in their TagMask
        /// </summary>
        /// <param name="gameObjects">Array of game objects to compare tags with</param>
        /// <returns>True if any two game objects have any tags matching in their TagMask</returns>
        public static bool HasAnyMatchingTag(GameObject[] gameObjects)
        {
            HashSet<string> seenTags = new HashSet<string>();

            foreach (var gameObject in gameObjects)
            {
                if (gameObject.TryGetComponent<MultipleTags>(out var tags))
                {
                    foreach (var tag in tags.GetSelectedTags())
                    {
                        if (seenTags.Contains(tag))
                            return true;

                        seenTags.Add(tag);
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Check if all elements have all matching TagMask
        /// </summary>
        /// <param name="gameObjects">Array of game objects to compare tags with</param>
        /// <returns>True if all game objects have all matching tags in their TagMask</returns>
        public static bool HasAllMatching(GameObject[] gameObjects)
        {
            if (gameObjects == null || gameObjects.Length == 0)
                return false;

            HashSet<string> firstTags = null;

            foreach (var gameObject in gameObjects)
            {
                if (!gameObject.TryGetComponent<MultipleTags>(out var tags))
                    return false;

                if (firstTags == null)
                {
                    firstTags = new HashSet<string>(tags.GetSelectedTags());
                }
                else
                {
                    bool hasMatchingTag = false;
                    foreach (var tag in tags.GetSelectedTags())
                    {
                        if (firstTags.Contains(tag))
                        {
                            hasMatchingTag = true;
                            break;
                        }
                    }

                    if (!hasMatchingTag)
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Check if any two GameObjects in the list have matching TagMask
        /// </summary>
        /// <param name="gameObjects">List of game objects to compare tags with</param>
        /// <returns>True if any two game objects have matching TagMask</returns>
        public static bool HasAnyMatchingTags(List<GameObject> gameObjects)
        {
            HashSet<int> seenMasks = new HashSet<int>();

            foreach (var gameObject in gameObjects)
            {
                if (gameObject.TryGetComponent<MultipleTags>(out var tags))
                {
                    if (seenMasks.Contains(tags.TagMask.Mask))
                        return true;

                    seenMasks.Add(tags.TagMask.Mask);
                }
            }
            return false;
        }

        /// <summary>
        /// Check if any two GameObjects in the list have matching tags in their TagMask
        /// </summary>
        /// <param name="gameObjects">List of game objects to compare tags with</param>
        /// <returns>Returns true if any two game objects have any matching tags in their TagMask</returns>
        public static bool HasAnyMatchingTag(List<GameObject> gameObjects)
        {
            HashSet<string> seenTags = new HashSet<string>();

            foreach (var gameObject in gameObjects)
            {
                if (gameObject.TryGetComponent<MultipleTags>(out var tags))
                {
                    foreach (var tag in tags.GetSelectedTags())
                    {
                        if (seenTags.Contains(tag))
                            return true;

                        seenTags.Add(tag);
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Check if all elements in the list have all matching TagMasks
        /// </summary>
        /// <param name="gameObjects">List of game objects to compare tags with</param>
        /// <returns>True if all game objects have all mathching tags in their TagMask</returns>
        public static bool HasAllMatching(List<GameObject> gameObjects)
        {
            if (gameObjects == null || gameObjects.Count == 0)
                return false;

            HashSet<string> firstTags = null;

            foreach (var gameObject in gameObjects)
            {
                if (!gameObject.TryGetComponent<MultipleTags>(out var tags))
                    return false;

                if (firstTags == null)
                {
                    firstTags = new HashSet<string>(tags.GetSelectedTags());
                }
                else
                {
                    bool hasMatchingTag = false;
                    foreach (var tag in tags.GetSelectedTags())
                    {
                        if (firstTags.Contains(tag))
                        {
                            hasMatchingTag = true;
                            break;
                        }
                    }

                    if (!hasMatchingTag)
                        return false;
                }
            }
            return true;
        }

        #endregion

        #region Find Transforms Functions
        // These fucntions find transforms based on their selected tags on their MultipleTags component.
        // They require game objects with MultipleTags component attached.
        // They return a single or an array of transforms.

        /// <summary>
        /// Find a Transform with any of the tags in the mask
        /// </summary>
        /// <param name="mask">The mask to search the transforms with</param>
        /// <returns>A transform with any of the tags in the mask</returns>
        public static Transform FindWithTag(int mask)
        {
            return FindGameObjectWithTag(mask).transform;
        }

        /// <summary>
        /// Find a Transform with any of the tags in the mask
        /// </summary>
        /// <param name="mask">The mask to search the transforms with</param>
        /// <returns>A transform with any of the tags in the mask</returns>
        public static Transform FindWithTag(TagMask mask)
        {
            return FindWithTag(mask.Mask);
        }

        /// <summary>
        /// Find a Transform with all of the tags in the mask
        /// </summary>
        /// <param name="mask">The mask to search the transforms with</param>
        /// <returns>A transform with all of the tags in the mask</returns>
        public static Transform FindWithTags(int mask)
        {
            return FindGameObjectWithTags(mask).transform;
        }

        /// <summary>
        /// Find a Transform with all of the tags in the mask
        /// </summary>
        /// <param name="mask">The mask to search the transforms with</param>
        /// <returns>A transform with all of the tags in the mask</returns>
        public static Transform FindWithTags(TagMask mask)
        {
            return FindWithTags(mask.Mask);
        }

        /// <summary>
        /// Find all Transforms with any of the tags in the mask
        /// </summary>
        /// <param name="mask">The mask to search the transforms with</param>
        /// <returns>An array of transforms with any of the tags in the mask</returns>
        public static Transform[] FindAllWithTag(int mask)
        {
            return ConvertGameObjectsToTransforms(FindGameObjectsWithTag(mask));
        }

        /// <summary>
        /// Find all Transforms with any of the tags in the mask
        /// </summary>
        /// <param name="mask">The mask to search the transforms with</param>
        /// <returns>An array of transforms with any of the tags in the mask</returns>
        public static Transform[] FindAllWithTag(TagMask mask)
        {
            return FindAllWithTag(mask.Mask);
        }

        /// <summary>
        /// Find all Transforms with all of the tags in the mask
        /// </summary>
        /// <param name="mask">The mask to search the transforms with</param>
        /// <returns>An array of transforms with all of the tags in the mask</returns>
        public static Transform[] FindAllWithTags(int mask)
        {
            return ConvertGameObjectsToTransforms(FindGameObjectsWithTags(mask));
        }

        /// <summary>
        /// Find all Transforms with all of the tags in the mask
        /// </summary>
        /// <param name="mask">The mask to search the transforms with</param>
        /// <returns>An array of transforms with all of the tags in the mask</returns>
        public static Transform[] FindAllWithTags(TagMask mask)
        {
            return FindAllWithTags(mask.Mask);
        }

        #endregion

        #region Finding Game Objects with TagMask but Without a MultipleTags Component Attached
        // These functions find game objects using a TagMask, but based on their default tags instead of MultipleTags.
        // They do not require MultipleTags to work.
        // They return a single or an array of game objects.

        /// <summary>
        /// Find the first GameObject with any tag in the mask without a MultipleTags attached
        /// </summary>
        /// <param name="mask">The mask to search the game objcects with</param>
        /// <returns>A game object with any tag in the mask</returns>
        public static GameObject FindGameObjectWithoutMultipleTags(int mask)
        {
            string[] tags = GetSelectedTags(mask);
            for (int i = 0; i < tags.Length; i++)
            {
                GameObject gameObject = GameObject.FindGameObjectWithTag(tags[i]);
                if (gameObject != null) return gameObject;
            }
            return null;
        }

        /// <summary>
        /// Find the first GameObject with any tag in the mask without a MultipleTags attached
        /// </summary>
        /// <param name="mask">The mask to search the game objcects with</param>
        /// <returns>A game object with any tag in the mask</returns>
        public static GameObject FindGameObjectWithoutMultipleTags(TagMask mask)
        {
            return FindGameObjectWithoutMultipleTags(mask.Mask);
        }

        /// <summary>
        /// Find all of the GameObjects with any tag in the mask without a MultipleTags attached
        /// </summary>
        /// <param name="mask">The mask to search the game objcects with</param>
        /// <returns>An array of game objects with any tag in the mask</returns>
        public static GameObject[] FindGameObjectsWithoutMultipleTags(int mask)
        {
            HashSet<GameObject> uniqueGameObjects = new();
            string[] tags = GetSelectedTags(mask);

            for (int i = 0; i < tags.Length; i++)
            {
                GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(tags[i]);

                foreach (GameObject obj in objectsWithTag)
                    uniqueGameObjects.Add(obj);
            }

            GameObject[] resultArray = new GameObject[uniqueGameObjects.Count];
            uniqueGameObjects.CopyTo(resultArray);
            return resultArray;
        }

        /// <summary>
        /// Find all of the GameObjects with any tag in the mask without a MultipleTags attached
        /// </summary>
        /// <param name="mask">The mask to search the game objcects with</param>
        /// <returns>An array of game objects with any tag in the mask</returns>
        public static GameObject[] FindGameObjectsWithoutMultipleTags(TagMask mask)
        {
            return FindGameObjectsWithoutMultipleTags(mask.Mask);
        }

        #endregion

        #region Finding Transforms with TagMask but Without a MultipleTags Component Attached
        // These functions find transforms using a TagMask, but based on their default tags instead of MultipleTags.
        // They do not require MultipleTags to work.
        // They return a single or an array of transforms.

        /// <summary>
        /// Find the first Transform with any tag in the mask without a MultipleTags attached
        /// </summary>
        /// <param name="mask">The mask to search the game objcects with</param>
        /// <returns>A transform with any tag in the mask</returns>
        public static Transform FindWithoutMultipleTags(int mask)
        {
            string[] tags = GetSelectedTags(mask);
            for (int i = 0; i < tags.Length; i++)
            {
                GameObject gameObject = GameObject.FindGameObjectWithTag(tags[i]);
                if (gameObject != null) return gameObject.transform;
            }
            return null;
        }

        /// <summary>
        /// Find the first Transform with any tag in the mask without a MultipleTags attached
        /// </summary>
        /// <param name="mask">The mask to search the game objcects with</param>
        /// <returns>A transform with any tag in the mask</returns>
        public static Transform FindWithoutMultipleTags(TagMask mask)
        {
            return FindWithoutMultipleTags(mask.Mask);
        }

        /// <summary>
        /// Find all of the Transform with any tag in the mask without a MultipleTags attached
        /// </summary>
        /// <param name="mask">The mask to search the game objcects with</param>
        /// <returns>An array of transforms with any tag in the mask</returns>
        public static Transform[] FindAllWithoutMulipleTags(int mask)
        {
            HashSet<Transform> uniqueTransforms = new();
            string[] tags = GetSelectedTags(mask);

            for (int i = 0; i < tags.Length; i++)
            {
                GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(tags[i]);

                foreach (GameObject obj in objectsWithTag)
                    uniqueTransforms.Add(obj.transform);
            }

            // Convert the HashSet to an array and return it
            Transform[] resultArray = new Transform[uniqueTransforms.Count];
            uniqueTransforms.CopyTo(resultArray);
            return resultArray;
        }

        /// <summary>
        /// Find all of the Transforms with any tag in the mask without a MultipleTags attached
        /// </summary>
        /// <param name="mask">The mask to search the game objcects with</param>
        /// <returns>An array of game objects with any tag in the mask</returns>
        public static Transform[] FindAllWithoutMulipleTags(TagMask mask)
        {
            return FindAllWithoutMulipleTags(mask.Mask);
        }

        #endregion

        #region Updating TagMask Functions
        // These functions updates a TagMask using a tag or a list or an array of tags.
        // Add a single or more than one tags to a mask.
        // Delete a single or more than one tags from a mask.
        // Toggle a single or more than one tags in the mask.

        /// <summary>
        /// Add a tag to a TagMask and return the resulting TagMask
        /// </summary>
        /// <param name="tagMask">The TagMask to be updated</param>
        /// <param name="tag">The tag to be added/param>
        /// <returns>The updated TagMask</returns>
        public static TagMask AddTagToMask(TagMask tagMask, string tag)
        {
            int tagIndex = Array.IndexOf(UnityEditorInternal.InternalEditorUtility.tags, tag);
            if (tagIndex != -1)
                tagMask.Mask |= (1 << tagIndex);
            return tagMask;
        }

        /// <summary>
        /// Add an array of tags to a TagMask and return the resulting TagMask
        /// </summary>
        /// <param name="tagMask">The TagMask to be updated</param>
        /// <param name="tags">The array of tags to be added</param>
        /// <returns>The updated TagMask</returns>
        public static TagMask AddTagsToMask(TagMask tagMask, string[] tags)
        {
            for (int i = 0; i < tags.Length; i++)
                AddTagToMask(tagMask, tags[i]);
            return tagMask;
        }

        /// <summary>
        /// Add a list of tags to a TagMask and return the resulting TagMask
        /// </summary>
        /// <param name="tagMask">The TagMask to be updated</param>
        /// <param name="tags">The list of tags to be added</param>
        /// <returns>The updated TagMask</returns>
        public static TagMask AddTagsToMask(TagMask tagMask, List<string> tags)
        {
            foreach (string tag in tags)
                AddTagToMask(tagMask, tag);
            return tagMask;
        }

        /// <summary>
        /// Remove a tag from a TagMask and return the resulting TagMask 
        /// </summary>
        /// <param name="tagMask">The TagMask to be updated</param>
        /// <param name="tag">The tag to be removed</param>
        /// <returns>The updated TagMask</returns>
        public static TagMask RemoveTagFromMask(TagMask tagMask, string tag)
        {
            int tagIndex = Array.IndexOf(UnityEditorInternal.InternalEditorUtility.tags, tag);
            if (tagIndex != -1)
                tagMask.Mask &= ~(1 << tagIndex);
            return tagMask;
        }

        /// <summary>
        /// Remove an array of tags to a TagMask and return the resulting TagMask
        /// </summary>
        /// <param name="tagMask">The TagMask to be updated</param>
        /// <param name="tags">The array of tags to be removed</param>
        /// <returns>The updated TagMask</returns>
        public static TagMask RemoveTagsFromMask(TagMask tagMask, string[] tags)
        {
            for (int i = 0; i < tags.Length; i++)
                RemoveTagFromMask(tagMask, tags[i]);
            return tagMask;
        }

        /// <summary>
        /// Remove a list of tags to a TagMask and return the resulting TagMask
        /// </summary>
        /// <param name="tagMask">The TagMask to be updated</param>
        /// <param name="tags">The list of tags to be removed</param>
        /// <returns>The updated TagMask</returns>
        public static TagMask RemoveTagsFromMask(TagMask tagMask, List<string> tags)
        {
            foreach (string tag in tags)
                RemoveTagFromMask(tagMask, tag);
            return tagMask;
        }

        /// <summary>
        /// Toggle a tag in a TagMask (add if not present, remove if present)
        /// </summary>
        /// <param name="tagMask">The TagMask to be updated</param>
        /// <param name="tag">The tag to be toggled</param>
        /// <returns>The updated TagMask</returns>
        public static TagMask ToggleTag(TagMask tagMask, string tag)
        {
            int tagIndex = Array.IndexOf(UnityEditorInternal.InternalEditorUtility.tags, tag);
            if (tagIndex != -1)
                tagMask.Mask ^= (1 << tagIndex);
            return tagMask;
        }

        /// <summary>
        /// Toggle an array of tags in a TagMask and return the resulting TagMask
        /// </summary>
        /// <param name="tagMask">The TagMask to be updated</param>
        /// <param name="tags">The array of tags to be toggled</param>
        /// <returns>The updated TagMask</returns>
        public static TagMask ToggleTags(TagMask tagMask, string[] tags)
        {
            for (int i = 0; i < tags.Length; i++)
                ToggleTag(tagMask, tags[i]);
            return tagMask;
        }

        /// <summary>
        /// Toggle a list of tags in a TagMask and return the resulting TagMask
        /// </summary>
        /// <param name="tagMask">The TagMask to be updated</param>
        /// <param name="tags">The list of tags to be toggled</param>
        /// <returns>The updated TagMask</returns>
        public static TagMask ToggleTags(TagMask tagMask, List<string> tags)
        {
            foreach (string tag in tags)
                ToggleTag(tagMask, tag);
            return tagMask;
        }

        #endregion

        /// <summary>
        /// Converts an array of GameObjects to an array of Transforms.
        /// </summary>
        /// <param name="gameObjects">Array of GameObjects to convert.</param>
        /// <returns>An array of Transforms corresponding to the input GameObjects.</returns>
        public static Transform[] ConvertGameObjectsToTransforms(GameObject[] gameObjects)
        {
            Transform[] transforms = new Transform[gameObjects.Length];

            for (int i = 0; i < gameObjects.Length; i++)
            {
                transforms[i] = gameObjects[i].transform;
            }

            return transforms;
        }
    }
}
