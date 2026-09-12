using UnityEditor;
using UnityEngine;

/**
 * EasyTagsFirstRun makes sure that the tutorial window opens on first import
 * 
 * @author (FeroDev)
 * @version 1.0
 * @date 06/09/2026
 */

namespace EasyTags
{
    /// <summary>
    /// Automatically opens the Easy Tags tutorial window the first time this package
    /// is imported into a project.
    /// </summary>
    [InitializeOnLoad]
    public static class EasyTagsFirstRun
    {
        private static readonly string FirstRunKey =
            "EasyTags_FirstRunShown_" + Application.dataPath.GetHashCode();

        static EasyTagsFirstRun()
        {
            if (EditorPrefs.GetBool(FirstRunKey, false))
                return;

            EditorPrefs.SetBool(FirstRunKey, true);
            EditorApplication.delayCall += EasyTagsTutorialWindow.ShowWindow;
        }

        //[MenuItem("Tools/Easy Tags/Reset First-Run Flag")]
        private static void ResetFirstRunFlag()
        {
            EditorPrefs.DeleteKey(FirstRunKey);
            Debug.Log("Easy Tags: first-run flag reset. The tutorial will open again next time scripts recompile or the Editor restarts.");
        }
    }
}