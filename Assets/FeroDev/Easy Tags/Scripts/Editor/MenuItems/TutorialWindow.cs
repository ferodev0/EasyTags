using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/**
 * TutorialWindow is a window that shows some information and simple ways to start using the asset.
 * It has a section for some information.
 * It has a section to help you start using the contents of the asset.
 * 
 * @author (FeroDev)
 * @version 1.0
 * @date 06/09/2026
 */

namespace EasyTags
{
    /// <summary>
    /// Tutorial window for Easy Tags inside editor
    /// Open via Tools -> Easy Tags -> Tutorial.
    /// </summary>

    public class EasyTagsTutorialWindow : EditorWindow
    {
        // Paths to some sources
        private const string ImagesFolder = "Assets/FeroDev/Easy Tags/Icons/Images";
        private const string PDFDocumentationPath = "Assets/FeroDev/Easy Tags/Documentation/Documentation.pdf";
        private const string OnlineDocumentationLink = "https://ferodev0.github.io/easy-tags-documentation/";

        private static readonly Color AccentColor = Color.cyan; // matches TagManagerWindow's cyan header/tag accent

        private Vector2 _scrollPosition;
        private GUIStyle _titleStyle;
        private GUIStyle _sectionHeaderStyle;
        private GUIStyle _sectionBoxStyle;
        private GUIStyle _bodyStyle;
        private GUIStyle _bulletStyle;
        private GUIStyle _codeStyle;
        private GUIStyle _buttonStyle;
        private bool _stylesInitialized;

        private readonly Dictionary<string, bool> _foldouts = new Dictionary<string, bool>();

        [MenuItem("Tools/Easy Tags/Documentation")]
        public static void ShowWindow()
        {
            // To setup and show the window
            var window = GetWindow<EasyTagsTutorialWindow>("Easy Tags Tutorial");
            window.minSize = new Vector2(800, 600);

            Rect rect = window.position;
            rect.width = 900;
            rect.height = 700;
            window.position = rect;
        }

        // Setting up all the styles
        private void InitStyles()
        {
            if (_stylesInitialized) return;

            _titleStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 24,
                fixedHeight = 0,
                wordWrap = true,
                margin = new RectOffset(10, 10, 14, 12)
            };
            _titleStyle.normal.textColor = AccentColor; // brand accent, matching TagManagerWindow's header color

            _sectionHeaderStyle = new GUIStyle(EditorStyles.foldout)
            {
                fontSize = 16,
                fontStyle = FontStyle.Bold,
                fixedHeight = 0,
                wordWrap = false,
                margin = new RectOffset(0, 0, 4, 4),
                padding = new RectOffset(EditorStyles.foldout.padding.left, 4, 6, 6)
            };
            _sectionHeaderStyle.normal.textColor = AccentColor;
            _sectionHeaderStyle.onNormal.textColor = AccentColor;
            _sectionHeaderStyle.focused.textColor = AccentColor;
            _sectionHeaderStyle.active.textColor = AccentColor;
            _sectionHeaderStyle.hover.textColor = AccentColor;

            _sectionBoxStyle = new GUIStyle(EditorStyles.helpBox)
            {
                padding = new RectOffset(14, 14, 10, 12),
                margin = new RectOffset(0, 0, 4, 10)
            };

            _bodyStyle = new GUIStyle(EditorStyles.wordWrappedLabel)
            {
                fontSize = 14,
                wordWrap = true
            };

            _bulletStyle = new GUIStyle(EditorStyles.wordWrappedLabel)
            {
                fontSize = 14,
                wordWrap = true,
                padding = new RectOffset(14, 4, 3, 3)
            };

            _codeStyle = new GUIStyle(EditorStyles.textArea)
            {
                wordWrap = true,
                fontSize = 13,
                font = GetMonospaceFont()
            };

            // Mirrors TagManagerWindow's neutral buttonStyle: same skin, padding, and margin,
            // so buttons across both Easy Tags windows read as part of the same toolset.
            _buttonStyle = new GUIStyle(GUI.skin.button);
            _buttonStyle.normal.textColor = Color.white;
            _buttonStyle.hover.textColor = Color.white;
            _buttonStyle.active.textColor = Color.white;
            _buttonStyle.padding = new RectOffset(10, 10, 3, 3);
            _buttonStyle.margin = new RectOffset(10, 10, 0, 0);

            _stylesInitialized = true;
        }

        // Making sure that the fonts are working
        private static Font _monospaceFont;
        private static Font GetMonospaceFont()
        {
            if (_monospaceFont != null) return _monospaceFont;
            _monospaceFont = Font.CreateDynamicFontFromOSFont(
                new[] { "Consolas", "Courier New", "Menlo", "Courier" }, 12);
            return _monospaceFont;
        }

