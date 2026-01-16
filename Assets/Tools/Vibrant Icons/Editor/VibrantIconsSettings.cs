#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Envi.VibrantIcons
{
    /// <summary>
    /// Represents a single rule for assigning an icon based on a file name.
    /// </summary>
    [Serializable]
    public class IconRule
    {
        /// <summary>
        /// The text to search for in the script's name (e.g., "Manager").
        /// </summary>
        public string Keyword;

        /// <summary>
        /// The icon texture to apply when this rule is matched.
        /// </summary>
        public Texture2D Icon;

        /// <summary>
        /// If true, the script name must match the keyword exactly (case-insensitive).
        /// If false, it checks if the script name contains the keyword.
        /// </summary>
        public bool ExactMatch;

        /// <summary>
        /// Determines the order of evaluation. Higher values are checked first.
        /// </summary>
        public int Priority = 1;
    }

    /// <summary>
    /// ScriptableObject storing configuration and rules for Vibrant Icons.
    /// </summary>
    [CreateAssetMenu(fileName = "VibrantIconsSettings", menuName = "Vibrant Icons Settings")]
    public class VibrantIconsSettings : ScriptableObject
    {
        #region Fields

        [Header("Global Settings")]
        [Tooltip("If enabled, icons will be applied automatically when new scripts are imported or recompiled.")]
        public bool AutoProcessOnImport = true;

        [Header("Naming Rules")]
        [Tooltip("List of rules defining which icons should be assigned to which script names.")]
        public List<IconRule> Rules = new List<IconRule>();

        #endregion

        #region Public Methods

        /// <summary>
        /// Tries to locate the settings asset in the project.
        /// </summary>
        /// <returns>The found settings instance, or null if none exists.</returns>
        public static VibrantIconsSettings GetOrCreateSettings()
        {
            var guids = AssetDatabase.FindAssets("t:VibrantIconsSettings");
            if (guids.Length > 0)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                return AssetDatabase.LoadAssetAtPath<VibrantIconsSettings>(path);
            }
            return null;
        }

        #endregion
    }
}
#endif
