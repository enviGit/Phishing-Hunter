using DG.Tweening;
using ph.Managers;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ph.Core {
    public class PlayerRatingSystem : MonoBehaviour {
        public enum PlayerPosition {
            Intern,
            JuniorCybersecurityAnalyst,
            CybersecurityAnalyst,
            SeniorCybersecurityAnalyst,
            CybersecurityExpert
        }
        private readonly Dictionary<string, Dictionary<PlayerPosition, string>> positionTranslations = new Dictionary<string, Dictionary<PlayerPosition, string>> {
    { "en", new Dictionary<PlayerPosition, string> {
        { PlayerPosition.Intern, "Intern" },
        { PlayerPosition.JuniorCybersecurityAnalyst, "Junior Cybersecurity Analyst" },
        { PlayerPosition.CybersecurityAnalyst, "Cybersecurity Analyst" },
        { PlayerPosition.SeniorCybersecurityAnalyst, "Senior Cybersecurity Analyst" },
        { PlayerPosition.CybersecurityExpert, "Cybersecurity Expert" }
    }},
    { "pl", new Dictionary<PlayerPosition, string> {
        { PlayerPosition.Intern, "Stażysta" },
        { PlayerPosition.JuniorCybersecurityAnalyst, "Junior Analityk Cyberbezpieczeństwa" },
        { PlayerPosition.CybersecurityAnalyst, "Analityk Cyberbezpieczeństwa" },
        { PlayerPosition.SeniorCybersecurityAnalyst, "Starszy Analityk Cyberbezpieczeństwa" },
        { PlayerPosition.CybersecurityExpert, "Ekspert ds. Cyberbezpieczeństwa" }
    }},
    { "es", new Dictionary<PlayerPosition, string> {
        { PlayerPosition.Intern, "Pasantía" },
        { PlayerPosition.JuniorCybersecurityAnalyst, "Analista Junior de Ciberseguridad" },
        { PlayerPosition.CybersecurityAnalyst, "Analista de Ciberseguridad" },
        { PlayerPosition.SeniorCybersecurityAnalyst, "Analista Senior de Ciberseguridad" },
        { PlayerPosition.CybersecurityExpert, "Experto en Ciberseguridad" }
    }},
    { "fr", new Dictionary<PlayerPosition, string> {
        { PlayerPosition.Intern, "Stagiaire" },
        { PlayerPosition.JuniorCybersecurityAnalyst, "Analyste Junior en Cybersécurité" },
        { PlayerPosition.CybersecurityAnalyst, "Analyste en Cybersécurité" },
        { PlayerPosition.SeniorCybersecurityAnalyst, "Analyste Senior en Cybersécurité" },
        { PlayerPosition.CybersecurityExpert, "Expert en Cybersécurité" }
    }},
    { "de", new Dictionary<PlayerPosition, string> {
        { PlayerPosition.Intern, "Praktikant" },
        { PlayerPosition.JuniorCybersecurityAnalyst, "Junior Analyst für Cybersicherheit" },
        { PlayerPosition.CybersecurityAnalyst, "Analyst für Cybersicherheit" },
        { PlayerPosition.SeniorCybersecurityAnalyst, "Senior Analyst für Cybersicherheit" },
        { PlayerPosition.CybersecurityExpert, "Cybersicherheitsexperte" }
    }},
    { "it", new Dictionary<PlayerPosition, string> {
        { PlayerPosition.Intern, "Tirocinante" },
        { PlayerPosition.JuniorCybersecurityAnalyst, "Analista Junior di Cybersecurity" },
        { PlayerPosition.CybersecurityAnalyst, "Analista di Cybersecurity" },
        { PlayerPosition.SeniorCybersecurityAnalyst, "Analista Senior di Cybersecurity" },
        { PlayerPosition.CybersecurityExpert, "Esperto di Cybersecurity" }
    }},
    { "ru", new Dictionary<PlayerPosition, string> {
        { PlayerPosition.Intern, "Стажер" },
        { PlayerPosition.JuniorCybersecurityAnalyst, "Младший аналитик по кибербезопасности" },
        { PlayerPosition.CybersecurityAnalyst, "Аналитик по кибербезопасности" },
        { PlayerPosition.SeniorCybersecurityAnalyst, "Старший аналитик по кибербезопасности" },
        { PlayerPosition.CybersecurityExpert, "Эксперт по кибербезопасности" }
    }},
    { "pt-BR", new Dictionary<PlayerPosition, string> {
        { PlayerPosition.Intern, "Estagiário" },
        { PlayerPosition.JuniorCybersecurityAnalyst, "Analista Júnior de Cibersegurança" },
        { PlayerPosition.CybersecurityAnalyst, "Analista de Cibersegurança" },
        { PlayerPosition.SeniorCybersecurityAnalyst, "Analista Sênior de Cibersegurança" },
        { PlayerPosition.CybersecurityExpert, "Especialista em Cibersegurança" }
    }},
    { "ko", new Dictionary<PlayerPosition, string> {
        { PlayerPosition.Intern, "인턴" },
        { PlayerPosition.JuniorCybersecurityAnalyst, "주니어 사이버 보안 분석가" },
        { PlayerPosition.CybersecurityAnalyst, "사이버 보안 분석가" },
        { PlayerPosition.SeniorCybersecurityAnalyst, "선임 사이버 보안 분석가" },
        { PlayerPosition.CybersecurityExpert, "사이버 보안 전문가" }
    }},
    { "zh-Hans", new Dictionary<PlayerPosition, string> {
        { PlayerPosition.Intern, "实习生" },
        { PlayerPosition.JuniorCybersecurityAnalyst, "初级网络安全分析师" },
        { PlayerPosition.CybersecurityAnalyst, "网络安全分析师" },
        { PlayerPosition.SeniorCybersecurityAnalyst, "高级网络安全分析师" },
        { PlayerPosition.CybersecurityExpert, "网络安全专家" }
    }}
};
        public static PlayerRatingSystem Instance;
        [SerializeField] private AchievementManager achievementManager;

        [Header("UI")]
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI positionText;
        [SerializeField] private Image progressSlider;
        [SerializeField] private CanvasGroup progressGroup;

        [Header("Progress")]
        [HideInInspector] public int level = 0;
        [HideInInspector] public PlayerPosition position = PlayerPosition.Intern;
        private float totalProgress;

        private void Awake() {
            Instance = this;
        }
        private void Start() {
            UpdateUI();
            progressSlider.fillAmount = 0f;
        }
        public void UpdateProgress() {
            int correctMails = MailManager.CorrectMailAnswers;
            int correctQuizzes = QuizManager.CorrectQuizAnswers;
            int totalMails = MailManager.TotalMailCount;
            int totalQuizzes = QuizManager.TotalQuizCount;

            progressGroup.transform.SetSiblingIndex(progressGroup.transform.parent.childCount - 3);
            progressGroup.DOFade(1f, 0.5f).SetEase(Ease.OutBack);

            float rawProgress = (correctMails * 2f + correctQuizzes) / (totalMails * 2f + totalQuizzes);
            totalProgress = rawProgress * MaxLevelCount();

            int newLevel = Mathf.FloorToInt(totalProgress);
            float currentLevelProgress = totalProgress - newLevel;

            if (newLevel > level && level < MaxLevelCount()) {
                level = newLevel;
                progressSlider.fillAmount = 0;
                UpdateDifficulty();
                UpdatePosition();
            }

            if (level >= MaxLevelCount()) {
                currentLevelProgress = 1f;
            }

            progressGroup.DOFade(1f, 0.5f).OnComplete(() => {
                progressSlider.DOFillAmount(currentLevelProgress, 1f).SetEase(Ease.OutSine);
            });

            UpdateUI();

            progressGroup.DOFade(0.1f, 0.5f).SetDelay(2f).SetEase(Ease.InBack).OnComplete(() => {
                progressGroup.transform.SetSiblingIndex(1);
            });

            achievementManager.CheckAllAchievements();
        }
        private int MaxLevelCount() {
            return 50;
        }
        private void UpdatePosition() {
            if (level < 5) position = PlayerPosition.Intern;
            else if (level < 10) position = PlayerPosition.JuniorCybersecurityAnalyst;
            else if (level < 20) position = PlayerPosition.CybersecurityAnalyst;
            else if (level < 40) position = PlayerPosition.SeniorCybersecurityAnalyst;
            else position = PlayerPosition.CybersecurityExpert;
        }
        private void UpdateDifficulty() {
            Settings.Difficulty = level < 5 ? 0 : 1;
        }
        private void UpdateUI() {
            levelText.text = $"{level}";
            positionText.text = GetPositionDisplayName(position);
        }
        private string GetPositionDisplayName(PlayerPosition pos) {
            string currentLanguage = Settings.Language;

            if (positionTranslations.ContainsKey(currentLanguage) &&
                positionTranslations[currentLanguage].ContainsKey(pos)) {
                return positionTranslations[currentLanguage][pos];
            }
            return positionTranslations["en"][pos];
        }
    }
}