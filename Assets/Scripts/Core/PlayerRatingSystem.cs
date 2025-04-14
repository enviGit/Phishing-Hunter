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
            JuniorAnalyst,
            Analyst,
            SeniorAnalyst,
            Expert
        }
        private readonly Dictionary<string, Dictionary<PlayerPosition, string>> positionTranslations = new Dictionary<string, Dictionary<PlayerPosition, string>> {
    { "en", new Dictionary<PlayerPosition, string> {
        { PlayerPosition.Intern, "Intern" },
        { PlayerPosition.JuniorAnalyst, "Junior Cybersecurity Analyst" },
        { PlayerPosition.Analyst, "Cybersecurity Analyst" },
        { PlayerPosition.SeniorAnalyst, "Senior Cybersecurity Analyst" },
        { PlayerPosition.Expert, "Cybersecurity Expert" }
    }},
    { "pl", new Dictionary<PlayerPosition, string> {
        { PlayerPosition.Intern, "Stażysta" },
        { PlayerPosition.JuniorAnalyst, "Junior Analityk Cyberbezpieczeństwa" },
        { PlayerPosition.Analyst, "Analityk Cyberbezpieczeństwa" },
        { PlayerPosition.SeniorAnalyst, "Starszy Analityk Cyberbezpieczeństwa" },
        { PlayerPosition.Expert, "Ekspert ds. Cyberbezpieczeństwa" }
    }},
    { "es", new Dictionary<PlayerPosition, string> {
        { PlayerPosition.Intern, "Pasantía" },
        { PlayerPosition.JuniorAnalyst, "Analista Junior de Ciberseguridad" },
        { PlayerPosition.Analyst, "Analista de Ciberseguridad" },
        { PlayerPosition.SeniorAnalyst, "Analista Senior de Ciberseguridad" },
        { PlayerPosition.Expert, "Experto en Ciberseguridad" }
    }},
    { "fr", new Dictionary<PlayerPosition, string> {
        { PlayerPosition.Intern, "Stagiaire" },
        { PlayerPosition.JuniorAnalyst, "Analyste Junior en Cybersécurité" },
        { PlayerPosition.Analyst, "Analyste en Cybersécurité" },
        { PlayerPosition.SeniorAnalyst, "Analyste Senior en Cybersécurité" },
        { PlayerPosition.Expert, "Expert en Cybersécurité" }
    }},
    { "de", new Dictionary<PlayerPosition, string> {
        { PlayerPosition.Intern, "Praktikant" },
        { PlayerPosition.JuniorAnalyst, "Junior Analyst für Cybersicherheit" },
        { PlayerPosition.Analyst, "Analyst für Cybersicherheit" },
        { PlayerPosition.SeniorAnalyst, "Senior Analyst für Cybersicherheit" },
        { PlayerPosition.Expert, "Cybersicherheitsexperte" }
    }},
    { "it", new Dictionary<PlayerPosition, string> {
        { PlayerPosition.Intern, "Tirocinante" },
        { PlayerPosition.JuniorAnalyst, "Analista Junior di Cybersecurity" },
        { PlayerPosition.Analyst, "Analista di Cybersecurity" },
        { PlayerPosition.SeniorAnalyst, "Analista Senior di Cybersecurity" },
        { PlayerPosition.Expert, "Esperto di Cybersecurity" }
    }},
    { "ru", new Dictionary<PlayerPosition, string> {
        { PlayerPosition.Intern, "Стажер" },
        { PlayerPosition.JuniorAnalyst, "Младший аналитик по кибербезопасности" },
        { PlayerPosition.Analyst, "Аналитик по кибербезопасности" },
        { PlayerPosition.SeniorAnalyst, "Старший аналитик по кибербезопасности" },
        { PlayerPosition.Expert, "Эксперт по кибербезопасности" }
    }},
    { "pt-BR", new Dictionary<PlayerPosition, string> {
        { PlayerPosition.Intern, "Estagiário" },
        { PlayerPosition.JuniorAnalyst, "Analista Júnior de Cibersegurança" },
        { PlayerPosition.Analyst, "Analista de Cibersegurança" },
        { PlayerPosition.SeniorAnalyst, "Analista Sênior de Cibersegurança" },
        { PlayerPosition.Expert, "Especialista em Cibersegurança" }
    }},
    { "ko", new Dictionary<PlayerPosition, string> {
        { PlayerPosition.Intern, "인턴" },
        { PlayerPosition.JuniorAnalyst, "주니어 사이버 보안 분석가" },
        { PlayerPosition.Analyst, "사이버 보안 분석가" },
        { PlayerPosition.SeniorAnalyst, "선임 사이버 보안 분석가" },
        { PlayerPosition.Expert, "사이버 보안 전문가" }
    }},
    { "zh-Hans", new Dictionary<PlayerPosition, string> {
        { PlayerPosition.Intern, "实习生" },
        { PlayerPosition.JuniorAnalyst, "初级网络安全分析师" },
        { PlayerPosition.Analyst, "网络安全分析师" },
        { PlayerPosition.SeniorAnalyst, "高级网络安全分析师" },
        { PlayerPosition.Expert, "网络安全专家" }
    }}
};
        public static PlayerRatingSystem Instance;

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
            totalProgress = rawProgress * MaxLevelCount(); // gdzie MaxLevelCount = ile poziomów przewidujesz

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
        }
        private int MaxLevelCount() {
            return 50;
        }
        private void UpdatePosition() {
            if (level < 5) position = PlayerPosition.Intern;
            else if (level < 10) position = PlayerPosition.JuniorAnalyst;
            else if (level < 20) position = PlayerPosition.Analyst;
            else if (level < 40) position = PlayerPosition.SeniorAnalyst;
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
            string currentLanguage = Settings.Language;

            if (positionTranslations.ContainsKey(currentLanguage) &&
                positionTranslations[currentLanguage].ContainsKey(pos)) {
                return positionTranslations[currentLanguage][pos];
            }
            return positionTranslations["en"][pos];
        }
    }
}