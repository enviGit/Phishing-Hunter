#if UNITY_EDITOR
using Object = UnityEngine.Object;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Envi.VibrantIcons
{
    /// <summary>
    /// Responsible for loading textures and handling file-related operations.
    /// </summary>
    public class TextureLoader
    {
        #region Methods

        /// <summary>
        /// Checks if the provided object is an accepted file (MonoScript).
        /// </summary>
        /// <param name="obj">The object to check.</param>
        /// <returns>True if the object is a MonoScript; otherwise, false.</returns>
        public bool IsAcceptedFile(Object obj)
        {
            // Ensure that the object is not null before checking its type.
            return obj != null && obj is MonoScript;
        }

        /// <summary>
        /// Checks if the specified folder path exists.
        /// </summary>
        /// <param name="folderPath">The path to the folder.</param>
        /// <returns>True if the folder exists; otherwise, false.</returns>
        public bool IsFolderAvailable(string folderPath)
        {
            try
            {
                // Check if the folder exists.
                return Directory.Exists(folderPath);
            }
            catch (Exception ex)
            {
                Debug.LogError($"An error occurred while checking if folder is available: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Finds a MonoScript at the specified path.
        /// </summary>
        /// <param name="path">The path to the MonoScript.</param>
        /// <returns>The found MonoScript or null if not found.</returns>
        public MonoScript FindMonoScriptAtPath(string path)
        {
            try
            {
                string assetPath = "Assets" + path.Replace(Application.dataPath, "");
                MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(assetPath);
                return script;
            }
            catch (Exception ex)
            {
                Debug.LogError($"An error occurred while finding MonoScript at path '{path}': {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Loads textures from the specified folder and invokes the provided action for each loaded texture.
        /// </summary>
        /// <param name="folderPath">The path to the folder containing textures.</param>
        /// <param name="onTextureLoaded">Action to execute for each loaded texture.</param>
        public void LoadTexturesFromFolder(string folderPath, IconColor selectedColor, Action<Texture2D> onTextureLoaded)
        {
            try
            {
                string[] textureGuids = AssetDatabase.FindAssets("t:texture", new[] { folderPath });

                foreach (var textureGuid in textureGuids)
                {
                    string texturePath = AssetDatabase.GUIDToAssetPath(textureGuid);
                    Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);

                    if (texture != null)
                    {
                        string fileName = Path.GetFileNameWithoutExtension(texturePath);

                        if (selectedColor == IconColor.All || fileName.ToLower().Contains(selectedColor.ToString().ToLower()))
                            onTextureLoaded(texture);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"An error occurred while loading textures from folder '{folderPath}': {ex.Message}");
            }
        }

        #endregion
    }
}
#endif