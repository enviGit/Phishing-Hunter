using System;
using System.Collections.Generic;

namespace ph.Managers.Save {
    [Serializable]
    public class GameData {
        public List<int> flaggedMailIds;
        public List<int> solvedQuizIds;

        public GameData() {
            this.flaggedMailIds = new List<int>();
            this.solvedQuizIds = new List<int>();
        }
    }
}
