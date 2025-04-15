using ph.Achievements;
using ph.Core;
using System.Collections.Generic;
using UnityEngine;

namespace ph.Managers {
    public class AchievementManager : MonoBehaviour {
        public static AchievementManager Instance;
        [SerializeField] private AchievementUI achievementUI;

        private HashSet<string> unlocked = new HashSet<string>();

        private void Awake() {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }
        public void CheckAllAchievements() {
            int quizCorrect = QuizManager.CorrectQuizAnswers;
            int mailCorrect = MailManager.CorrectMailAnswers;
            int level = PlayerRatingSystem.Instance.level;

            foreach (var a in AchievementDatabase.achievements) {
                if (unlocked.Contains(a.id)) continue;

                bool shouldUnlock = a.type switch {
                    AchievementType.Quiz => quizCorrect >= a.threshold,
                    AchievementType.Mail => mailCorrect >= a.threshold,
                    AchievementType.Level => level >= a.threshold,
                    _ => false
                };

                if (shouldUnlock) UnlockAchievement(a);
            }
        }
        private void UnlockAchievement(Achievement achievement) {
            unlocked.Add(achievement.id);
            achievementUI.Show(achievement);
        }
    }
}
