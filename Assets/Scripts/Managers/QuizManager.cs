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

    [Serializable]
    public class LocalizedQuizData : QuizData {
        public string lang;
    }

    [Serializable]
    public class LocalizedQuizDataList {
        public List<LocalizedQuizData> items;
    }

    public class QuizManager : MonoBehaviour {
        [SerializeField] private TextAsset jsonFile;
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
        private LocalizedQuizData resLangData;
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
            List<LocalizedQuizData> resourcesData = null;

            if (jsonFile != null) {
                string wrappedJson = "{\"items\":" + jsonFile.text + "}";
                resourcesData = JsonUtility.FromJson<LocalizedQuizDataList>(wrappedJson).items;
            }
            else {
                Debug.LogError("Brak pliku questions.json w folderze Resources.");
            }

            resLangData = resourcesData?.FirstOrDefault(x => x.lang == Settings.Language);

            if (resLangData != null) {
                questionList = resLangData.newbieQuestions;
            }
            else {
                Debug.LogError("Brak danych dla wybranego języka.");
            }
        }
        private void DisplayQuestions() {
            // Pytania, które jeszcze nie zostały wyświetlone oraz pytania, które zostały odpowiedziane błędnie.
            List<QuizQuestion> questionsToDisplay;

            if (Settings.Difficulty == 0) {
                questionsToDisplay = questionList
    .Where(q => !fullyCorrectQuestions.Contains(q.id)) // <-- odfiltrowujemy w pełni zaliczone
    .Where(q => !displayedQuestionIds.Contains(q.id) || incorrectQuestions.Contains(q.id))
    .ToList();
            }
            else {
                questionsToDisplay = questionList
    .Where(q => !fullyCorrectQuestions.Contains(q.id))
    .Where(q => !displayedQuestionIds.Contains(q.id) || incorrectQuestions.Contains(q.id))
    .ToList();

                questionsToDisplay.AddRange(resLangData.advancedQuestions.Where(q => !fullyCorrectQuestions.Contains(q.id))
    .Where(q => !displayedQuestionIds.Contains(q.id) || incorrectQuestions.Contains(q.id))
    .ToList());
            }



            foreach (var question in questionsToDisplay) {
                GameObject questionItem = Instantiate(questionPrefab, workSpace.GetChild(0));

                TextMeshProUGUI questionText = questionItem.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI ansAText = questionItem.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI ansBText = questionItem.transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI ansCText = questionItem.transform.GetChild(2).GetChild(2).GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI ansDText = questionItem.transform.GetChild(2).GetChild(3).GetComponent<TextMeshProUGUI>();

                questionText.text = question.question;
                ansAText.text = "A: " + question.ansA;
                ansBText.text = "B: " + question.ansB;
                ansCText.text = "C: " + question.ansC;
                ansDText.text = "D: " + question.ansD;

                Toggle ansAToggle = questionItem.transform.GetChild(3).GetChild(0).GetComponent<Toggle>();
                Toggle ansBToggle = questionItem.transform.GetChild(3).GetChild(1).GetComponent<Toggle>();
                Toggle ansCToggle = questionItem.transform.GetChild(3).GetChild(2).GetComponent<Toggle>();
                Toggle ansDToggle = questionItem.transform.GetChild(3).GetChild(3).GetComponent<Toggle>();

                ansAToggle.onValueChanged.AddListener((isSelected) => SetAnswer(question.id, isSelected, "A"));
                ansBToggle.onValueChanged.AddListener((isSelected) => SetAnswer(question.id, isSelected, "B"));
                ansCToggle.onValueChanged.AddListener((isSelected) => SetAnswer(question.id, isSelected, "C"));
                ansDToggle.onValueChanged.AddListener((isSelected) => SetAnswer(question.id, isSelected, "D"));

                displayedQuestionIds.Add(question.id);
                totalQuestions++;
            }
        }
        public void CheckAnswers() {
            checkButton.interactable = false;

            foreach (Transform questionItem in workSpace.GetChild(0)) {
                TextMeshProUGUI questionText = questionItem.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                QuizQuestion question = resLangData.newbieQuestions
    .Concat(resLangData.advancedQuestions)
    .FirstOrDefault(q => questionText.text == q.question);

                if (question == null) continue;

                List<string> selectedAnswers = GetSelectedAnswersForQuestion(question.id);
                List<string> correctAnswersList = question.correctAns;

                bool isCorrect = selectedAnswers.Count == correctAnswersList.Count &&
                     !selectedAnswers.Except(correctAnswersList).Any();

                if (!isCorrect) {
                    incorrectQuestions.Add(question.id);
                }
                else {
                    if (!fullyCorrectQuestions.Contains(question.id)) {
                        fullyCorrectQuestions.Add(question.id);
                    }
                }

                UpdateToggleColor(questionItem, isCorrect);

                if (isCorrect) {
                    correctAnswers++;
                    CorrectQuizAnswers++;
                }

                if (correctAnswers > 0)
                    PlayerRatingSystem.Instance.UpdateProgress();
            }

            string result = $"{correctAnswers} / {totalQuestions} ({(correctAnswers / (float)totalQuestions * 100):0.0}%)";
            resultText.text = result;

            resultPanel.GetComponent<CanvasGroup>().DOFade(1, 0.5f);
            newButton.interactable = true;
        }
        public void LoadNewQuestions() {
            newButton.interactable = false;
            checkButton.interactable = true;

            foreach (Transform child in workSpace.GetChild(0)) {
                Destroy(child.gameObject);
            }

            displayedQuestionIds.Clear();
            correctAnswers = 0;
            totalQuestions = 0;
            userAnswers.Clear();
            incorrectQuestions.Clear();

            DisplayQuestions();
            resultPanel.GetComponent<CanvasGroup>().DOFade(0, 0.5f);
        }
        private void SetAnswer(int questionId, bool isSelected, string answer) {
            if (!userAnswers.ContainsKey(questionId)) {
                userAnswers[questionId] = new List<string>();
            }

            if (isSelected && !userAnswers[questionId].Contains(answer)) {
                userAnswers[questionId].Add(answer);
            }
            else if (!isSelected && userAnswers[questionId].Contains(answer)) {
                userAnswers[questionId].Remove(answer);
            }
        }
        private List<string> GetSelectedAnswersForQuestion(int questionId) {
            return userAnswers.ContainsKey(questionId) ? userAnswers[questionId] : new List<string>();
        }
        private void UpdateToggleColor(Transform questionItem, bool isCorrect) {
            Toggle[] toggles = questionItem.GetChild(3).GetComponentsInChildren<Toggle>();

            foreach (Toggle toggle in toggles) {
                if (toggle.isOn) {
                    ColorBlock colorBlock = new ColorBlock {
                        normalColor = isCorrect ? Color.green : Color.red,
                        highlightedColor = isCorrect ? Color.green : Color.red,
                        pressedColor = isCorrect ? Color.green : Color.red,
                        selectedColor = isCorrect ? Color.green : Color.red,
                        disabledColor = isCorrect ? Color.green : Color.red,
                        colorMultiplier = 1f
                    };
                    toggle.colors = colorBlock;
                }

                toggle.interactable = false;
            }
        }
        private int LoadTotalQuizCount() {
            List<LocalizedQuizData> resourcesData = null;

            if (jsonFile != null) {
                string wrappedJson = "{\"items\":" + jsonFile.text + "}";
                resourcesData = JsonUtility.FromJson<LocalizedQuizDataList>(wrappedJson).items;
            }
            else {
                Debug.LogError("Brak pliku questions.json w folderze Resources.");
            }

            string lang = Settings.Language.ToLower();

            var resLangData = resourcesData?.FirstOrDefault(x => x.lang == lang);

            if (resLangData != null) {
                List<QuizQuestion> newbieQuestions = resLangData.newbieQuestions;
                List<QuizQuestion> advancedQuestions = resLangData.advancedQuestions;
                return newbieQuestions.Count + advancedQuestions.Count;
            }
            else {
                Debug.LogError("Brak danych dla wybranego języka.");
                return 0;
            }
        }
    }
}