        // ---- Main GUI function ----
        private void OnGUI()
        {
            InitStyles();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            EditorGUILayout.Space(6);

            DrawTitleSection();
            EditorGUILayout.Space(10);
            DrawFeatureList();

            EditorGUILayout.Space(16);
            EditorGUILayout.LabelField("How To Use", _titleStyle);
            EditorGUILayout.Space(2);

            DrawTagManagerSection();
            DrawGenerateAccessibleTagsSection();
            DrawMultipleTagsComponentSection();
            DrawTagAttributeSection();
            DrawExampleUsagesSection();
            DrawTagMaskUtilsSection();

            EditorGUILayout.Space(16);
            DrawFooter();
            EditorGUILayout.Space(10);

            EditorGUILayout.EndScrollView();
        }

        // ---- Header ----

        private void DrawTitleSection()
        {
            EditorGUILayout.BeginHorizontal();

            Texture2D logo = LoadImage("icon-big.png");
            if (logo != null)
            {
                GUILayout.Label(logo, GUILayout.Width(48), GUILayout.Height(48));
            }

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Easy Tags", _titleStyle);
            EditorGUILayout.LabelField("v1.0  -  Improved Tag System for Unity", EditorStyles.miniLabel);
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(8);

            EditorGUILayout.LabelField(
                "Easy Tags is an extension tool for the Unity tag system to let you better utilize tags in your game. " +
                "Easy Tags has an extensive set of functionalities to help you better utilize tags in your game. " +
                "The main functionalities of Easy Tags are as follows:",
                _bodyStyle);
        }

        private void DrawFeatureList()
        {
            string[] features =
            {
                "A custom attribute for strings to display a dropdown for tags in the inspector.",
                "A custom property for tags to let you choose multiple tags as a mask.",
                "A custom component to let you assign multiple tags to your game objects.",
                "Accessible tags to let you access tags by their defined names instead of writing them by hand.",
                "A better tag management window for adding and deleting tags.",
                "Extensive utility functions to help you jump to development right away.",
                "An optimized cache based search system for the objects that has MultipleTags."
            };

            DrawBulletList(features);
        }

        // ---- How To Use sections ----

        private void DrawTagManagerSection()
        {
            DrawSection("Tag Manager Window", () =>
            {
                EditorGUILayout.LabelField(
                    "To open the Tag Manager window, go to Tools -> Easy Tags -> Tag Manager", _bodyStyle);
                DrawImage("easytags.png");

                EditorGUILayout.LabelField("It will open this window:", _bodyStyle);
                DrawImage("tagmanager.png");
            });
        }

        private void DrawGenerateAccessibleTagsSection()
        {
            DrawSection("Generate Accessible Tags", () =>
            {
                EditorGUILayout.LabelField(
                    "Easy Tags automatically generates and updates the accessible tags, but in case it doesn't, " +
                    "go to Tools -> Easy Tags -> Generate Accessible Tags:", _bodyStyle);
                DrawImage("easytags.png");

                DrawCodeSnippet(
                    "// Add the namespace\n" +
                    "using UnityEngine;\n" +
                    "using EasyTags;");

                DrawCodeSnippet(
                    "// Then you can use the Tags class to directly use the defined tags\n" +
                    "void Start()\n" +
                    "{\n" +
                    "    GameObject gameObject = GameObject.FindGameObjectWithTag(Tags.Player);\n" +
                    "}");

                EditorGUILayout.LabelField("[Tag] attribute in inspector:", _bodyStyle);
                DrawImage("tagattribute.png");

                EditorGUILayout.LabelField("TagMask property in inspector:", _bodyStyle);
                DrawImage("tagmask.png");
            });
        }

        private void DrawMultipleTagsComponentSection()
        {
            DrawSection("Multiple Tags Component", () =>
            {
                EditorGUILayout.LabelField(
                    "To add Multiple Tags component to your game objects, either use the traditional method, " +
                    "or select your game objects and go to Tools -> Easy Tags -> Add Multiple Tags to Selected:", _bodyStyle);
                DrawImage("addmultipletags.png");

                EditorGUILayout.LabelField("Multiple Tags Component:", _bodyStyle);
                DrawImage("multipletags.png");

                DrawBulletList(new[]
                {
                    "Add multiple tags using the Tag Mask.",
                    "Click selected tags to remove them.",
                    "Open the Tag Manager."
                });
            });
        }

