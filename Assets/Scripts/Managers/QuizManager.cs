using DG.Tweening;
using ph.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ph.Managers {
    [Serializable]
    public class QuizQuestion {
        public int id;
        public string question;
        public string ansA;
        public string ansB;
        public string ansC;
        public string ansD;
        public List<string> correctAns;
        public string tag;
    }

    [Serializable]
    public class QuizData {
        public List<QuizQuestion> newbieQuestions;
        public List<QuizQuestion> advancedQuestions;
    }

    public class QuizManager : MonoBehaviour {
        private string languageFileName = "questions";
        [SerializeField] private Transform workSpace;
        [SerializeField] private GameObject questionPrefab;
        [SerializeField] private int maxDisplayedQuestions = 7;
        [SerializeField] private GameObject resultPanel;
        private TextMeshProUGUI resultText;
        [SerializeField] private Button newButton;
        [SerializeField] private Button checkButton;
        private List<QuizQuestion> questionList;
        private List<int> displayedQuestionIds = new List<int>();
        private int correctAnswers = 0;
        private int totalQuestions = 0;
        private Dictionary<int, List<string>> userAnswers = new();
        private List<int> incorrectQuestions = new List<int>();
        private List<int> fullyCorrectQuestions = new();
        private QuizData quizData;
        private Dictionary<string, (string SingleChoice, string MultipleChoice)> localization =
    new() {
        { "en", ("(Single choice)", "(Multiple choice)") },
        { "pl", ("(Jednokrotny wybór)", "(Wielokrotny wybór)") },
        { "es", ("(Opción única)", "(Opción múltiple)") },
        { "fr", ("(Choix unique)", "(Choix multiples)") },
        { "de", ("(Einzelauswahl)", "(Mehrfachauswahl)") },
        { "it", ("(Scelta singola)", "(Scelta multipla)") },
        { "ru", ("(Один вариант)", "(Несколько вариантов)") },
        { "pt-BR", ("(Escolha única)", "(Escolha múltipla)") },
        { "ko", ("(단일 선택)", "(다중 선택)") },
        { "zh-Hans", ("(单选)", "(多选)") }
    };
        public static int TotalQuizCount { get; private set; }
        public static int CorrectQuizAnswers { get; private set; }

        private void Start() {
            resultText = resultPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            TotalQuizCount = LoadTotalQuizCount();
            Debug.Log($"Total Quiz Count: {TotalQuizCount}");
            CorrectQuizAnswers = 0;
            LoadQuestions();
            DisplayQuestions();
            resultPanel.GetComponent<CanvasGroup>().alpha = 0;
            checkButton.interactable = true;
            newButton.interactable = false;
        }
        private void LoadQuestions() {
            string lang = Settings.Language;
            string fileName = $"{languageFileName}_{lang}";
            TextAsset json = Resources.Load<TextAsset>(fileName);

            if (json == null) {
                Debug.LogError($"Brak pliku JSON: {fileName} w folderze Resources.");
                return;
            }

            quizData = JsonUtility.FromJson<QuizData>(json.text);

            questionList = Settings.Difficulty == 0
                ? quizData.newbieQuestions
                : quizData.newbieQuestions.Concat(quizData.advancedQuestions).ToList();
        }
        public void DisplayQuestions() {
            List<QuizQuestion> questionsToDisplay = questionList
                .Where(q => !fullyCorrectQuestions.Contains(q.id))
                .Where(q => !displayedQuestionIds.Contains(q.id) || incorrectQuestions.Contains(q.id))
                .Take(maxDisplayedQuestions)
                .ToList();

            foreach (var q in questionsToDisplay) {
                GameObject item = Instantiate(questionPrefab, workSpace.GetChild(0));
                SetupQuestionUI(item, q);
                displayedQuestionIds.Add(q.id);
                totalQuestions++;
            }
        }
        private void SetupQuestionUI(GameObject item, QuizQuestion q) {
            item.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = q.question;
            item.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = "A: " + q.ansA;
            item.transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = "B: " + q.ansB;
            item.transform.GetChild(2).GetChild(2).GetComponent<TextMeshProUGUI>().text = "C: " + q.ansC;
            item.transform.GetChild(2).GetChild(3).GetComponent<TextMeshProUGUI>().text = "D: " + q.ansD;

            var qType = item.transform.GetChild(4).GetComponent<TextMeshProUGUI>();
            if (localization.TryGetValue(Settings.Language, out var labels)) {
                qType.text = q.correctAns.Count > 1 ? labels.MultipleChoice : labels.SingleChoice;
            }

            SetupToggle(item.transform.GetChild(3).GetChild(0).GetComponent<Toggle>(), q.id, "A");
            SetupToggle(item.transform.GetChild(3).GetChild(1).GetComponent<Toggle>(), q.id, "B");
            SetupToggle(item.transform.GetChild(3).GetChild(2).GetComponent<Toggle>(), q.id, "C");
            SetupToggle(item.transform.GetChild(3).GetChild(3).GetComponent<Toggle>(), q.id, "D");
        }
        private void SetupToggle(Toggle toggle, int questionId, string answer) {
            toggle.onValueChanged.AddListener((isOn) => SetAnswer(questionId, isOn, answer));
        }
        public void CheckAnswers() {
            checkButton.interactable = false;

            foreach (Transform questionItem in workSpace.GetChild(0)) {
                var questionText = questionItem.GetChild(1).GetComponent<TextMeshProUGUI>().text;
                var question = questionList.FirstOrDefault(q => q.question == questionText);
                if (question == null) continue;

                var selected = GetSelectedAnswersForQuestion(question.id);
                var isCorrect = selected.Count == question.correctAns.Count &&
                                !selected.Except(question.correctAns).Any();

                if (isCorrect) {
                    correctAnswers++;
                    CorrectQuizAnswers++;
                    fullyCorrectQuestions.Add(question.id);
                }
                else {
                    incorrectQuestions.Add(question.id);
                }

                UpdateToggleColor(questionItem, isCorrect);
            }

            PlayerRatingSystem.Instance.UpdateProgress();

            float percentage = (float)correctAnswers / totalQuestions * 100f;
            resultText.text = $"{correctAnswers} / {totalQuestions} ({percentage:0}%)";
            resultText.color = percentage switch {
                >= 80 => Color.green,
                >= 50 => Color.yellow,
                _ => Color.red
            };

            resultPanel.GetComponent<CanvasGroup>().DOFade(1, 0.5f);
            newButton.interactable = true;
        }
        public void LoadNewQuestions() {
            foreach (Transform child in workSpace.GetChild(0)) Destroy(child.gameObject);
            displayedQuestionIds.Clear();
            correctAnswers = 0;
            totalQuestions = 0;
            userAnswers.Clear();
            incorrectQuestions.Clear();

            DisplayQuestions();
            resultPanel.GetComponent<CanvasGroup>().DOFade(0, 0.5f);
            newButton.interactable = false;
            checkButton.interactable = true;
        }
        private void SetAnswer(int questionId, bool isSelected, string answer) {
            if (!userAnswers.ContainsKey(questionId)) userAnswers[questionId] = new();
            if (isSelected && !userAnswers[questionId].Contains(answer)) userAnswers[questionId].Add(answer);
            else if (!isSelected) userAnswers[questionId].Remove(answer);
        }
        private List<string> GetSelectedAnswersForQuestion(int questionId) =>
            userAnswers.TryGetValue(questionId, out var answers) ? answers : new();
        private void UpdateToggleColor(Transform questionItem, bool isCorrect) {
            foreach (var toggle in questionItem.GetChild(3).GetComponentsInChildren<Toggle>()) {
                if (toggle.isOn) {
                    var color = isCorrect ? Color.green : Color.red;
                    var block = toggle.colors;
                    block.normalColor = block.highlightedColor = block.pressedColor =
                        block.selectedColor = block.disabledColor = color;
                    toggle.colors = block;
                }
                toggle.interactable = false;
            }
        }
        private int LoadTotalQuizCount() {
            string lang = Settings.Language;
            string fileName = $"{languageFileName}_{lang}";
            TextAsset json = Resources.Load<TextAsset>(fileName);

            if (json == null) {
                Debug.LogError($"Brak pliku JSON: {fileName} w folderze Resources.");
                return 0;
            }

            QuizData data = JsonUtility.FromJson<QuizData>(json.text);

            if (data == null) {
                Debug.LogError("Nie udało się zdeserializować danych quizu.");
                return 0;
            }

            return data.newbieQuestions.Count + data.advancedQuestions.Count;
        }
    }
}