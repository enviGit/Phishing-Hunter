#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Envi.VibrantIcons
{
    /// <summary>
    /// Adds context menu items to the Project View to allow quick icon assignment via right-click.
    /// </summary>
    public static class VibrantIconsContextMenu
    {
        #region Menu Items

        [MenuItem("Assets/Vibrant Icons/Set Icon...", false, 20)]
        private static void SetIconFromContextMenu()
        {
            var selectedScripts = Selection.objects.OfType<MonoScript>().ToList();

            if (selectedScripts.Count == 0) return;

            IconPickerWindow.Show((selectedTexture) =>
            {
                ApplyIconToSelectedScripts(selectedScripts, selectedTexture);
            });
        }

        [MenuItem("Assets/Vibrant Icons/Set Icon...", true)]
        private static bool ValidateSetIconFromContextMenu()
        {
            return Selection.objects.OfType<MonoScript>().Any();
        }

        #endregion

        #region Logic

        private static void ApplyIconToSelectedScripts(System.Collections.Generic.List<MonoScript> scripts, Texture2D selectedIcon)
        {
            bool isDark = EditorGUIUtility.isProSkin;
            string targetTheme = isDark ? "Dark" : "Light";
            string iconName = selectedIcon.name;

            Texture2D finalIcon = selectedIcon;
            string[] guids = AssetDatabase.FindAssets($"{iconName} t:Texture2D");

            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.Contains(targetTheme) &&
                    path.Contains("Vibrant Icons") &&
                    Path.GetFileNameWithoutExtension(path) == iconName)
                {
                    finalIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                    break;
                }
            }

            int count = 0;
            foreach (var script in scripts)
            {
                string path = AssetDatabase.GetAssetPath(script);
                var importer = AssetImporter.GetAtPath(path) as MonoImporter;
                if (importer != null)
                {
                    importer.SetIcon(finalIcon);
                    EditorUtility.SetDirty(importer);
                    importer.SaveAndReimport();
                    count++;
                }
            }

            AssetDatabase.Refresh();
            Debug.Log($"Vibrant Icons: Changed icon for {count} scripts (via Context Menu).");
        }

        #endregion
    }
}
#endif
