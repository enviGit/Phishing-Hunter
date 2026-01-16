using DG.Tweening;
using ph.Managers.Save;
using ph.Player;
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

    public class QuizManager : MonoBehaviour, IDataPersistence {
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
        private int correctAnswersCurrentRound = 0;
        public static int TotalQuizCount { get; private set; }
        public static int CorrectQuizAnswers { get; private set; }
        private bool isInitialized = false;

        private void Awake() {
            if (resultPanel)
                resultText = resultPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        }
        private void Start() {
            if (resultPanel) resultPanel.GetComponent<CanvasGroup>().alpha = 0;
            if (checkButton) checkButton.interactable = true;
            if (newButton) newButton.interactable = false;

            if (DataPersistence.instance == null) {
                InitializeManager();
            }
        }
        private void OnEnable() {
            if (DataPersistence.instance != null) {
                DataPersistence.instance.LoadDataOnObject(this);
            }
        }
        public void LoadData(GameData data) {
            this.fullyCorrectQuestions = data.solvedQuizIds;
            CorrectQuizAnswers = data.correctQuizCount;
            InitializeManager();
        }

        public void SaveData(ref GameData data) {
            data.solvedQuizIds = this.fullyCorrectQuestions;
            data.correctQuizCount = CorrectQuizAnswers;
        }

        private void InitializeManager() {
            if (isInitialized) return;

            LoadQuestionsFromFile();
            TotalQuizCount = CountTotalQuestions();
            Debug.Log($"Total Quiz Count: {TotalQuizCount}");

            DisplayQuestions();
            isInitialized = true;
        }

        private void LoadQuestionsFromFile() {
            string lang = Settings.Language;
            string fileName = $"{languageFileName}_{lang}";
            TextAsset json = Resources.Load<TextAsset>($"Data/{fileName}");

            if (json == null) {
                Debug.LogError($"Brak pliku JSON: {fileName} w folderze Resources.");
                return;
            }

            QuizData quizData = JsonUtility.FromJson<QuizData>(json.text);
            if (quizData == null) return;

            questionList = Settings.Difficulty == 0
                ? quizData.newbieQuestions
                : quizData.newbieQuestions.Concat(quizData.advancedQuestions).ToList();
        }
        private int CountTotalQuestions() {
            string lang = Settings.Language;
            TextAsset json = Resources.Load<TextAsset>($"Data/{languageFileName}_{lang}");
            if (json == null) return 0;

            QuizData data = JsonUtility.FromJson<QuizData>(json.text);
            return data != null ? data.newbieQuestions.Count + data.advancedQuestions.Count : 0;
        }
        public void DisplayQuestions() {
            if (questionList == null) return;

            List<QuizQuestion> questionsToDisplay = questionList
                .Where(q => !fullyCorrectQuestions.Contains(q.id))
                .Where(q => !displayedQuestionIds.Contains(q.id) || incorrectQuestions.Contains(q.id))
                .Take(maxDisplayedQuestions)
                .ToList();

            foreach (var q in questionsToDisplay) {
                CreateQuestionObject(q);
                if (!displayedQuestionIds.Contains(q.id)) {
                    displayedQuestionIds.Add(q.id);
                }
            }
        }
        private void CreateQuestionObject(QuizQuestion q) {
            GameObject item = Instantiate(questionPrefab, workSpace.GetChild(0));

            var questionText = item.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            var answersParent = item.transform.GetChild(2);
            var typeText = item.transform.GetChild(4).GetComponent<TextMeshProUGUI>();
            var togglesParent = item.transform.GetChild(3);

            questionText.text = q.question;
            answersParent.GetChild(0).GetComponent<TextMeshProUGUI>().text = "A: " + q.ansA;
            answersParent.GetChild(1).GetComponent<TextMeshProUGUI>().text = "B: " + q.ansB;
            answersParent.GetChild(2).GetComponent<TextMeshProUGUI>().text = "C: " + q.ansC;
            answersParent.GetChild(3).GetComponent<TextMeshProUGUI>().text = "D: " + q.ansD;

            if (localization.TryGetValue(Settings.Language, out var labels)) {
                typeText.text = q.correctAns.Count > 1 ? labels.MultipleChoice : labels.SingleChoice;
            }

            SetupToggle(togglesParent.GetChild(0).GetComponent<Toggle>(), q.id, "A");
            SetupToggle(togglesParent.GetChild(1).GetComponent<Toggle>(), q.id, "B");
            SetupToggle(togglesParent.GetChild(2).GetComponent<Toggle>(), q.id, "C");
            SetupToggle(togglesParent.GetChild(3).GetComponent<Toggle>(), q.id, "D");
        }
        private void SetupToggle(Toggle toggle, int questionId, string answer) {
            toggle.onValueChanged.RemoveAllListeners();
            toggle.onValueChanged.AddListener((isOn) => SetAnswer(questionId, isOn, answer));
        }
        public void CheckAnswers() {
            checkButton.interactable = false;
            correctAnswersCurrentRound = 0;
            int totalQuestionsInRound = workSpace.GetChild(0).childCount;

            foreach (Transform questionItem in workSpace.GetChild(0)) {
                var questionText = questionItem.GetChild(1).GetComponent<TextMeshProUGUI>().text;
                var question = questionList.FirstOrDefault(q => q.question == questionText);
                if (question == null) continue;

                var selected = GetSelectedAnswersForQuestion(question.id);

                bool isCorrect = selected.Count == question.correctAns.Count &&
                                 !selected.Except(question.correctAns).Any();

                if (isCorrect) {
                    correctAnswersCurrentRound++;

                    if (!fullyCorrectQuestions.Contains(question.id)) {
                        CorrectQuizAnswers++;
                        fullyCorrectQuestions.Add(question.id);

                        if (incorrectQuestions.Contains(question.id))
                            incorrectQuestions.Remove(question.id);
                    }
                }
                else {
                    if (!incorrectQuestions.Contains(question.id))
                        incorrectQuestions.Add(question.id);
                }

                UpdateToggleColor(questionItem, isCorrect);
            }

            if (correctAnswersCurrentRound > 0) {
                PlayerRatingSystem.Instance.UpdateProgress();
            }

            ShowResult(correctAnswersCurrentRound, totalQuestionsInRound);
        }

        private void ShowResult(int correct, int total) {
            float percentage = total > 0 ? (float)correct / total * 100f : 0f;
            resultText.text = $"{correct} / {total} ({percentage:0}%)";
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

            userAnswers.Clear();
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
    }
}
