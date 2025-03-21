#if UNITY_EDITOR
using Object = UnityEngine.Object;
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Envi.VibrantIcons
{
    /// <summary>
    /// Custom Editor Window for customizing script icons in the Unity Editor.
    /// </summary>
    public class VibrantIconsWindow : EditorWindow
    {
        #region Private Variables

        private TargetList targetList;
        private IconManager iconManager;
        private TextureLoader textureLoader;
        private float textureScrollbarWidth = 16f;
        private Vector2 scrollPosition;
        private int displayedObjectsCount = 0;
        private const int MinDisplayedObjects = 0;
        private const string EditorPrefsKey = "VibrantIconsPrefs";

        #endregion

        #region Properties

        private int DisplayedObjectsCount
        {
            get => displayedObjectsCount;
            set
            {
                displayedObjectsCount = value;
                targetList.UpdateDisplayedObjectsCount(displayedObjectsCount);
            }
        }

        #endregion

        #region MenuItem

        /// <summary>
        /// MenuItem attribute to create the window from the Unity Editor menu.
        /// </summary>
        [MenuItem("Tools/Envi/Vibrant Icons")]
        private static void GetWindow()
        {
            VibrantIconsWindow window = GetWindow<VibrantIconsWindow>();
            window.position = new Rect(Screen.width / 2F, Screen.height / 2F, 425F, 335F);
            window.titleContent = new GUIContent("Customize Your Script Icons");
            window.ShowUtility();
        }

        #endregion

        #region Unity Callbacks

        /// <summary>
        /// Called when the window is enabled.
        /// </summary>
        private void OnEnable()
        {
            targetList = new TargetList();
            iconManager = new IconManager();
            iconManager.ColorChanged += OnColorChanged;
            textureLoader = new TextureLoader();
            minSize = new Vector2(425f, 335f);
            string savedColor = EditorPrefs.GetString(EditorPrefsKey + "_SelectedColor", IconColor.All.ToString());
            iconManager.SelectedColor = (IconColor)Enum.Parse(typeof(IconColor), savedColor);

            LoadPreferences();
            RefreshAvailableTextures();
        }

        /// <summary>
        /// Called when the window is disabled.
        /// </summary>
        private void OnDisable()
        {
            iconManager.ColorChanged -= OnColorChanged;
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles the color change event and refreshes available textures.
        /// </summary>
        /// <param name="newColor">The new selected color.</param>
        private void OnColorChanged(IconColor newColor)
        {
            RefreshAvailableTextures();
            EditorPrefs.SetString(EditorPrefsKey + "_SelectedColor", newColor.ToString());
        }

        /// <summary>
        /// Handles drag-and-drop functionality for adding targets.
        /// </summary>
        /// <param name="evt">The current event.</param>
        private void HandleDragAndDrop(Event evt)
        {
            if (evt.type == EventType.DragUpdated || evt.type == EventType.DragPerform)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();

                    Object[] draggedObjects = DragAndDrop.objectReferences;

                    // Filter out non-accepted files and folders
                    draggedObjects = draggedObjects
                        .Where(obj => obj != null && (textureLoader.IsAcceptedFile(obj) || Directory.Exists(AssetDatabase.GetAssetPath(obj))))
                        .ToArray();

                    foreach (Object obj in draggedObjects)
                    {
                        if (obj is DefaultAsset folder)
                        {
                            string folderPath = AssetDatabase.GetAssetPath(folder);
                            Object[] csFiles = Directory.GetFiles(folderPath, "*.cs", SearchOption.TopDirectoryOnly)
                                .Select(filePath => AssetDatabase.LoadAssetAtPath<MonoScript>(filePath))
                                .Where(monoScript => monoScript != null)
                                .Distinct()
                                .Where(script => !targetList.Contains(script))
                                .ToArray();

                            targetList.AddTargets(csFiles);
                        }
                        else if (textureLoader.IsAcceptedFile(obj) && !targetList.Contains(obj))
                        {
                            // If a file is dropped, add it to the list
                            targetList.AddTargets(new Object[] { obj });
                        }
                    }

                    evt.Use();
                }

            }
        }

        #endregion

        #region GUI

        /// <summary>
        /// Draws the GUI for the window.
        /// </summary>
        private void OnGUI()
        {
            Event evt = Event.current;

            HandleDragAndDrop(evt);

            EditorGUI.BeginChangeCheck();

            DrawSelectedObjectsSection();
            DrawClearListAndRestoreDefaultIconsButtons();
            DrawAvailableIconsSection();

            EditorGUI.EndChangeCheck();

            SavePreferences();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Refreshes the available textures based on dark or light mode.
        /// </summary>
        private void RefreshAvailableTextures()
        {
            iconManager.ClearAvailableTextures();
            string scriptFolderPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this)));
            string parentFolder = Directory.GetParent(scriptFolderPath).FullName;
            int index = parentFolder.IndexOf("Assets", StringComparison.OrdinalIgnoreCase);

            if (index >= 0)
                parentFolder = parentFolder.Substring(index);

            string texturesFolder = Path.Combine(parentFolder, "Textures");
            string darkFolder = Path.Combine(texturesFolder, "Dark");
            string lightFolder = Path.Combine(texturesFolder, "Light");

            if (textureLoader.IsFolderAvailable(darkFolder) && textureLoader.IsFolderAvailable(lightFolder))
            {
                string modeFolder = iconManager.IsDarkMode ? darkFolder : lightFolder;
                textureLoader.LoadTexturesFromFolder(modeFolder, iconManager.SelectedColor, iconManager.AddTexture);
            }
            else
                textureLoader.LoadTexturesFromFolder(texturesFolder, iconManager.SelectedColor, iconManager.AddTexture);
        }

        /// <summary>
        /// Draws the section for selected objects.
        /// </summary>
        private void DrawSelectedObjectsSection()
        {
            EditorGUILayout.BeginHorizontal();

            // Label for displaying the total number of objects
            EditorGUILayout.LabelField($"Total: {targetList.Count}               Displayed Objects: ", Styles.BoldLabel);

            // Slider for selecting the number of displayed objects
            int totalObjectsCount = targetList.Count;
            int newDisplayedObjectsCount = EditorGUILayout.IntSlider(displayedObjectsCount, MinDisplayedObjects, totalObjectsCount, GUILayout.ExpandWidth(true));

            if (newDisplayedObjectsCount != displayedObjectsCount)
            {
                displayedObjectsCount = newDisplayedObjectsCount;
                targetList.UpdateDisplayedObjectsCount(displayedObjectsCount);
            }

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("+", GUILayout.Width(20)))
            {
                string folderPath = EditorUtility.OpenFolderPanel("Select Folder Containing Script Files", "Assets", "");

                if (!string.IsNullOrEmpty(folderPath))
                {
                    string[] csFiles = Directory.GetFiles(folderPath, "*.cs");

                    targetList.AddTargets(csFiles.Select(filePath => textureLoader.FindMonoScriptAtPath(filePath)).Where(script => script != null));
                }
            }

            if (GUILayout.Button("...", GUILayout.Width(20)))
                ShowObjectListPopup();

            EditorGUILayout.EndHorizontal();

            targetList.Draw();
        }

        /// <summary>
        /// Checks if any target in the target list has a custom icon set.
        /// </summary>
        /// <returns>True if any target has a custom icon set, otherwise false.</returns>
        private bool AnyTargetHasCustomIcon()
        {
            return iconManager.AnyTargetHasCustomIcon(targetList);
        }

        /// <summary>
        /// Draws buttons to clear the list and restore default icons.
        /// </summary>
        private void DrawClearListAndRestoreDefaultIconsButtons()
        {
            GUILayout.Space(5f);

            Rect layoutRect = EditorGUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();

                // Calculate total available width for buttons
                float totalAvailableWidth = layoutRect.width - 20f;

                // Calculate dynamic button width based on available space
                float buttonWidth = (totalAvailableWidth - 10f) / 2f;

                GUI.enabled = targetList.Count > 0;

                // Draw Clear List button
                if (GUI.Button(new Rect(layoutRect.x + 10f, layoutRect.y, buttonWidth, EditorGUIUtility.singleLineHeight), "Clear List"))
                {
                    targetList.Clear();
                    Event.current.Use();
                }

                GUI.enabled = targetList.Count > 0 && AnyTargetHasCustomIcon();

                // Draw Restore Default Icons button
                if (GUI.Button(new Rect(layoutRect.x + buttonWidth + 20f, layoutRect.y, buttonWidth, EditorGUIUtility.singleLineHeight), "Restore Default Icons"))
                {
                    iconManager.RestoreDefaultIcons(targetList);
                    Event.current.Use();
                }

                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(25f);
        }

        /// <summary>
        /// Draws the section for available icons.
        /// </summary>
        private void DrawAvailableIconsSection()
        {
            GUILayout.BeginHorizontal();
            GUI.enabled = true;
            EditorGUILayout.LabelField("Available Icons", Styles.BoldLabel);

            GUILayout.Label("Color:");
            iconManager.SelectedColor = (IconColor)EditorGUILayout.EnumPopup(iconManager.SelectedColor, GUILayout.Width(80));

            GUILayout.FlexibleSpace();

            if (GUILayout.Button(iconManager.IsDarkMode ? "Dark" : "Light", GUILayout.Width(50), GUILayout.Height(20)))
            {
                iconManager.ToggleDarkMode();
                RefreshAvailableTextures();
            }

            GUILayout.EndHorizontal();

            DrawAvailableTextures();
        }

        /// <summary>
        /// Draws the available textures in the GUI.
        /// </summary>
        private void DrawAvailableTextures()
        {
            int iconSize = 60;
            int availableWidth = (int)(position.width - textureScrollbarWidth);
            int texturesPerRow = Mathf.Max(1, availableWidth / iconSize);
            int totalRows = Mathf.CeilToInt((float)iconManager.AvailableTextures.Count / texturesPerRow);
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, false, true, GUILayout.Width(position.width - textureScrollbarWidth), GUILayout.ExpandHeight(true));

            for (int i = 0; i < totalRows; i++)
            {
                EditorGUILayout.BeginHorizontal();

                int startIndex = i * texturesPerRow;
                int endIndex = Mathf.Min(startIndex + texturesPerRow, iconManager.AvailableTextures.Count);

                for (int j = startIndex; j < endIndex; j++)
                {
                    GUILayout.BeginVertical(GUILayout.Width(iconSize));
                    GUILayout.Box(iconManager.AvailableTextures[j], GUILayout.Width(iconSize), GUILayout.Height(iconSize));

                    if (Event.current.type == EventType.MouseDown && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
                        iconManager.SetIconForTargets(targetList, iconManager.AvailableTextures[j]);

                    GUILayout.EndVertical();
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// Shows a context menu to remove selected objects.
        /// </summary>
        private void ShowObjectListPopup()
        {
            GenericMenu menu = new GenericMenu();

            foreach (var target in targetList)
            {
                string label = $"Remove {target.name} ({target.GetType().Name})";
                menu.AddItem(new GUIContent(label), false, () => targetList.RemoveTarget(target));
            }

            menu.ShowAsContext();
        }

        #endregion

        #region EditorPrefs

        /// <summary>
        /// Loads preferences from EditorPrefs.
        /// </summary>
        private void LoadPreferences()
        {
            string preferencesJson = EditorPrefs.GetString(EditorPrefsKey, string.Empty);

            if (!string.IsNullOrEmpty(preferencesJson))
            {
                try
                {
                    Preferences preferencesData = JsonUtility.FromJson<Preferences>(preferencesJson);

                    iconManager.IsDarkMode = preferencesData.IsDarkMode;
                    targetList.AddTargets(preferencesData.Targets);
                    DisplayedObjectsCount = preferencesData.DisplayedObjectsCount;
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to load preferences: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Saves preferences to EditorPrefs.
        /// </summary>
        private void SavePreferences()
        {
            try
            {
                Preferences preferencesData = new Preferences
                {
                    IsDarkMode = iconManager.IsDarkMode,
                    Targets = targetList.ToList(),
                    DisplayedObjectsCount = DisplayedObjectsCount
                };

                string preferencesJson = JsonUtility.ToJson(preferencesData);

                EditorPrefs.SetString(EditorPrefsKey, preferencesJson);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to save preferences: {ex.Message}");
            }
        }

        #endregion
    }
}
#endif