using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/**
 * This is a test script to manually and visually test all the functionalities of Easy Tags
 * 
 * @author (FeroDev)
 * @version 1.0
 * @date 26/06/2024
 */

namespace EasyTags
{
    public class Test : MonoBehaviour
    {
        [Header("Activate-Deactivate Tests")]
        public bool printTagMasks;
        public bool findGameObjects;
        public bool compareGameObjects;
        public bool hasMathingFunctions;
        public bool findTransforms;
        public bool findWithoutMultipleTags;

        [Tag] public string Tag;

        [Header("Test Tag Mask")]
        public TagMask testMask;

        [Header("Test: Find Game Objects With any Tags in the Mask")]
        public TagMask tagMask;

        [Header("Test: Find Game Objects With all Tags in the Mask")]
        public TagMask tagMask1;

        [Header("Test: Compare Game Objects With any Tags in the Mask")]
        public TagMask tagMask2;
        public GameObject gameObject1_1;
        public GameObject gameObject1_2;

        [Space(10)]
        public GameObject gameObject1_3;
        public GameObject gameObject1_4;

        [Header("Test: Compare Game Objects if all Tags Matching")]
        public GameObject gameObject2_1;
        public GameObject gameObject2_2;

        [Space(10)]
        public GameObject gameObject2_3;
        public GameObject gameObject2_4;

        [Header("Test: Use Arrays and Lists of Game Objects to Compare")]
        public TagMask tagMask3;
        public GameObject[] gameObjectsArray;
        public List<GameObject> gameObjectsList;

        [Space(10)]
        public TagMask tagMask4;
        public GameObject[] gameObjectsArray2;
        public List<GameObject> gameObjectsList2;

        [Space(10)]
        [Header("Visual Test")]
        public TagMask enemyTagMask;
        public TagMask redTagMask;
        public TagMask redEnemyTags;
        public TagMask friendTagMask;
        public TagMask whiteTagMask;
        public TagMask passiveTagMask;
        public TagMask greenTagMask;
        public TagMask greenFriendTags;

        void Start()
        {
            TestAllTagMaskUtilFunctions();
        }

        void Update()
        {
            if (Input.GetKeyUp(KeyCode.T))
                TestAllTagMaskUtilFunctions();
            if (Input.GetKeyUp(KeyCode.Space))
                TestAllTagMaskUtilFunctions();
            if (Input.GetKeyUp(KeyCode.N))
            {
                SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
            }
        }

