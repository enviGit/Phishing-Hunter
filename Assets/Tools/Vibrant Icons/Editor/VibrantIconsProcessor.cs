#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Envi.VibrantIcons
{
    /// <summary>
    /// AssetPostprocessor responsible for applying icon rules automatically on import
    /// and providing static methods for batch processing.
    /// </summary>
    public class VibrantIconsProcessor : AssetPostprocessor
    {
        #region Unity Callbacks

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            var settings = VibrantIconsSettings.GetOrCreateSettings();
            if (settings == null || !settings.AutoProcessOnImport) return;

            foreach (string path in importedAssets)
            {
                if (path.EndsWith(".cs")) ApplyIconToScript(path, settings);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Scans all MonoScripts in the project and applies the current icon rules.
        /// </summary>
        public static void ApplyIconsToAllScripts()
        {
            var settings = VibrantIconsSettings.GetOrCreateSettings();
            if (settings == null)
            {
                Debug.LogWarning("Vibrant Icons: Settings not found.");
                return;
            }

            string[] allScripts = AssetDatabase.FindAssets("t:MonoScript");
            int count = 0;
            try
            {
                for (int i = 0; i < allScripts.Length; i++)
                {
                    string path = AssetDatabase.GUIDToAssetPath(allScripts[i]);
                    EditorUtility.DisplayProgressBar("Vibrant Icons", $"Processing: {Path.GetFileName(path)}", (float)i / allScripts.Length);
                    if (ApplyIconToScript(path, settings)) count++;
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
                Debug.Log($"Vibrant Icons: Updated {count} scripts.");
            }
        }

        #endregion

        #region Internal Logic

        private static bool ApplyIconToScript(string path, VibrantIconsSettings settings)
        {
            string fileName = Path.GetFileNameWithoutExtension(path);

            var sortedRules = settings.Rules.OrderByDescending(r => r.Priority);

            foreach (var rule in sortedRules)
            {
                if (string.IsNullOrEmpty(rule.Keyword) || rule.Icon == null) continue;

                bool match = rule.ExactMatch
                    ? fileName.Equals(rule.Keyword, System.StringComparison.OrdinalIgnoreCase)
                    : fileName.IndexOf(rule.Keyword, System.StringComparison.OrdinalIgnoreCase) >= 0;

                if (match)
                {
                    bool isDarkTheme = EditorGUIUtility.isProSkin;
                    return SetSmartIcon(path, rule.Icon, isDarkTheme);
                }
            }
            return false;
        }

        private static bool SetSmartIcon(string scriptPath, Texture2D sourceIcon, bool isDarkMode)
        {
            MonoImporter importer = AssetImporter.GetAtPath(scriptPath) as MonoImporter;
            if (importer == null) return false;

            string iconName = sourceIcon.name;
            string targetThemeFolder = isDarkMode ? "Dark" : "Light";

            string[] guids = AssetDatabase.FindAssets($"{iconName} t:Texture2D");
            Texture2D finalIcon = sourceIcon;

            foreach (var guid in guids)
            {
                string p = AssetDatabase.GUIDToAssetPath(guid);
                if (p.Contains(targetThemeFolder) && Path.GetFileNameWithoutExtension(p) == iconName)
                {
                    finalIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(p);
                    break;
                }
            }

            var currentIcon = importer.GetIcon();
            if (currentIcon != finalIcon)
            {
                importer.SetIcon(finalIcon);
                importer.SaveAndReimport();
                return true;
            }
            return false;
        }

        #endregion
    }
}
#endif
