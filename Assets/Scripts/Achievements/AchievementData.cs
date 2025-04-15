using System.Collections.Generic;
using UnityEngine;

namespace ph.Achievements {
    public enum AchievementType {
        Quiz,
        Mail,
        Level
    }

    [System.Serializable]
    public class Achievement {
        public string id;
        public AchievementType type;
        public int threshold;
        public Sprite icon;

        public Dictionary<string, string> titles;
        public Dictionary<string, string> descriptions;

        public string GetTitle(string lang) => titles.ContainsKey(lang) ? titles[lang] : titles["en"];
        public string GetDescription(string lang) => descriptions.ContainsKey(lang) ? descriptions[lang] : descriptions["en"];
    }
}
