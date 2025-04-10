using DG.Tweening;
using ph.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ph.Core.OS {
    [Serializable]
    public class QuizQuestion {
        public int id;
        public string question;
        public string ansA;
        public string ansB;
        public string ansC;
        public string ansD;
        public string correctAns;
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
        [SerializeField] private float verticalSpacing = 75f;
        [SerializeField] private int maxDisplayedQuestions = 7;
        [SerializeField] private GameObject resultPanel;
        private TextMeshProUGUI resultText;
        [SerializeField] private Button newButton;
        [SerializeField] private Button checkButton;
        private List<QuizQuestion> questionList;
        private List<int> displayedQuestionIds = new List<int>();
        private int correctAnswers = 0;
        private int totalQuestions = 0;
        private Dictionary<int, string> userAnswers = new Dictionary<int, string>();
        private List<int> incorrectQuestions = new List<int>();

        private void Start() {
            resultText = resultPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
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

            string lang = Settings.Language.ToLower();

            var resLangData = resourcesData?.FirstOrDefault(x => x.lang == lang);

            if (resLangData != null) {
                questionList = Settings.Difficulty == 0 ? resLangData.newbieQuestions : resLangData.advancedQuestions;
            }
            else {
                Debug.LogError("Brak danych dla wybranego języka.");
            }
        }
        private void DisplayQuestions() {
            // Pytania, które jeszcze nie zostały wyświetlone oraz pytania, które zostały odpowiedziane błędnie.
            List<QuizQuestion> questionsToDisplay = questionList
        .Where(q => !displayedQuestionIds.Contains(q.id) || incorrectQuestions.Contains(q.id))
        .Where(q => !userAnswers.ContainsKey(q.id) || userAnswers[q.id] != q.correctAns)
        .ToList();

            float currentY = -5f;

            foreach (var question in questionsToDisplay) {
                GameObject questionItem = Instantiate(questionPrefab, workSpace.GetChild(0));
                RectTransform rectTransform = questionItem.GetComponent<RectTransform>();
                rectTransform.localPosition = new Vector3(0, currentY, 0);
                currentY -= verticalSpacing;

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

            foreach (var question in questionList.Where(q => displayedQuestionIds.Contains(q.id))) {
                string selectedAnswer = GetSelectedAnswerForQuestion(question.id);
                bool isCorrect = selectedAnswer == question.correctAns;

                if (!isCorrect) {
                    incorrectQuestions.Add(question.id);
                }

                UpdateToggleColor(selectedAnswer, isCorrect);

                if (isCorrect) {
                    correctAnswers++;
                }
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
            incorrectQuestions.Clear();

            DisplayQuestions();
            resultPanel.GetComponent<CanvasGroup>().DOFade(0, 0.5f);
        }
        private void SetAnswer(int questionId, bool isSelected, string answer) {
            if (isSelected) {
                if (userAnswers.ContainsKey(questionId)) {
                    userAnswers[questionId] = answer;
                }
                else {
                    userAnswers.Add(questionId, answer);
                }
            }
            else {
                if (userAnswers.ContainsKey(questionId) && userAnswers[questionId] == answer) {
                    userAnswers.Remove(questionId);
                }
            }
        }
        private string GetSelectedAnswerForQuestion(int questionId) {
            if (userAnswers.ContainsKey(questionId)) {
                return userAnswers[questionId];
            }
            else {
                return "";
            }
        }
        private void UpdateToggleColor(string selectedAnswer, bool isCorrect) {
            foreach (Transform questionItem in workSpace.GetChild(0)) {
                Toggle ansAToggle = questionItem.GetChild(3).GetChild(0).GetComponent<Toggle>();
                Toggle ansBToggle = questionItem.GetChild(3).GetChild(1).GetComponent<Toggle>();
                Toggle ansCToggle = questionItem.GetChild(3).GetChild(2).GetComponent<Toggle>();
                Toggle ansDToggle = questionItem.GetChild(3).GetChild(3).GetComponent<Toggle>();

                ColorBlock colorBlock = new ColorBlock {
                    normalColor = isCorrect ? Color.green : Color.red,
                    highlightedColor = isCorrect ? Color.green : Color.red,
                    pressedColor = isCorrect ? Color.green : Color.red,
                    selectedColor = isCorrect ? Color.green : Color.red,
                    disabledColor = isCorrect ? Color.green : Color.red,
                    colorMultiplier = 1f
                };

                switch (selectedAnswer) {
                    case "A":
                        ansAToggle.colors = colorBlock;
                        break;
                    case "B":
                        ansBToggle.colors = colorBlock;
                        break;
                    case "C":
                        ansCToggle.colors = colorBlock;
                        break;
                    case "D":
                        ansDToggle.colors = colorBlock;
                        break;
                }

                ansAToggle.interactable = false;
                ansBToggle.interactable = false;
                ansCToggle.interactable = false;
                ansDToggle.interactable = false;
            }
        }
    }
}