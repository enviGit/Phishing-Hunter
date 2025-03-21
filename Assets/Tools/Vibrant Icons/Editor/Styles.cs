using UnityEditor;
using UnityEngine;

namespace Envi.VibrantIcons
{
    /// <summary>
    /// Static class for defining custom GUI styles.
    /// </summary>
    public static class Styles
    {
        #region Properties

        /// <summary>
        /// Gets a new GUIStyle for bold labels.
        /// </summary>
        public static GUIStyle BoldLabel => new GUIStyle(EditorStyles.boldLabel);

        /// <summary>
        /// Gets a new GUIStyle for mini labels.
        /// </summary>
        public static GUIStyle MiniLabel => new GUIStyle(EditorStyles.miniLabel);

        #endregion
    }
}