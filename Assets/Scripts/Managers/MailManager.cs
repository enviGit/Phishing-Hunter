using DG.Tweening;
using ph.Core;
using Random = UnityEngine.Random;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ph.Managers {
    [Serializable]
    public class Email {
        public int id;
        public string subject;
        public string sender;
        public string body;
        public bool isPhishing;
        public string dateTime;
        public string tag;
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
        private CanvasGroup mainPreviewCanvas;
        public int maxDisplayedEmails = 10;
        private List<Email> emailList;
        private List<LocalizedEmailData> resourcesData = null;
        private Dictionary<Email, DateTime> generatedDates = new Dictionary<Email, DateTime>();
        private List<int> flaggedEmailIds = new List<int>();
        private List<int> correctlyMarkedEmails = new List<int>();
        private int correctMarksCount = 0;
        public static int TotalMailCount { get; private set; }
        public static int CorrectMailAnswers { get; private set; }

        private void Start() {
            mainPreviewCanvas = mailPreview.GetComponent<CanvasGroup>();
            CorrectMailAnswers = 0;
            LoadEmails();
            TotalMailCount = LoadTotalMailCount();
            Debug.Log($"Total Mail Count: {TotalMailCount}");
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

            var resLangData = resourcesData?.FirstOrDefault(x => x.lang == Settings.Language);

            if (resLangData == null) {
                Debug.LogError("Brak danych e-maili dla wybranego języka.");
                return;
            }

            emailList = resLangData.newbieEmails
    .Concat(resLangData.advancedEmails)
    .GroupBy(e => e.id)
    .Select(g => g.First())
    .ToList();

            foreach (var email in emailList) {
                if (string.IsNullOrEmpty(email.dateTime)) {
                    email.dateTime = GenerateRandomDate().ToString("yyyy-MM-dd HH:mm");
                }
            }
        }
        private DateTime GenerateRandomDate() {
            DateTime now = DateTime.Now;
            return now.AddDays(Random.Range(-30, 0))
                      .AddHours(Random.Range(-23, 0))
                      .AddMinutes(Random.Range(-59, 0));
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
            .Where(email => !flaggedEmailIds.Contains(email.id))
            .OrderBy(email => generatedDates[email])
            .ToList();

                List<Email> displayedEmails = filteredEmails.Take(maxDisplayedEmails).ToList();

                for (int i = displayedEmails.Count - 1; i >= 0; i--) {
                    GameObject mailItem = Instantiate(mailPrefab, workSpace.GetChild(0));

                    TextMeshProUGUI senderText = mailItem.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
                    TextMeshProUGUI subjectText = mailItem.transform.GetChild(4).GetComponent<TextMeshProUGUI>();
                    TextMeshProUGUI dateTimeText = mailItem.transform.GetChild(5).GetComponent<TextMeshProUGUI>();
                    TextMeshProUGUI messageText = mailItem.transform.GetChild(6).GetComponent<TextMeshProUGUI>();
                    Button openButton = mailItem.transform.GetChild(0).GetComponent<Button>();

                    Email emailCopy = displayedEmails[i];

                    senderText.text = displayedEmails[i].sender;
                    subjectText.text = displayedEmails[i].subject;
                    dateTimeText.text = generatedDates[displayedEmails[i]].ToString("dd.MM.yyyy HH:mm");
                    messageText.text = displayedEmails[i].body;

                    openButton.onClick.AddListener(() => OpenEmail(emailCopy, mailItem));

                    Button safeButton = mailItem.transform.GetChild(1).GetComponent<Button>();
                    Button phishingButton = mailItem.transform.GetChild(2).GetComponent<Button>();
                    safeButton.onClick.AddListener(() => MarkEmailAsSafe(emailCopy, mailItem));
                    phishingButton.onClick.AddListener(() => MarkEmailAsPhishing(emailCopy, mailItem));
                }
            }
        }
        private void OpenEmail(Email email, GameObject mailItem) {
            if (!mailPreview.activeSelf) {
                mailPreview.SetActive(true);
            }

            mainPreviewCanvas.alpha = 0;
            mailPreview.transform.localScale = Vector3.one * 0.8f;
            mainPreviewCanvas.DOFade(1f, 0.25f).SetEase(Ease.OutQuad);
            mailPreview.transform.DOScale(0.65f, 0.25f).SetEase(Ease.OutBack);

            TextMeshProUGUI senderText = mailPreview.transform.GetChild(1).GetChild(2).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI subjectText = mailPreview.transform.GetChild(1).GetChild(3).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI dateTimeText = mailPreview.transform.GetChild(1).GetChild(4).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI messageText = mailPreview.transform.GetChild(1).GetChild(5).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();

            senderText.text = mailItem.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text;
            subjectText.text = mailItem.transform.GetChild(4).GetComponentInChildren<TextMeshProUGUI>().text;
            dateTimeText.text = mailItem.transform.GetChild(5).GetComponentInChildren<TextMeshProUGUI>().text;
            messageText.text = mailItem.transform.GetChild(6).GetComponentInChildren<TextMeshProUGUI>().text;

            Button safeButton = mailPreview.transform.GetChild(1).GetChild(0).GetComponent<Button>();
            Button phishingButton = mailPreview.transform.GetChild(1).GetChild(1).GetComponent<Button>();
            safeButton.onClick.AddListener(() => MarkEmailAsSafe(email, mailItem));
            phishingButton.onClick.AddListener(() => MarkEmailAsPhishing(email, mailItem));
        }
        private void MarkEmailAsSafe(Email email, GameObject mailItem) {
            flaggedEmailIds.Add(email.id);

            mainPreviewCanvas.DOFade(0f, 0.25f).SetEase(Ease.OutQuad).OnKill(() => {
                mailPreview.SetActive(false);
            });

            mailItem.transform.DOScale(0f, 0.25f).SetEase(Ease.OutQuad).OnKill(() => {
                if (mailItem != null) Destroy(mailItem);
            });

            if (email.isPhishing) {
                return;
            }

            correctMarksCount++;
            CorrectMailAnswers++;
            correctlyMarkedEmails.Add(email.id);
            PlayerRatingSystem.Instance.UpdateProgress();
        }
        private void MarkEmailAsPhishing(Email email, GameObject mailItem) {
            flaggedEmailIds.Add(email.id);

            mainPreviewCanvas.DOFade(0f, 0.25f).SetEase(Ease.OutQuad).OnKill(() => {
                mailPreview.SetActive(false);
            });

            mailItem.transform.DOScale(0f, 0.25f).SetEase(Ease.OutQuad).OnKill(() => {
                if (mailItem != null) Destroy(mailItem);
            });

            if (!email.isPhishing) {
                return;
            }

            correctMarksCount++;
            CorrectMailAnswers++;
            correctlyMarkedEmails.Add(email.id);
            PlayerRatingSystem.Instance.UpdateProgress();
        }
        private int LoadTotalMailCount() {
            if (resourcesData == null) {
                return 0;
            }

            var langData = resourcesData.FirstOrDefault(x => x.lang == Settings.Language);

            if (langData == null) {
                Debug.LogError("Brak danych maili dla wybranego języka.");
                return 0;
            }

            int count = 0;
            if (langData.newbieEmails != null) {
                count += langData.newbieEmails.Count;
            }

            return count;
        }
        public void RefreshEmails() {
            Transform contentRoot = workSpace.GetChild(0);
            int currentEmailCount = contentRoot.childCount;

            mainPreviewCanvas.DOFade(0f, 0.25f).SetEase(Ease.OutQuad).OnKill(() => {
                mailPreview.SetActive(false);
            });

            for (int i = 0; i < currentEmailCount; i++) {
                Destroy(contentRoot.GetChild(i).gameObject);
            }

            var langData = resourcesData?.FirstOrDefault(x => x.lang == Settings.Language);

            if (langData == null) {
                Debug.LogError("Brak danych emaili dla języka: " + Settings.Language);
                return;
            }

            List<Email> finalEmailPool;

            if (Settings.Difficulty == 0) {
                finalEmailPool = langData.newbieEmails
                    .Where(e => !flaggedEmailIds.Contains(e.id))
                    .OrderBy(e => generatedDates[e])
                    .Take(maxDisplayedEmails)
                    .ToList();
            }
            else {
                var availableNewbies = langData.newbieEmails
            .Where(e => !flaggedEmailIds.Contains(e.id) && !correctlyMarkedEmails.Contains(e.id))
            .OrderBy(e => generatedDates[e])
            .ToList();

                var availableAdvanced = langData.advancedEmails
            .Where(e => !flaggedEmailIds.Contains(e.id))
            .OrderBy(e => generatedDates[e])
            .ToList();

                int totalToLoad = maxDisplayedEmails;

                int minAdvanced = totalToLoad / 4;
                int maxAdvanced = totalToLoad / 3 + 1;
                int advancedCount = Random.Range(minAdvanced, maxAdvanced + 1);
                int newbieCount = totalToLoad - advancedCount;

                var chosenNewbies = availableNewbies.Take(newbieCount).ToList();
                int remaining = totalToLoad - chosenNewbies.Count;

                var chosenAdvanced = availableAdvanced.Take(remaining).ToList();

                finalEmailPool = chosenNewbies.Concat(chosenAdvanced).ToList();
            }

            foreach (var email in finalEmailPool) {
                GameObject mailItem = Instantiate(mailPrefab, contentRoot);

                mailItem.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = email.sender;
                mailItem.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = email.subject;
                mailItem.transform.GetChild(5).GetComponent<TextMeshProUGUI>().text = generatedDates[email].ToString("dd.MM.yyyy HH:mm");
                mailItem.transform.GetChild(6).GetComponent<TextMeshProUGUI>().text = email.body;

                Email emailCopy = email;
                mailItem.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => OpenEmail(emailCopy, mailItem));
                mailItem.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => MarkEmailAsSafe(emailCopy, mailItem));
                mailItem.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() => MarkEmailAsPhishing(emailCopy, mailItem));
            }
        }
    }
}