        private void DrawTagAttributeSection()
        {
            DrawSection("[Tag] Attribute and TagMask Property", () =>
            {
                EditorGUILayout.LabelField(
                    "You can use the custom [Tag] attribute to use tag dropdowns and custom TagMask property " +
                    "to use masks for tags in your scripts:", _bodyStyle);

                DrawCodeSnippet(
                    "// Add the namespace\n" +
                    "using UnityEngine;\n" +
                    "using EasyTags;");

                DrawCodeSnippet(
                    "// Then you can use [Tag] attribute for tag dropdowns\n" +
                    "[Tag] public string tag;\n\n" +
                    "// And TagMask property for masks for tags\n" +
                    "public TagMask tagMask;");
            });
        }

        private void DrawExampleUsagesSection()
        {
            DrawSection("TagMask and MultipleTags Example Usages", () =>
            {
                EditorGUILayout.LabelField(
                    "TagMask and MultipleTags has some built-in functions that you can easily use. " +
                    "Here are some example usages of these functions:", _bodyStyle);

                DrawCodeSnippet(
                    "// Add the namespace\n" +
                    "using UnityEngine;\n" +
                    "using EasyTags;");

                DrawCodeSnippet(
                    "// Adding or removing tags from a mask\n" +
                    "public TagMask tagMask;\n\n" +
                    "void Start()\n" +
                    "{\n" +
                    "    tagMask = new TagMask(new string[] {Tags.Enemy, Tags.Player});\n" +
                    "    tagMask.AddTagsToMask(new string[] {Tags.Knight, Tags.Archer});\n" +
                    "    tagMask.RemoveTagFromMask(Tags.Player);\n" +
                    "}");

                DrawCodeSnippet(
                    "// Collision handling example using MultipleTags\n" +
                    "private void OnCollisionEnter(Collision collision)\n" +
                    "{\n" +
                    "    // or use MatchesWith(tagMask) based on your use case\n" +
                    "    if (collision.gameObject.GetComponent<MultipleTags>().HasTags(tagMask))\n" +
                    "    {\n" +
                    "        TakeDamage(10);\n" +
                    "    }\n" +
                    "}");

                DrawCodeSnippet(
                    "// MultipleTags uses TagMask, so something like this is also possible\n" +
                    "string[] selectedTags = gameObject.GetComponent<MultipleTags>().TagMask.GetSelectedTags();\n\n" +
                    "// Although you can directly use MultipleTags for that\n" +
                    "string[] selectedTags = gameObject.GetComponent<MultipleTags>().GetSelectedTags();");
            });
        }

        private void DrawTagMaskUtilsSection()
        {
            DrawSection("TagMaskUtils Class", () =>
            {
                EditorGUILayout.LabelField(
                    "TagMaskUtils is a static utility class that has a lot of built-in function to use with " +
                    "MultipleTags and TagMask. Here is a summary of the things it can do:", _bodyStyle);

                DrawBulletList(new[]
                {
                    "Caches game objects with MultipleTags.",
                    "Uses the cache to efficiently search for game objects using TagMask.",
                    "Compares game objects with TagMask.",
                    "Checks a list or an array for similarly tagged (MultipleTags) game objects using TagMask.",
                    "Uses the cache to efficiently search for transforms using TagMask.",
                    "Does a traditional search for traditionally tagged objects (without MultipleTags attached) using TagMask.",
                    "Modifies a TagMask using a tag or a list or array of tags."
                });

                EditorGUILayout.LabelField(
                    "Here are example usages for different types of functions in TagMaskUtils:", _bodyStyle);

                DrawCodeSnippet(
                    "// Add the namespace\n" +
                    "using UnityEngine;\n" +
                    "using EasyTags;");

                DrawCodeSnippet(
                    "// Prepare your TagMask either through code or in the inspector\n" +
                    "public TagMask tagMask;\n\n" +
                    "void Start()\n" +
                    "{\n" +
                    "    // Find specifically tagged game objects that have MultipleTags attached\n" +
                    "    GameObject[] archerEnemies = TagMaskUtils.FindGameObjectsWithTags(tagMask);\n\n" +
                    "    // Find specifically tagged transforms that have MultipleTags attached\n" +
                    "    Transform[] archerEnemiesTransforms = TagMaskUtils.FindAllWithTags(tagMask);\n" +
                    "}");

                DrawCodeSnippet(
                    "// You can even use TagMask to search for traditionally tagged game objects\n" +
                    "// This function finds game objects using more than one tag with TagMask\n" +
                    "// And it works for game objects that doesn't have MultipleTags\n" +
                    "GameObject[] allEnemies = TagMaskUtils.FindGameObjectsWithoutMultipleTags(tagMask);");

                DrawCodeSnippet(
                    "// Collision handling example using TagMaskUtils\n" +
                    "private void OnCollisionEnter(Collision collision)\n" +
                    "{\n" +
                    "    if (TagMaskUtils.CompareGameObjectsTag(this.gameObject, collision.gameObject))\n" +
                    "    {\n" +
                    "        // Do something if two MultipleTags game objects have any matching tags\n" +
                    "    }\n" +
                    "}");

                DrawCodeSnippet(
                    "// You can also check if a list or array of GameObjects has similar MultipleTags\n" +
                    "public GameObject[] gameObjects;\n\n" +
                    "void Start()\n" +
                    "{\n" +
                    "    if (TagMaskUtils.HasAllMatching(gameObjects))\n" +
                    "    {\n" +
                    "        // Do something if all of the GameObjects have the same tags in their MultipleTags\n" +
                    "    }\n" +
                    "}");
            });
        }

