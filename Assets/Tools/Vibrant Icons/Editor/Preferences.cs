using Object = UnityEngine.Object;
using System;
using System.Collections.Generic;

namespace Envi.VibrantIcons
{
    /// <summary>
    /// Class representing preferences for VibrantIconsWindow.
    /// </summary>
    [Serializable]
    public class Preferences
    {
        #region Fields

        public bool IsDarkMode;
        public List<Object> Targets;
        public int DisplayedObjectsCount;

        #endregion
    }
}