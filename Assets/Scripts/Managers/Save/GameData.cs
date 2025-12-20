using System;
using System.Collections.Generic;

namespace ph.Managers.Save {
    [Serializable]
    public class GameData {
        public int currentLevel;
        public int playerPosition;

        public int correctMailsCount;
        public int correctQuizCount;

        public List<int> flaggedMailIds;
        public List<int> solvedQuizIds;

        public List<string> unlockedAchievements;

        public GameData() {
            this.currentLevel = 0;
            this.playerPosition = 0;
            this.correctMailsCount = 0;
            this.correctQuizCount = 0;

            this.flaggedMailIds = new List<int>();
            this.solvedQuizIds = new List<int>();
            this.unlockedAchievements = new List<string>();
        }
    }
}