        private void DrawFooter()
        {
            EditorGUILayout.Space(4);

            if (!string.IsNullOrEmpty(PDFDocumentationPath) && File.Exists(PDFDocumentationPath))
            {
                if (GUILayout.Button("Open PDF Documentation", _buttonStyle))
                {
                    EditorUtility.OpenWithDefaultApp(PDFDocumentationPath);
                }
                EditorGUILayout.Space(4);
            }

            EditorGUILayout.Space(4);

            if (!string.IsNullOrEmpty(OnlineDocumentationLink))
            {
                if (GUILayout.Button("Open Online Documentation", _buttonStyle))
                {
                    Application.OpenURL(OnlineDocumentationLink);
                }
                EditorGUILayout.Space(4);
            }

            EditorGUILayout.LabelField("Easy Tags - Improved Tag System for Unity", EditorStyles.centeredGreyMiniLabel);
        }

        // ---- Shared helpers ----

        /// <summary>
        /// Draws a foldout toggle for the given section title and returns whether it's expanded.
        /// Callers wrap their whole section (foldout + content)
        /// </summary>

        private bool DrawFoldout(string title)
        {
            if (!_foldouts.ContainsKey(title))
                _foldouts[title] = false;

            bool isOpen = _foldouts[title];

            // Reserve a rect that spans the full available width of the row, not just
            // however wide the label text happens to be.
            Rect rect = GUILayoutUtility.GetRect(
                new GUIContent(title), _sectionHeaderStyle, GUILayout.ExpandWidth(true));

            // Handle the click ourselves across the entire rect first, and consume the
            // event so the Foldout call below (used purely for drawing) doesn't also
            // try to react to the same click and double-toggle.
            if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
            {
                isOpen = !isOpen;
                GUI.changed = true;
                Event.current.Use();
            }

            EditorGUI.Foldout(rect, isOpen, title, true, _sectionHeaderStyle);

            _foldouts[title] = isOpen;

            if (isOpen)
                EditorGUILayout.Space(6);

            return isOpen;
        }

        /// <summary>
        /// Wraps a "How To Use" section in a boxed vertical group and draws
        /// its content only while expanded
        /// </summary>
        private void DrawSection(string title, System.Action drawContent)
        {
            EditorGUILayout.BeginVertical(_sectionBoxStyle);

            if (DrawFoldout(title))
            {
                drawContent();
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(6);
        }

        private void DrawBulletList(IEnumerable<string> items)
        {
            foreach (string item in items)
            {
                EditorGUILayout.LabelField("•  " + item, _bulletStyle);
            }
        }

        private void DrawCodeSnippet(string code)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Copy", _buttonStyle, GUILayout.Width(70)))
            {
                GUIUtility.systemCopyBuffer = code;
                ShowNotification(new GUIContent("Copied to clipboard"));
            }
            EditorGUILayout.EndHorizontal();

            float width = position.width - 24f;
            float height = _codeStyle.CalcHeight(new GUIContent(code), width);
            EditorGUILayout.SelectableLabel(code, _codeStyle, GUILayout.Height(height));
            EditorGUILayout.Space(6);
        }

        private Texture2D LoadImage(string fileName)
        {
            string path = Path.Combine(ImagesFolder, fileName).Replace("\\", "/");
            return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }

        private void DrawImage(string fileName)
        {
            Texture2D texture = LoadImage(fileName);

            if (texture == null)
            {
                EditorGUILayout.HelpBox($"Image not found: {ImagesFolder}/{fileName}", MessageType.None);
                return;
            }

            float maxWidth = position.width - 30f;
            float aspect = (float)texture.height / texture.width;
            float width = Mathf.Min(maxWidth, texture.width);
            float height = width * aspect;

            Rect rect = GUILayoutUtility.GetRect(width, height, GUILayout.ExpandWidth(false));
            GUI.DrawTexture(rect, texture, ScaleMode.ScaleToFit);
            EditorGUILayout.Space(4);
        }
    }
}