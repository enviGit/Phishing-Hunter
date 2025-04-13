using ph.Managers;
using UnityEngine;

namespace ph.TestDebug {
    [ExecuteInEditMode]
    public class ResetDifficulty : MonoBehaviour {
        [ContextMenu("Set Difficulty to Newbie")]
        private void SetDifficultyToNewbie() {
            Settings.Difficulty = 0;
            Debug.Log("Difficulty has been set to: 0 (Newbie)");
        }

        [ContextMenu("Set Difficulty to Advanced")]
        private void SetDifficultyToAdvanced() {
            Settings.Difficulty = 1;
            Debug.Log("Difficulty has been set to: 1 (Advanced)");
        }
    }
}
