using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

/**
 * TagMaskUtils is a static class to let you use TagMask Utility Functions globally
 * This class includes all the functions to utilize MultipleTags usage like finding game objects and comparing tags
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

            // Register for exact matches (TryGetValue = one dictionary lookup instead of ContainsKey + indexer)
            if (!_objectsByExactMask.TryGetValue(maskValue, out HashSet<GameObject> exactSet))
            {
                exactSet = new HashSet<GameObject>();
                _objectsByExactMask[maskValue] = exactSet;
            }
            exactSet.Add(obj);

            // Register for partial matches
            string[] tags = tagMask.GetSelectedTags();
            foreach (string tag in tags)
            {
                if (!_objectsByIndividualTag.TryGetValue(tag, out HashSet<GameObject> tagSet))
                {
                    tagSet = new HashSet<GameObject>();
                    _objectsByIndividualTag[tag] = tagSet;
                }
                tagSet.Add(obj);
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
            if (_objectsByExactMask.TryGetValue(maskValue, out HashSet<GameObject> exactSet))
            {
                // Remove the game object from the HashSet
                exactSet.Remove(obj);

                // Optionally remove the HashSet entry if no objects are left
                if (exactSet.Count == 0)
                {
                    _objectsByExactMask.Remove(maskValue);
                }
            }

            // Unregister from partial matches
            string[] tags = tagMask.GetSelectedTags();
            foreach (string tag in tags)
            {
                if (_objectsByIndividualTag.TryGetValue(tag, out HashSet<GameObject> tagSet))
                {
                    tagSet.Remove(obj);
                    // Optionally remove the tag entry if no objects are left
                    if (tagSet.Count == 0)
                    {
                        _objectsByIndividualTag.Remove(tag);
                    }
                }
            }
        }

        #endregion

        #region Getting Selected Tags Functions
        // These functions get the selected tags in a TagMask and return them in an array of strings.

        // Cached copy of Unity's tag list. Without this, UnityEditorInternal.InternalEditorUtility.tags
        // gets re-fetched on every single call made anywhere in this class (nearly every method funnels
        // through GetSelectedTags or the mask-editing functions below).
        private static string[] _cachedAllTags;
        private static string[] AllTags => _cachedAllTags ??= UnityEditorInternal.InternalEditorUtility.tags;

        /// <summary>
        /// Clears the cached tag list, forcing it to be re-read from Unity's Tag Manager on the next call.
        /// Call this if tags are added/removed/renamed while the cache may already be populated.
        /// </summary>
        public static void RefreshTagCache()
        {
            _cachedAllTags = null;
        }

        /// <summary>
        /// Gets the selected tags in the TagMask
        /// </summary>
        /// <param name="mask">The mask to get the selected tags from</param>
        /// <returns>An array of strings for selected tags</returns>
        public static string[] GetSelectedTags(int mask)
        {
            string[] allTags = AllTags;
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
                // Grab the first element without pulling in System.Linq just for this one call,
                // and without throwing if the set were ever empty (foreach simply won't execute).
                foreach (GameObject obj in gameObjects)
                {
                    return obj;
                }
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
            if (!gameObject.TryGetComponent<MultipleTags>(out var multipleTags))
            {
                Debug.LogWarning("GameObject does not have MultipleTags component.");
                return false;
            }

            // HashSet.Overlaps is O(n) instead of the previous nested-loop O(n*m) comparison
            HashSet<string> maskTags = new HashSet<string>(GetSelectedTags(mask));
            return maskTags.Overlaps(multipleTags.GetSelectedTags());
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
            if (!gameObject.TryGetComponent<MultipleTags>(out var multipleTags))
            {
                Debug.LogWarning("GameObject does not have MultipleTags component.");
                return false;
            }

            HashSet<string> maskTags = new HashSet<string>(GetSelectedTags(mask)); // Tags from the mask
            HashSet<string> gameObjectTags = new HashSet<string>(multipleTags.GetSelectedTags());

            // True only if every tag in the mask is also present on the game object
            return maskTags.IsSubsetOf(gameObjectTags);
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
            if (!obj1.TryGetComponent<MultipleTags>(out var tags1) || !obj2.TryGetComponent<MultipleTags>(out var tags2))
            {
                Debug.LogWarning("One or both GameObjects do not have a MultipleTags component.");
                return false;
            }

            HashSet<string> tagSet1 = new HashSet<string>(tags1.GetSelectedTags());
            return tagSet1.Overlaps(tags2.GetSelectedTags());
        }

        /// <summary>
        /// Check if two GameObjects have all matching tags
        /// </summary>
        /// <param name="obj1">The game object to compare tags with</param>
        /// <param name="obj2">The game object to compare tags with</param>
        /// <returns>True if both game objects has all matching tags</returns>
        public static bool CompareGameObjectsTags(GameObject obj1, GameObject obj2)
        {
            if (!obj1.TryGetComponent<MultipleTags>(out var multipleTags1) || !obj2.TryGetComponent<MultipleTags>(out var multipleTags2))
            {
                Debug.LogWarning("One or both GameObjects do not have a MultipleTags component.");
                return false;
            }

            bool obj1HasAll = CompareTags(obj1, multipleTags2.TagMask);
            bool obj2HasAll = CompareTags(obj2, multipleTags1.TagMask);

            return obj1HasAll && obj2HasAll;
        }

        #endregion

        #region Has Matching Functions (Has any or all <<Game Objects>> in <<Array or List>> with similar <<Tag or Tags>>)
        // These functions check if elements from an array or a list of game objects has matching tags.
        // They require game objects with MultipleTags component attached.
        // Check if any two elements has matching tags.
        // Check if any two elements has matching TagMasks.
        // Check if all elements has matching TagMasks.
        // They return a boolean value.
        //
        // These accept IEnumerable<GameObject> / ICollection<GameObject> instead of separate
        // GameObject[] and List<GameObject> overloads. Both arrays and Lists satisfy these
        // interfaces, so every existing call site (array or list) keeps compiling and working
        // exactly as before -- this just removes the duplicated method bodies and duplicated docs.

        /// <summary>
        /// Check if any two GameObjects in the collection have matching TagMask
        /// </summary>
        /// <param name="gameObjects">Collection of game objects to compare tags with</param>
        /// <returns>True if any two game objects have any TagMask matching</returns>
        public static bool HasAnyMatchingTags(IEnumerable<GameObject> gameObjects)
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
        /// Check if any two GameObjects in the collection have matching tags in their TagMask
        /// </summary>
        /// <param name="gameObjects">Collection of game objects to compare tags with</param>
        /// <returns>True if any two game objects have any tags matching in their TagMask</returns>
        public static bool HasAnyMatchingTag(IEnumerable<GameObject> gameObjects)
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
        /// <param name="gameObjects">Collection of game objects to compare tags with</param>
        /// <returns>True if all game objects have all matching tags in their TagMask</returns>
        public static bool HasAllMatching(ICollection<GameObject> gameObjects)
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
        // These functions find transforms based on their selected tags on their MultipleTags component.
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
        /// <param name="mask">The mask to search the game objects with</param>
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
        /// <param name="mask">The mask to search the game objects with</param>
        /// <returns>A game object with any tag in the mask</returns>
        public static GameObject FindGameObjectWithoutMultipleTags(TagMask mask)
        {
            return FindGameObjectWithoutMultipleTags(mask.Mask);
        }

        /// <summary>
        /// Find all of the GameObjects with any tag in the mask without a MultipleTags attached
        /// </summary>
        /// <param name="mask">The mask to search the game objects with</param>
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
        /// <param name="mask">The mask to search the game objects with</param>
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
        /// <param name="mask">The mask to search the game objects with</param>
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
        /// <param name="mask">The mask to search the game objects with</param>
        /// <returns>A transform with any tag in the mask</returns>
        public static Transform FindWithoutMultipleTags(TagMask mask)
        {
            return FindWithoutMultipleTags(mask.Mask);
        }

        /// <summary>
        /// Find all of the Transform with any tag in the mask without a MultipleTags attached
        /// </summary>
        /// <param name="mask">The mask to search the game objects with</param>
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
        /// <param name="mask">The mask to search the game objects with</param>
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
        //
        // The "multiple tags" overloads accept IEnumerable<string>, so both string[] and
        // List<string> (or any other collection of tag names) work without needing a
        // separate overload for each collection type.

        /// <summary>
        /// Add a tag to a TagMask and return the resulting TagMask
        /// </summary>
        /// <param name="tagMask">The TagMask to be updated</param>
        /// <param name="tag">The tag to be added</param>
        /// <returns>The updated TagMask</returns>
        public static TagMask AddTagToMask(TagMask tagMask, string tag)
        {
            int tagIndex = Array.IndexOf(AllTags, tag);
            if (tagIndex != -1)
                tagMask.Mask |= (1 << tagIndex);
            return tagMask;
        }

        /// <summary>
        /// Add a collection of tags to a TagMask and return the resulting TagMask
        /// </summary>
        /// <param name="tagMask">The TagMask to be updated</param>
        /// <param name="tags">The collection of tags to be added</param>
        /// <returns>The updated TagMask</returns>
        public static TagMask AddTagsToMask(TagMask tagMask, IEnumerable<string> tags)
        {
            foreach (string tag in tags)
                tagMask = AddTagToMask(tagMask, tag);
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
            int tagIndex = Array.IndexOf(AllTags, tag);
            if (tagIndex != -1)
                tagMask.Mask &= ~(1 << tagIndex);
            return tagMask;
        }

        /// <summary>
        /// Remove a collection of tags from a TagMask and return the resulting TagMask
        /// </summary>
        /// <param name="tagMask">The TagMask to be updated</param>
        /// <param name="tags">The collection of tags to be removed</param>
        /// <returns>The updated TagMask</returns>
        public static TagMask RemoveTagsFromMask(TagMask tagMask, IEnumerable<string> tags)
        {
            foreach (string tag in tags)
                tagMask = RemoveTagFromMask(tagMask, tag);
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
            int tagIndex = Array.IndexOf(AllTags, tag);
            if (tagIndex != -1)
                tagMask.Mask ^= (1 << tagIndex);
            return tagMask;
        }

        /// <summary>
        /// Toggle a collection of tags in a TagMask and return the resulting TagMask
        /// </summary>
        /// <param name="tagMask">The TagMask to be updated</param>
        /// <param name="tags">The collection of tags to be toggled</param>
        /// <returns>The updated TagMask</returns>
        public static TagMask ToggleTags(TagMask tagMask, IEnumerable<string> tags)
        {
            foreach (string tag in tags)
                tagMask = ToggleTag(tagMask, tag);
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