using ph.Managers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
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
        public string difficulty;
        public bool isAnsweredCorrectly;
    }

    [Serializable]
    public class QuizData {
        public List<QuizQuestion> newbieQuestions;
        public List<QuizQuestion> advancedQuestions;
    }

    public class QuizManager : MonoBehaviour {
        public TextAsset jsonFile;
        public Transform workSpace;
        public GameObject questionPrefab;
        public float verticalSpacing = 75f;
        public int maxDisplayedQuestions = 7;
        private List<QuizQuestion> questionList;

        private void Start() {
            LoadQuestions();
            DisplayQuestions();
        }

        private void LoadQuestions() {
            string persistentFilePath = Path.Combine(Application.persistentDataPath, "quiz.json");

            QuizData questionsFromResources = null;
            QuizData questionsFromPersistent = null;

            if (jsonFile != null) {
                questionsFromResources = JsonUtility.FromJson<QuizData>(jsonFile.text);
            }
            else {
                Debug.LogError("Brak pliku quiz.json w folderze Resources.");
            }

            if (File.Exists(persistentFilePath)) {
                string jsonTextFromPersistent = File.ReadAllText(persistentFilePath);
                questionsFromPersistent = JsonUtility.FromJson<QuizData>(jsonTextFromPersistent);
            }

            if (questionsFromPersistent == null || questionsFromResources.newbieQuestions.Count > questionsFromPersistent.newbieQuestions.Count || questionsFromResources.advancedQuestions.Count > questionsFromPersistent.advancedQuestions.Count) {
                Debug.Log("Nowe dane w Resources. Kopiowanie do persistentDataPath...");
                SaveQuestions(questionsFromResources);
                questionList = Settings.Difficulty == 0 ? questionsFromResources.newbieQuestions : questionsFromResources.advancedQuestions;
            }
            else {
                questionList = Settings.Difficulty == 0 ? questionsFromPersistent.newbieQuestions : questionsFromPersistent.advancedQuestions;
            }
        }

        private void SaveQuestions(QuizData quizData) {
            string filePath = Path.Combine(Application.persistentDataPath, "quiz.json");
            string json = JsonUtility.ToJson(quizData, true);
            File.WriteAllText(filePath, json);
            Debug.Log("Zapisano dane quizu do pliku w persistentDataPath.");
        }

        private void DisplayQuestions() {
            string selectedDifficulty = Settings.Difficulty == 0 ? "newbie" : "cybersecurity_analyst";
            List<QuizQuestion> filteredQuestions = questionList
                .Where(q => q.difficulty == selectedDifficulty)
                .Take(maxDisplayedQuestions)
                .ToList();

            float currentY = -5f;

            foreach (var question in filteredQuestions) {
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
            }
        }
    }
}
