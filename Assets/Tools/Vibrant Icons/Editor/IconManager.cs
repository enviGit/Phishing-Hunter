#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Envi.VibrantIcons
{
    /// <summary>
    /// Icon manager responsible for managing available icons and operations on them.
    /// </summary>
    public class IconManager
    {
        #region Events

        /// <summary>
        /// Event triggered when the selected color is changed.
        /// </summary>
        public event Action<IconColor> ColorChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the currently selected icon color.
        /// </summary>
        private IconColor selectedColor = IconColor.All;
        public IconColor SelectedColor
        {
            get => selectedColor;
            set
            {
                selectedColor = value;
                ColorChanged?.Invoke(selectedColor);
            }
        }

        /// <summary>
        /// Flag indicating whether dark mode is enabled.
        /// </summary>
        public bool IsDarkMode { get; set; } = true;

        /// <summary>
        /// List of available textures (icons) in the manager.
        /// </summary>
        public List<Texture2D> AvailableTextures => availableTextures;

        #endregion

        #region Fields

        private List<Texture2D> availableTextures = new List<Texture2D>();

        #endregion

        #region Methods

        /// <summary>
        /// Toggles dark mode and saves the preference.
        /// </summary>
        public void ToggleDarkMode()
        {
            IsDarkMode = !IsDarkMode;
        }

        /// <summary>
        /// Restores default icons for specified targets.
        /// </summary>
        /// <param name="targetList">List of objects for which default icons should be restored.</param>
        public void RestoreDefaultIcons(TargetList targetList)
        {
            foreach (var target in targetList)
            {
                // Get the path to the object.
                var targetPath = AssetDatabase.GetAssetPath(target);

                // Get the importer for the specified object type (MonoScript).
                var importer = (MonoImporter)AssetImporter.GetAtPath(targetPath);

                // Remove the icon and save again to apply changes.
                importer.SetIcon(null);
                importer.SaveAndReimport();
            }
        }

        /// <summary>
        /// Adds a new texture (icon) to the list of available textures.
        /// </summary>
        /// <param name="texture">New texture to add.</param>
        public void AddTexture(Texture2D texture)
        {
            availableTextures.Add(texture);
        }

        /// <summary>
        /// Clears the list of available textures.
        /// </summary>
        public void ClearAvailableTextures()
        {
            availableTextures.Clear();
        }

        /// <summary>
        /// Sets a specific icon for specified targets.
        /// </summary>
        /// <param name="targetList">List of objects for which a new icon should be set.</param>
        /// <param name="texture">New texture (icon) to set.</param>
        public void SetIconForTargets(TargetList targetList, Texture2D texture)
        {
            foreach (var target in targetList)
            {
                // Get the path to the object.
                var targetPath = AssetDatabase.GetAssetPath(target);

                // Get the importer for the specified object type (MonoScript).
                var importer = (MonoImporter)AssetImporter.GetAtPath(targetPath);

                // Set the new icon and save again to apply changes.
                importer.SetIcon(texture);
                importer.SaveAndReimport();
            }
        }

        /// <summary>
        /// Checks if any target has a custom icon set.
        /// </summary>
        /// <param name="targetList">List of objects to check for custom icons.</param>
        /// <returns>True if any target has a custom icon, otherwise false.</returns>
        public bool AnyTargetHasCustomIcon(TargetList targetList)
        {
            foreach (var target in targetList)
            {
                var targetPath = AssetDatabase.GetAssetPath(target);
                var importer = (MonoImporter)AssetImporter.GetAtPath(targetPath);

                if (importer != null && importer.GetIcon() != null)
                    return true;
            }

            return false;
        }

        #endregion
    }
}
#endif