        void TestAllTagMaskUtilFunctions()
        {
            if (printTagMasks)
            {
                // Printing out the selected tags:
                Debug.LogWarning("Printing out the selected tags in tagMask: "); PrintStringArray(TagMaskUtils.GetSelectedTags(tagMask));
                Debug.LogWarning("Printing out the selected tags in tagMask1: "); PrintStringArray(TagMaskUtils.GetSelectedTags(tagMask1));
                Debug.LogWarning("Printing out the selected tags in tagMask2: "); PrintStringArray(TagMaskUtils.GetSelectedTags(tagMask2));
                Debug.LogWarning("Printing out the selected tags in tagMask3: "); PrintStringArray(TagMaskUtils.GetSelectedTags(tagMask3));
            }

            if (findGameObjects)
            {
                // Finding the first game object with any of the tag in the mask:
                Debug.LogWarning("Printing out the name of the first game object with any tag in tagMask1: "); Debug.Log(TagMaskUtils.FindGameObjectWithTag(tagMask1).name);
                // Finding the first game object with all of the tag in the mask:
                Debug.LogWarning("Printing out the name of the first game object with all of the tags in tagMask: "); Debug.Log(TagMaskUtils.FindGameObjectWithTags(tagMask).name);

                // Finding all of the game objects with only one of the tag in the mask:
                Debug.LogWarning("Printing out the names of all of the game objects with one of the tags in tagMask1: ");
                PrintGameObjectNames(TagMaskUtils.FindGameObjectsWithTag(tagMask1));
                // Finding all of the game objects with all of the tag in the mask:
                Debug.LogWarning("Printing out the names of all of the game objects with all of the tags in tagMask: ");
                PrintGameObjectNames(TagMaskUtils.FindGameObjectsWithTags(tagMask));
            }

            if (compareGameObjects)
            {
                // Comparing if a game object has any of the tags in the mask:
                Debug.LogWarning("Printing out if the game object 1_1 has any of the tags in tagMask2: "); Debug.Log(TagMaskUtils.CompareTag(gameObject1_1, tagMask2));
                // Comparing if a game object has all of the tags in the mask:
                Debug.LogWarning("Printing out if the game object 1_4 has all of the tags in tagMask2: "); Debug.Log(TagMaskUtils.CompareTags(gameObject1_4, tagMask2));

                // Comparing if 2 game objects has any tags matching:
                Debug.LogWarning("Printing out if the game object 1_1 and game object 1_2 has any matching tag: "); Debug.Log(TagMaskUtils.CompareGameObjectsTag(gameObject1_1, gameObject1_2));
                // Comparing if 2 game objects has any tags matching:
                Debug.LogWarning("Printing out if the game object 1_1 and game object 1_4 has any matching tag: "); Debug.Log(TagMaskUtils.CompareGameObjectsTag(gameObject1_1, gameObject1_4));

                // Comparing if 2 game objects has all tags matching:
                Debug.LogWarning("Printing out if the game object 2_1 and game object 2_2 has all matching tag: "); Debug.Log(TagMaskUtils.CompareGameObjectsTags(gameObject2_1, gameObject2_2));
                // Comparing if 2 game objects has all tags matching:
                Debug.LogWarning("Printing out if the game object 2_2 and game object 2_3 has all matching tag: "); Debug.Log(TagMaskUtils.CompareGameObjectsTags(gameObject2_2, gameObject2_3));
            }

            if (hasMathingFunctions)
            {
                // Comparing if any of the game objects in array has any matching tag masks:
                Debug.LogWarning("Printing out if gameObjectsArray has any matching tag masked elements: "); Debug.Log(TagMaskUtils.HasAnyMatchingTags(gameObjectsArray));
                // Comparing if any of the game objects in array has any matching tags:
                Debug.LogWarning("Printing out if gameObjectsArray has any matching tagged elements: "); Debug.Log(TagMaskUtils.HasAnyMatchingTag(gameObjectsArray));
                // Comparing if all of the elements in game object list has all matching tags:
                Debug.LogWarning("Printing out if gameObjectsList has all matching tagged elements: "); Debug.Log(TagMaskUtils.HasAllMatching(gameObjectsList));
                // Comparing if all of the elements in game object array has all matching tags:
                Debug.LogWarning("Printing out if gameObjectsArray has all matching tagged elements: "); Debug.Log(TagMaskUtils.HasAllMatching(gameObjectsArray));
                // Comparing if any of the elements in game object list has any matching tag masks:
                Debug.LogWarning("Printing out if gameObjectsList2 has any matching tag masked elements: "); Debug.Log(TagMaskUtils.HasAnyMatchingTags(gameObjectsList2));
                // Comparing if any of the elements in game object list has any matching tag:
                Debug.LogWarning("Printing out if gameObjectsList2 has any matching tagged elements: "); Debug.Log(TagMaskUtils.HasAnyMatchingTag(gameObjectsList2));
            }

            if (findTransforms)
            {
                // Finding a transform with any of the tags in the mask:
                Debug.LogWarning("Printing out the name of the first transform with any of the tags tagMask"); Debug.Log(TagMaskUtils.FindWithTag(tagMask).name);
                // Finding a transform with all of the tags in the mask:
                Debug.LogWarning("Printing out the name of the first transform with all of the tags tagMask1"); Debug.Log(TagMaskUtils.FindWithTags(tagMask1).name);
                // Finding all transforms with any of the tags in the mask:
                Debug.LogWarning("Printing out the names of all transforms with any of the tags tagMask2");
                PrintTransformNames(TagMaskUtils.FindAllWithTag(tagMask2));
                // Finding all transforms with all of the tags in the mask:
                Debug.LogWarning("Printing out the names of all transforms with all of the tags tagMask3");
                PrintTransformNames(TagMaskUtils.FindAllWithTags(tagMask3));
            }

            if (findWithoutMultipleTags)
            {
                // Finding a game object with any of the tags in the mask but without multiple tags attached:
                Debug.LogWarning("Printing out the name of the first game object with any of the tags in tagMask");
                Debug.Log(TagMaskUtils.FindGameObjectWithoutMultipleTags(tagMask).name);

                // Finding all game objects with any of the tags in the mask but without multiple tags attached:
                Debug.LogWarning("Printing out the name of all game objects with any of the tags in tagMask1");
                PrintGameObjectNames(TagMaskUtils.FindGameObjectsWithoutMultipleTags(tagMask1));

                // Finding a transform with any of the tags in the mask but without multiple tags attached:
                Debug.LogWarning("Printing out the name of the first transform with any of the tags in tagMask");
                Debug.Log(TagMaskUtils.FindWithoutMultipleTags(tagMask).name);

                // Finding all transforms with any of the tags in the mask but without multiple tags attached:
                Debug.LogWarning("Printing out the name of all transforms with any of the tags in tagMask1");
                PrintTransformNames(TagMaskUtils.FindAllWithoutMulipleTags(tagMask1));
            }

            testMask.AddTagsToMask(new string[] { Tags.Player, Tags.Respawn });
        }

