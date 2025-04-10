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
            List<QuizQuestion> filteredQuestions = questionList.Take(maxDisplayedQuestions).ToList();

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