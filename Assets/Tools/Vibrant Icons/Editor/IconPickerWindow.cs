#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Envi.VibrantIcons
{
    /// <summary>
    /// Custom editor window for selecting icons from the Vibrant Icons package
    /// with name, theme, and color filtering capabilities.
    /// </summary>
    public class IconPickerWindow : EditorWindow
    {
        #region Enums

        public enum FilterTheme { All, Light, Dark }
        public enum FilterColor { All, Gray, White, Black, Red, Pink, Orange, Brown, Yellow, Green, Purple, Blue }

        #endregion

        #region Fields

        private Action<Texture2D> _onIconSelected;
        private List<Texture2D> _allIcons = new List<Texture2D>();
        private List<Texture2D> _filteredIcons = new List<Texture2D>();

        private string _searchString = "";
        private FilterTheme _selectedTheme = FilterTheme.All;
        private FilterColor _selectedColor = FilterColor.All;
        private Vector2 _scrollPos;

        private const float IconSize = 50f;
        private const float Spacing = 5f;
        private const float Padding = 10f;
        private const float ScrollbarWidth = 15f;

        #endregion

        #region Public Methods

        /// <summary>
        /// Opens the icon picker window as a utility window.
        /// </summary>
        /// <param name="onSelected">Callback invoked when an icon is clicked.</param>
        public static void Show(Action<Texture2D> onSelected)
        {
            var window = GetWindow<IconPickerWindow>(true, "Select Icon", true);
            window._onIconSelected = onSelected;
            window.minSize = new Vector2(400, 500);
            window.LoadIcons();
            window.ShowUtility();
        }

        #endregion

        #region Logic

        private void LoadIcons()
        {
            _allIcons.Clear();
            string[] guids = AssetDatabase.FindAssets("t:Texture2D");

            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.Contains("Vibrant Icons") && path.Contains("/Textures/"))
                {
                    var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                    if (tex != null) _allIcons.Add(tex);
                }
            }
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            _filteredIcons = _allIcons.Where(icon =>
            {
                string path = AssetDatabase.GetAssetPath(icon);
                string name = icon.name.ToLower();

                if (!string.IsNullOrEmpty(_searchString) && !name.Contains(_searchString.ToLower()))
                    return false;

                if (_selectedTheme == FilterTheme.Dark && !path.Contains("/Dark/")) return false;
                if (_selectedTheme == FilterTheme.Light && !path.Contains("/Light/")) return false;

                if (_selectedColor != FilterColor.All)
                {
                    string requiredColor = _selectedColor.ToString().ToLower();
                    if (!name.Contains(requiredColor)) return false;
                }
                return true;
            }).ToList();
        }

        #endregion

        #region GUI Drawing

        private void OnGUI()
        {
            DrawToolbar();
            DrawGrid();
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
                Close();
        }

        private void DrawToolbar()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            {
                string newSearch = GUILayout.TextField(_searchString, EditorStyles.toolbarSearchField, GUILayout.Width(120));
                if (newSearch != _searchString)
                {
                    _searchString = newSearch;
                    ApplyFilters();
                }

                GUIStyle cancelStyle = GUI.skin.FindStyle("ToolbarSeachCancelButton") ?? GUI.skin.FindStyle("ToolbarSearchCancelButton");
                if (GUILayout.Button(cancelStyle == null ? "X" : "", cancelStyle ?? EditorStyles.miniButton))
                {
                    _searchString = "";
                    GUI.FocusControl(null);
                    ApplyFilters();
                }

                GUILayout.Space(10);
                GUILayout.Label("Theme:", EditorStyles.miniLabel, GUILayout.Width(45));
                FilterTheme newTheme = (FilterTheme)EditorGUILayout.EnumPopup(_selectedTheme, EditorStyles.toolbarPopup, GUILayout.Width(60));
                if (newTheme != _selectedTheme)
                {
                    _selectedTheme = newTheme;
                    ApplyFilters();
                }

                GUILayout.Space(5);
                GUILayout.Label("Color:", EditorStyles.miniLabel, GUILayout.Width(40));
                FilterColor newColor = (FilterColor)EditorGUILayout.EnumPopup(_selectedColor, EditorStyles.toolbarPopup, GUILayout.Width(70));
                if (newColor != _selectedColor)
                {
                    _selectedColor = newColor;
                    ApplyFilters();
                }

                GUILayout.FlexibleSpace();
                GUILayout.Label($"{_filteredIcons.Count} icons", EditorStyles.miniLabel);
            }
            GUILayout.EndHorizontal();
        }

        private void DrawGrid()
        {
            float availableWidth = position.width - (Padding * 2) - ScrollbarWidth;
            int columns = Mathf.FloorToInt(availableWidth / (IconSize + Spacing));
            if (columns < 1) columns = 1;

            _scrollPos = GUILayout.BeginScrollView(_scrollPos);
            GUILayout.BeginVertical();
            GUILayout.Space(Padding);

            int index = 0;
            while (index < _filteredIcons.Count)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(Padding);

                for (int i = 0; i < columns; i++)
                {
                    if (index >= _filteredIcons.Count)
                    {
                         GUILayout.Label("", GUILayout.Width(IconSize), GUILayout.Height(IconSize));
                    }
                    else
                    {
                        var icon = _filteredIcons[index];
                        if (GUILayout.Button(new GUIContent(icon, icon.name), GUILayout.Width(IconSize), GUILayout.Height(IconSize)))
                        {
                            _onIconSelected?.Invoke(icon);
                            Close();
                        }
                        index++;
                    }
                    if (i < columns - 1) GUILayout.Space(Spacing);
                }
                GUILayout.Space(Padding);
                GUILayout.EndHorizontal();
                GUILayout.Space(Spacing);
            }
            GUILayout.Space(Padding);
            GUILayout.EndVertical();
            GUILayout.EndScrollView();
        }

        #endregion
    }
}
#endif
