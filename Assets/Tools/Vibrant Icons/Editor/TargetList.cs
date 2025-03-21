#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Envi.VibrantIcons
{
    /// <summary>
    /// Represents a list of Unity Objects with additional functionality.
    /// </summary>
    public class TargetList : IEnumerable<Object>
    {
        #region Fields

        /// <summary>
        /// The internal list of targets.
        /// </summary>
        private List<Object> targets = new List<Object>();

        /// <summary>
        /// Gets the number of targets in the list.
        /// </summary>
        public int Count => targets.Count;

        /// <summary>
        /// Gets the number of targets to display based on the slider value.
        /// </summary>
        private int displayedObjectsCount = 2;
        private int DisplayedTargetsCount => Mathf.Min(targets.Count, Mathf.Max(0, displayedObjectsCount));

        /// <summary>
        /// Gets the displayed targets.
        /// </summary>
        private IEnumerable<Object> DisplayedTargets => targets.Take(DisplayedTargetsCount);

        #endregion

        #region Methods

        /// <summary>
        /// Adds a collection of targets to the list.
        /// </summary>
        /// <param name="newTargets">The targets to add.</param>
        public void AddTargets(IEnumerable<Object> newTargets)
        {
            targets.AddRange(newTargets);
        }

        /// <summary>
        /// Removes a specific target from the list.
        /// </summary>
        /// <param name="target">The target to remove.</param>
        public void RemoveTarget(Object target)
        {
            int index = targets.IndexOf(target);

            if (index >= 0)
                targets.RemoveAt(index);
        }

        /// <summary>
        /// Updates the number of displayed objects in the list based on the provided count.
        /// </summary>
        /// <param name="newCount">The new count of displayed objects.</param>
        public void UpdateDisplayedObjectsCount(int newCount)
        {
            displayedObjectsCount = newCount;
        }

        /// <summary>
        /// Checks if the list contains a specific target.
        /// </summary>
        /// <param name="target">The target to check.</param>
        /// <returns>True if the target is in the list; otherwise, false.</returns>
        public bool Contains(Object target)
        {
            return targets.Contains(target);
        }

        /// <summary>
        /// Clears all targets from the list.
        /// </summary>
        public void Clear()
        {
            targets.Clear();
        }

        /// <summary>
        /// Draws a visual representation of the targets in the Editor window.
        /// </summary>
        public void Draw()
        {
            foreach (var target in DisplayedTargets)
            {
                EditorGUILayout.BeginHorizontal();

                if (target != null)
                {
                    Texture2D icon = AssetPreview.GetMiniThumbnail(target);

                    if (icon != null)
                        GUILayout.Label(icon, GUILayout.Width(16), GUILayout.Height(16));
                    else
                        GUILayout.Label("", GUILayout.Width(16), GUILayout.Height(16));

                    EditorGUILayout.LabelField($"{target.name} ({target.GetType().Name})", Styles.MiniLabel);

                    if (GUILayout.Button("X", GUILayout.Width(20)))
                        RemoveTarget(target);
                }
                else
                    RemoveTarget(target);

                EditorGUILayout.EndHorizontal();
            }
        }

        #endregion

        #region IEnumerable Implementation

        /// <summary>
        /// Gets an enumerator for iterating over the targets.
        /// </summary>
        /// <returns>The enumerator for the targets.</returns>
        public IEnumerator<Object> GetEnumerator()
        {
            return targets.GetEnumerator();
        }

        /// <summary>
        /// Gets the enumerator for iterating over the targets.
        /// </summary>
        /// <returns>The enumerator for the targets.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
#endif