using UnityEngine;
using System.Collections.Generic;
using System;

/**
 * TagMask struct lets you to select multiple tags as a bitmask
 * This struct includes the necessary code to let you use TagMask independently and without the help of MultipleTags
 * 
 * @author (FeroDev)
 * @version 1.0
 * @date 26/06/2024
 */

namespace EasyTags
{
    [System.Serializable]
    public struct TagMask
    {
        [SerializeField]
        private int mask;

        /// <summary>
        /// Get and set the mask value
        /// </summary>
        public int Mask
        {
            get { return mask; }
            set { mask = value; }
        }

        #region Constructors

        /// <summary>
        /// Initialize TagMask with a mask
        /// </summary>
        /// <param name="mask">The mask value to initialize with</param>
        public TagMask(int mask)
        {
            this.mask = mask;
        }

        /// <summary>
        /// Initialize TagMask with a tag
        /// </summary>
        /// <param name="tag">The tag to initialize with</param>
        public TagMask(string tag)
        {
            mask = 0;
            AddTagToMask(tag);
        }

        /// <summary>
        /// Initialize TagMask with an array of tags
        /// </summary>
        /// <param name="tags">The array of tags to initialize with</param>
        public TagMask(string[] tags)
        {
            mask = 0;
            for (int i = 0; i < tags.Length; i++)
                AddTagToMask(tags[i]);
        }

        /// <summary>
        /// Initialize TagMask with a list of tags
        /// </summary>
        /// <param name="tags">The list of tags to initialize with</param>
        public TagMask(List<string> tags)
        {
            mask = 0;
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
                if ((mask & (1 << i)) != 0)
                {
                    selectedTagsList.Add(allTags[i]);
                }
            }

            return selectedTagsList.ToArray();
        }

        #region HasTag

        /// <summary>
        /// Checks if the mask has the integer corresponding to a tag
        /// </summary>
        /// <param name="tagIndex">The index of the tag to check</param>
        /// <returns>True if the tag is in the mask, false otherwise</returns>
        public bool HasTag(int tagIndex)
        {
            return (mask & (1 << tagIndex)) != 0;
        }

        /// <summary>
        /// Checks if the mask has a tag
        /// </summary>
        /// <param name="tag">The tag to check</param>
        /// <returns>True if the tag is in the mask, false otherwise</returns>
        public bool HasTag(string tag)
        {
            int tagIndex = Array.IndexOf(UnityEditorInternal.InternalEditorUtility.tags, tag);
            return HasTag(tagIndex);
        }

        #endregion

        #region HasTags

        /// <summary>
        /// Checks if the mask has all the integers correcponding to all the tags selected in a mask
        /// </summary>
        /// <param name="mask">The mask to check against</param>
        /// <returns>True if the masks has all the tags, false otherwise</returns>
        public bool HasTags(int mask)
        {
            string[] tags = TagMaskUtils.GetSelectedTags(mask);
            for (int i = 0; i < tags.Length; i++)
            {
                if (!HasTag(tags[i]))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if the mask has all the tags in the mask from an array of strings
        /// </summary>
        /// <param name="tags">The array of tags to check</param>
        /// <returns>True if all tags are in the mask, false otherwise</returns>
        public bool HasTags(string[] tags)
        {
            for (int i = 0; i < tags.Length; i++)
            {
                int tagIndex = Array.IndexOf(UnityEditorInternal.InternalEditorUtility.tags, tags[i]);
                if (!HasTag(tagIndex))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if the mask has all the tags in the mask from a list of strings
        /// </summary>
        /// <param name="tags">The list of tags to check</param>
        /// <returns>True if all tags are in the mask, false otherwise</returns>
        public bool HasTags(List<string> tags)
        {
            foreach (string tag in tags)
            {
                int tagIndex = Array.IndexOf(UnityEditorInternal.InternalEditorUtility.tags, tag);
                if (!HasTag(tagIndex))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if the mask has all the tags from another mask
        /// </summary>
        /// <param name="mask">The mask to check</param>
        /// <returns>True if all tags are in the mask, false otherwise</returns>
        public bool HasTags(TagMask mask)
        {
            return HasTags(mask.Mask);
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
            return mask == Mask;
        }

        /// <summary>
        /// Checks if the mask matches with another mask (no different tags are allowed)
        /// </summary>
        /// <param name="mask"></param>
        /// <returns>True if the masks match, false otherwise</returns>
        public bool MathesWith(TagMask mask)
        {
            return mask.Mask == Mask;
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
                mask |= (1 << tagIndex);
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
                mask &= ~(1 << tagIndex);
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
                mask ^= (1 << tagIndex);
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
    }
}