        // Function to print out a string array
        private void PrintStringArray(string[] array)
        {
            foreach (string item in array)
            {
                Debug.Log(item);
            }
        }

        // Function to print out names of game objects in an array
        private void PrintGameObjectNames(GameObject[] array)
        {
            foreach (GameObject obj in array)
            {
                Debug.Log(obj.name);
            }
        }

        // Function to print out names of transforms in an array
        private void PrintTransformNames(Transform[] array)
        {
            foreach (Transform t in array)
            {
                Debug.Log(t.name);
            }
        }

        public void DisplayEnemies()
        {
            ResetCubes();
            Transform[] enemies = TagMaskUtils.FindAllWithTag(enemyTagMask);
            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i].position = new(enemies[i].position.x, 3, enemies[i].position.z);
            }
        }

        public void DisplayRed()
        {
            ResetCubes();
            Transform[] enemies = TagMaskUtils.FindAllWithTag(redTagMask);
            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i].position = new(enemies[i].position.x, 3, enemies[i].position.z);
            }
        }

        public void DisplayRedEnemy()
        {
            ResetCubes();
            Transform[] enemies = TagMaskUtils.FindAllWithTags(redEnemyTags);
            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i].position = new(enemies[i].position.x, 3, enemies[i].position.z);
            }
        }

        public void DisplayFriends()
        {
            ResetCubes();
            Transform[] enemies = TagMaskUtils.FindAllWithTag(friendTagMask);
            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i].position = new(enemies[i].position.x, 3, enemies[i].position.z);
            }
        }

        public void DisplayWhite()
        {
            ResetCubes();
            Transform[] enemies = TagMaskUtils.FindAllWithTag(whiteTagMask);
            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i].position = new(enemies[i].position.x, 3, enemies[i].position.z);
            }
        }

        public void DisplayPassives()
        {
            ResetCubes();
            Transform[] enemies = TagMaskUtils.FindAllWithTag(passiveTagMask);
            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i].position = new(enemies[i].position.x, 3, enemies[i].position.z);
            }
        }

        public void DisplayGreen()
        {
            ResetCubes();
            Transform[] enemies = TagMaskUtils.FindAllWithTag(greenTagMask);
            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i].position = new(enemies[i].position.x, 3, enemies[i].position.z);
            }
        }

        public void DisplayGreenFriend()
        {
            ResetCubes();
            Transform[] enemies = TagMaskUtils.FindAllWithTags(greenFriendTags);
            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i].position = new(enemies[i].position.x, 3, enemies[i].position.z);
            }
        }

        public void ResetCubes()
        {
            MultipleTags[] all = FindObjectsByType<MultipleTags>(FindObjectsSortMode.None);
            for (int i = 0; i < all.Length; i++)
            {
                all[i].transform.position = new(all[i].transform.position.x, 0, all[i].transform.position.z);
            }
        }
    }
}
