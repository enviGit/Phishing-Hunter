using DG.Tweening;
using ph.Managers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ph.Core.OS {
    [Serializable]
    public class Email {
        public int id;
        public string subject;
        public string sender;
        public string recipient;
        public string body;
        public bool isPhishing;
        public string dateTime;
    }
    [Serializable]
    public class EmailData {
        public List<Email> newbieEmails;
        public List<Email> advancedEmails;
    }
    [Serializable]
    public class LocalizedEmailData : EmailData {
        public string lang;
    }
    [Serializable]
    public class LocalizedEmailDataList {
        public List<LocalizedEmailData> items;
    }

    public class MailManager : MonoBehaviour {
        public TextAsset jsonFile;
        public Transform workSpace;
        public GameObject mailPrefab;
        public GameObject mailPreview;
        public float verticalSpacing = 25f;
        public int maxDisplayedEmails = 10;
        private List<Email> emailList;
        private List<LocalizedEmailData> resourcesData = null;
        private Dictionary<Email, DateTime> generatedDates = new Dictionary<Email, DateTime>();
        private Dictionary<int, bool> emailFlags = new Dictionary<int, bool>();

        private void Start() {
            LoadEmails();
            AssignRandomDates();
            DisplayEmails();
        }
        private void LoadEmails() {
            if (jsonFile != null) {
                string wrappedJson = "{\"items\":" + jsonFile.text + "}";
                resourcesData = JsonUtility.FromJson<LocalizedEmailDataList>(wrappedJson).items;
            }
            else {
                Debug.LogError("Brak pliku emails.json w folderze Resources.");
                return;
            }

            string selectedLanguage = Settings.Language == "pl" ? "pl" : "en";
            var resLangData = resourcesData?.FirstOrDefault(x => x.lang == selectedLanguage);

            if (resLangData == null) {
                Debug.LogError("Brak danych e-maili dla wybranego języka.");
                return;
            }

            if (Settings.Difficulty == 0) {
                emailList = resLangData.newbieEmails;
            }
            else {
                emailList = resLangData.advancedEmails;
            }

            foreach (var email in emailList) {
                if (string.IsNullOrEmpty(email.dateTime)) {
                    email.dateTime = GenerateRandomDate().ToString("yyyy-MM-dd HH:mm");
                }
            }
        }
        private DateTime GenerateRandomDate() {
            DateTime now = DateTime.Now;
            return now.AddDays(UnityEngine.Random.Range(-30, 0))
                      .AddHours(UnityEngine.Random.Range(-23, 0))
                      .AddMinutes(UnityEngine.Random.Range(-59, 0));
        }
        private void AssignRandomDates() {
            foreach (var email in emailList) {
                if (DateTime.TryParse(email.dateTime, out DateTime emailDate)) {
                    generatedDates[email] = emailDate;
                }
                else {
                    generatedDates[email] = GenerateRandomDate();
                }
            }
        }
        private void DisplayEmails() {
            string selectedLanguage = Settings.Language;

            var emailData = resourcesData
                .FirstOrDefault(e => e.lang == selectedLanguage);

            if (emailData != null) {
                List<Email> filteredEmails = (Settings.Difficulty == 0)
                    ? emailData.newbieEmails
                    : emailData.advancedEmails;

                filteredEmails = filteredEmails
                    .OrderBy(email => generatedDates[email])
                    .ToList();

                List<Email> displayedEmails = filteredEmails.Take(maxDisplayedEmails).ToList();

                float currentY = -5f;

                for (int i = displayedEmails.Count - 1; i >= 0; i--) {
                    GameObject mailItem = Instantiate(mailPrefab, workSpace.GetChild(0));

                    RectTransform rectTransform = mailItem.GetComponent<RectTransform>();
                    rectTransform.localPosition = new Vector3(0, currentY, 0);
                    currentY -= verticalSpacing;

                    TextMeshProUGUI senderText = mailItem.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
                    TextMeshProUGUI subjectText = mailItem.transform.GetChild(4).GetComponent<TextMeshProUGUI>();
                    TextMeshProUGUI dateTimeText = mailItem.transform.GetChild(5).GetComponent<TextMeshProUGUI>();
                    TextMeshProUGUI messageText = mailItem.transform.GetChild(6).GetComponent<TextMeshProUGUI>();
                    Button openButton = mailItem.transform.GetChild(0).GetComponent<Button>();

                    senderText.text = displayedEmails[i].sender;
                    subjectText.text = displayedEmails[i].subject;
                    dateTimeText.text = generatedDates[displayedEmails[i]].ToString("dd.MM.yyyy HH:mm");
                    messageText.text = displayedEmails[i].body;

                    openButton.onClick.AddListener(() => OpenEmail(mailItem));
                }
            }
        }
        private void OpenEmail(GameObject mailItem) {
            if (!mailPreview.activeSelf) {
                mailPreview.SetActive(true);
            }

            CanvasGroup canvasGroup = mailPreview.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
            mailPreview.transform.localScale = Vector3.one * 0.8f;
            canvasGroup.DOFade(1f, 0.25f).SetEase(Ease.OutQuad);
            mailPreview.transform.DOScale(0.65f, 0.25f).SetEase(Ease.OutBack);

            TextMeshProUGUI senderText = mailPreview.transform.GetChild(1).GetChild(2).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI subjectText = mailPreview.transform.GetChild(1).GetChild(3).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI dateTimeText = mailPreview.transform.GetChild(1).GetChild(4).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI messageText = mailPreview.transform.GetChild(1).GetChild(5).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();

            senderText.text = mailItem.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text;
            subjectText.text = mailItem.transform.GetChild(4).GetComponentInChildren<TextMeshProUGUI>().text;
            dateTimeText.text = mailItem.transform.GetChild(5).GetComponentInChildren<TextMeshProUGUI>().text;
            messageText.text = mailItem.transform.GetChild(6).GetComponentInChildren<TextMeshProUGUI>().text;
        }
    }
}