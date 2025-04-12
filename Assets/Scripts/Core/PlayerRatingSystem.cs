using ph.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ph.Core {
    public class PlayerRatingSystem : MonoBehaviour {
        public enum PlayerPosition {
            Intern,
            JuniorAnalyst,
            Analyst,
            SeniorAnalyst,
            Expert
        }

        [Header("Managers")]
        public QuizManager quizManager;
        public MailManager mailManager;

        [Header("UI")]
        public TextMeshProUGUI levelText;
        public TextMeshProUGUI positionText;
        public Image progressSlider;

        [Header("Progress")]
        public int level = 0;
        public PlayerPosition position = PlayerPosition.Intern;
        private int quizzesCompleted = 0;
        private int mailsIdentified = 0;

        private void Start() {
            UpdateUI();
            progressSlider.fillAmount = 0f;
        }

        public void OnQuizCompleted() {
            quizzesCompleted++;
            UpdateProgress();
        }

        public void OnMailPhishingIdentified() {
            mailsIdentified++;
            UpdateProgress();
        }

        private void UpdateProgress() {
            int totalProgress = quizzesCompleted + mailsIdentified;
            int requiredForNextLevel = (level + 1) * 5;

            progressSlider.fillAmount = (float)totalProgress / requiredForNextLevel;

            if (totalProgress >= requiredForNextLevel) {
                level++;
                quizzesCompleted = 0;
                mailsIdentified = 0;
                UpdatePosition();
                UpdateDifficulty();
                UpdateUI();
                progressSlider.fillAmount = 0f;
            }
        }

        private void UpdatePosition() {
            if (level < 3) position = PlayerPosition.Intern;
            else if (level < 6) position = PlayerPosition.JuniorAnalyst;
            else if (level < 9) position = PlayerPosition.Analyst;
            else if (level < 12) position = PlayerPosition.SeniorAnalyst;
            else position = PlayerPosition.Expert;
        }

        private void UpdateDifficulty() {
            Settings.Difficulty = level < 5 ? 0 : 1;
        }

        private void UpdateUI() {
            levelText.text = $"{level}";
            positionText.text = GetPositionDisplayName(position);
        }

        private string GetPositionDisplayName(PlayerPosition pos) {
            switch (pos) {
                case PlayerPosition.Intern: return "Intern";
                case PlayerPosition.JuniorAnalyst: return "Junior Cybersecurity Analyst";
                case PlayerPosition.Analyst: return "Cybersecurity Analyst";
                case PlayerPosition.SeniorAnalyst: return "Senior Cybersecurity Analyst";
                case PlayerPosition.Expert: return "Cybersecurity Expert";
                default: return "Unknown";
            }
        }
    }
}
