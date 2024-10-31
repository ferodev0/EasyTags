using System;
using System.Collections.Generic;
using UnityEngine;

/**
 * MultipleTags component (attach this component to the game objects that you want to use multiple tags on)
 * This class include the necessary components to use multiple tags on game objects
 * 
 * @author Ferhat Cemoglu (FeroDev)
 * @version 1.0
 * @date 26/06/2024
 */

namespace EasyTags
{
    public class MultipleTags : MonoBehaviour
    {
        [SerializeField]
        private TagMask tagMask;

        /// <summary>
        /// Get and set the tagMask value
        /// </summary>
        public TagMask TagMask { get => tagMask; set => tagMask = value; }

        #region Registration

        /// <summary>
        /// Registers this game object with its TagMask
        /// </summary>
        private void OnEnable()
        {
            TagMaskUtils.RegisterGameObject(gameObject, tagMask);
        }

        /// <summary>
        /// Unregisters this game object with its TagMask
        /// </summary>
        private void OnDestroy()
        {
            TagMaskUtils.UnregisterGameObject(gameObject, tagMask);
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize MultipleTags with a TagMask
        /// </summary>
        /// <param name="tagMask">The TagMask to initialize with</param>
        public MultipleTags(TagMask tagMask)
        {
            this.tagMask = tagMask;
        }

        /// <summary>
        /// Initialize MultipleTags with a tag
        /// </summary>
        /// <param name="tag">The tag to initialize with</param>
        public MultipleTags(string tag)
        {
            AddTagToMask(tag);
        }

        /// <summary>
        /// Initialize MultipleTags with an array of tags
        /// </summary>
        /// <param name="tags">The array of tags to initialize with</param>
        public MultipleTags(string[] tags)
        {
            for (int i = 0; i < tags.Length; i++)
                AddTagToMask(tags[i]);
        }

        /// <summary>
        /// Initialize MultipleTags with a list of tags
        /// </summary>
        /// <param name="tags">The list of tags to initialize with</param>
        public MultipleTags(List<string> tags)
        {
            foreach (string tag in tags)
                AddTagToMask(tag);
        }

        #endregion

        /// <summary>
        /// Returns an array of strings of selected tags
        /// </summary>
        /// <returns>An array of selected tags</returns>
        public string[] GetSelectedTags()
        {
            string[] allTags = UnityEditorInternal.InternalEditorUtility.tags;
            List<string> selectedTagsList = new();

            for (int i = 0; i < allTags.Length; i++)
            {
                if ((tagMask.Mask & (1 << i)) != 0)
                {
                    selectedTagsList.Add(allTags[i]);
                }
            }

            return selectedTagsList.ToArray();
        }

        #region HasTag

        /// <summary>
        /// Checks if the mask has a tag
        /// </summary>
        /// <param name="tag">The tag to check</param>
        /// <returns>True if the tag is in the mask, false otherwise</returns>
        public bool HasTag(string tag)
        {
            return tagMask.HasTag(tag);
        }

        /// <summary>
        /// Checks if the mask has any tag from another mask
        /// </summary>
        /// <param name="mask">The mask to check against</param>
        /// <returns>True if any tag from the mask is in the mask, false otherwise</returns>
        public bool HasTag(int mask)
        {
            return tagMask.HasTag(mask);
        }

        #endregion

        #region HasTags

        /// <summary>
        /// Checks if the mask has all the tags from another mask
        /// </summary>
        /// <param name="mask">The mask to check against</param>
        /// <returns>True if all tags from the mask are in the mask, false otherwise</returns>
        public bool HasTags(int mask)
        {
            return tagMask.HasTags(mask);
        }

        /// <summary>
        /// Checks if the mask has all the tags from another mask
        /// </summary>
        /// <param name="mask">The TagMask to check against</param>
        /// <returns>True if all tags from the mask are in the mask, false otherwise</returns>
        public bool HasTags(TagMask mask)
        {
            return tagMask.HasTags(mask);
        }

        /// <summary>
        /// Checks if the mask has all the tags from an array of tags
        /// </summary>
        /// <param name="tags">The array of strings to check against</param>
        /// <returns>True if all tags from the array are in the mask, false otherwise</returns>
        public bool HasTags(string[] tags)
        {
            return tagMask.HasTags(tags);
        }

        /// <summary>
        /// Checks if the mask has all the tags from a list of tags
        /// </summary>
        /// <param name="tags">The list of strings to check against</param>
        /// <returns>True if all tags from the list are in the mask, false otherwise</returns>
        public bool HasTags(List<string> tags)
        {
            return tagMask.HasTags(tags);
        }

        #endregion

        #region MatchesWith

        /// <summary>
        /// Checks if the mask matches with another mask (no different tags are allowed)
        /// </summary>
        /// <param name="mask"></param>
        /// <returns>True if the masks match, false otherwise</returns>
        public bool MatchesWith(int mask)
        {
            return tagMask.MatchesWith(mask);
        }

        /// <summary>
        /// Checks if the mask matches with another mask (no different tags are allowed)
        /// </summary>
        /// <param name="mask"></param>
        /// <returns>True if the masks match, false otherwise</returns>
        public bool MatchesWith(TagMask mask)
        {
            return tagMask.MathesWith(mask);
        }

        #endregion

        #region Modify Mask

        /// <summary>
        /// Adds a tag to the mask
        /// </summary>
        /// <param name="tag">The tag to add to the mask</param>
        public void AddTagToMask(string tag)
        {
            int tagIndex = Array.IndexOf(UnityEditorInternal.InternalEditorUtility.tags, tag);
            if (tagIndex != -1)
            {
                tagMask.Mask |= (1 << tagIndex);
            }
        }

        /// <summary>
        /// Adds an array of tags to the mask
        /// </summary>
        /// <param name="tags">The array of tags to add to the mask</param>
        public void AddTagsToMask(string[] tags)
        {
            for (int i = 0; i < tags.Length; i++)
                AddTagToMask(tags[i]);
        }

        /// <summary>
        /// Adds a list of tags to the mask
        /// </summary>
        /// <param name="tags">The list of tags to add to the mask</param>
        public void AddTagsToMask(List<string> tags)
        {
            foreach (string tag in tags)
                AddTagToMask(tag);
        }

        /// <summary>
        /// Removes a tag from the mask
        /// </summary>
        /// <param name="tag">The tag to remove from the mask</param>
        public void RemoveTagFromMask(string tag)
        {
            int tagIndex = Array.IndexOf(UnityEditorInternal.InternalEditorUtility.tags, tag);
            if (tagIndex != -1)
            {
                tagMask.Mask &= ~(1 << tagIndex);
            }
        }

        /// <summary>
        /// Removes an array of tags from the mask
        /// </summary>
        /// <param name="tags">The array of tags to remove from the mask</param>
        public void RemoveTagsFromMask(string[] tags)
        {
            for (int i = 0; i < tags.Length; i++)
                RemoveTagFromMask(tags[i]);
        }

        /// <summary>
        /// Removes a list of tags from the mask
        /// </summary>
        /// <param name="tags">The list of tags to remove from the mask</param>
        public void RemoveTagsFromMask(List<string> tags)
        {
            foreach (string tag in tags)
                RemoveTagFromMask(tag);
        }

        /// <summary>
        /// Toggles a tag in the mask (add if not present or remove if present)
        /// </summary>
        /// <param name="tag">The tag to toggle in the mask</param>
        public void ToggleTag(string tag)
        {
            int tagIndex = Array.IndexOf(UnityEditorInternal.InternalEditorUtility.tags, tag);
            if (tagIndex != -1)
            {
                tagMask.Mask ^= (1 << tagIndex);
            }
        }

        /// <summary>
        /// Toggles an array of tags in the mask (add if not present or remove if present)
        /// </summary>
        /// <param name="tags">The array of tags to toggle in the mask</param>
        public void ToggleTags(string[] tags)
        {
            for (int i = 0; i < tags.Length; i++)
                ToggleTag(tags[i]);
        }

        /// <summary>
        /// Toggles a list of tags in the mask (add if not present or remove if present)
        /// </summary>
        /// <param name="tags">The list of tags to toggle in the mask</param>
        public void ToggleTags(List<string> tags)
        {
            foreach (string tag in tags)
                ToggleTag(tag);
        }

        #endregion

        #region CompareTags

        /// <summary>
        /// Checks if this mask has any matching tag with another game object
        /// </summary>
        /// <param name="otherObject">The other game object to check against</param>
        /// <returns>True if any tag matches, false otherwise</returns>
        public bool CompareTag(GameObject otherObject)
        {
            return TagMaskUtils.CompareGameObjectsTag(gameObject, otherObject);
        }

        /// <summary>
        /// Checks if this mask has all matching tags with another game object
        /// </summary>
        /// <param name="otherObject">The other game object to check against</param>
        /// <returns>True if all tags match, false otherwise</returns>
        public bool CompareTags(GameObject otherObject)
        {
            return TagMaskUtils.CompareGameObjectsTags(gameObject, otherObject);
        }

        #endregion
    }
}
