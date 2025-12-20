using DG.Tweening;
using ph.Core;
using ph.Managers.Save;
using Random = UnityEngine.Random;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
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
        public List<string> attachments;
    }
    [Serializable]
    public class EmailData {
        public List<Email> newbieEmails;
        public List<Email> advancedEmails;
    }

    public class MailManager : MonoBehaviour, IDataPersistence {
        private string languageFileName = "emails";
        public Transform workSpace;
        public GameObject mailPrefab;
        public GameObject mailPreview;
        public GameObject imagePreview;
        private CanvasGroup mainPreviewCanvas;
        private CanvasGroup imagePreviewCanvas;
        private int maxDisplayedEmails = 0;
        private int minInitialDisplayedEmails = 3;
        private int maxInitialDisplayedEmails = 7;
        public int maxPossibleDisplayedEmails = 22;
        private float lastRefreshTime = 0f;
        private List<Email> emailList;
        private EmailData emailData;
        private Dictionary<Email, DateTime> generatedDates = new Dictionary<Email, DateTime>();
        private List<int> flaggedEmailIds = new List<int>();
        private List<int> correctlyMarkedEmails = new List<int>();
        private int correctMarksCount = 0;
        [SerializeField] private Sprite[] extensionSprites = new Sprite[3]; //0 - .png, 1 - .pdf, 2 - .exe
        public static int TotalMailCount { get; private set; }
        public static int CorrectMailAnswers { get; private set; }
        private bool isInitialized = false;


        private void Awake() {
            if (mailPreview) mainPreviewCanvas = mailPreview.GetComponent<CanvasGroup>();
            if (imagePreview) imagePreviewCanvas = imagePreview.GetComponent<CanvasGroup>();
        }
        private void Start() {
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
            this.flaggedEmailIds = data.flaggedMailIds;
            CorrectMailAnswers = data.correctMailsCount;
            this.correctMarksCount = data.correctMailsCount;

            InitializeManager();
        }

        public void SaveData(ref GameData data) {
            data.flaggedMailIds = this.flaggedEmailIds;
            data.correctMailsCount = CorrectMailAnswers;
        }
        private void InitializeManager() {
            if (isInitialized) return;

            maxDisplayedEmails = Random.Range(minInitialDisplayedEmails, maxInitialDisplayedEmails + 1);
            lastRefreshTime = Time.time;

            LoadEmailsFromFile();
            TotalMailCount = CountTotalEmails();
            Debug.Log($"Total Mail Count: {TotalMailCount}");

            AssignRandomDates();
            RefreshDisplay();

            isInitialized = true;
        }
        private void LoadEmailsFromFile() {
            string lang = Settings.Language;
            string fileName = $"{languageFileName}_{lang}";
            TextAsset json = Resources.Load<TextAsset>($"Data/{fileName}");

            if (json == null) {
                Debug.LogError($"Brak pliku JSON: {fileName} w folderze Resources.");
                return;
            }

            emailData = JsonUtility.FromJson<EmailData>(json.text);

            if (emailData == null) {
                Debug.LogError("Brak danych e-maili dla wybranego języka.");
                return;
            }

            emailList = emailData.newbieEmails
    .Concat(emailData.advancedEmails)
    .GroupBy(e => e.id)
    .Select(g => g.First())
    .ToList();

            foreach (var email in emailList) {
                if (string.IsNullOrEmpty(email.dateTime)) {
                    email.dateTime = GenerateRandomDate().ToString("yyyy-MM-dd HH:mm");
                }
            }
        }
        private int CountTotalEmails() {
            if (emailData == null) return 0;
            return emailData.newbieEmails.Count + emailData.advancedEmails.Count;
        }
        private List<Email> GetEmailsToDisplay(int count) {
            if (emailData == null) return new List<Email>();

            var availableNewbies = emailData.newbieEmails
                .Where(e => !flaggedEmailIds.Contains(e.id))
                .OrderBy(e => generatedDates[e])
                .ToList();

            var availableAdvanced = emailData.advancedEmails
                .Where(e => !flaggedEmailIds.Contains(e.id))
                .OrderBy(e => generatedDates[e])
                .ToList();

            if (Settings.Difficulty == 0) {
                return availableNewbies.Take(count).ToList();
            }
            else {
                int totalToLoad = count;
                int minAdvanced = totalToLoad / 4;
                int maxAdvanced = totalToLoad / 3 + 1;
                int advancedCount = Random.Range(minAdvanced, maxAdvanced + 1);

                advancedCount = Mathf.Min(advancedCount, availableAdvanced.Count);
                int newbieCount = totalToLoad - advancedCount;
                newbieCount = Mathf.Min(newbieCount, availableNewbies.Count);

                var chosenNewbies = availableNewbies.Take(newbieCount).ToList();
                var chosenAdvanced = availableAdvanced.Take(advancedCount).ToList();

                return chosenNewbies.Concat(chosenAdvanced)
                    .OrderBy(e => generatedDates[e])
                    .ToList();
            }
        }
        public void RefreshEmails() {
            float timeNow = Time.time;
            float secondsPassed = timeNow - lastRefreshTime;
            float mailInterval = 10f;
            int maxToAddInOneRefresh = 5;

            int intervals = Mathf.FloorToInt(secondsPassed / mailInterval);

            if (intervals > 0) {
                lastRefreshTime = timeNow;
                int newMails = 0;
                for (int i = 1; i <= intervals; i++) {
                    int minMail = Mathf.Clamp(i, 1, 3);
                    int maxMail = Mathf.Clamp(2 + i, minMail + 1, maxToAddInOneRefresh);
                    newMails += Random.Range(minMail, maxMail + 1);
                }
                newMails = Mathf.Clamp(newMails, 0, maxToAddInOneRefresh);

                maxDisplayedEmails = Mathf.Min(maxDisplayedEmails + newMails, maxPossibleDisplayedEmails);

                if (maxDisplayedEmails < maxPossibleDisplayedEmails)
                    Debug.Log($"Dodano {newMails} nowych maili.");

                RefreshDisplay();
            }
        }
        private void RefreshDisplay() {
            Transform contentRoot = workSpace.GetChild(0);
            foreach (Transform child in contentRoot) {
                Destroy(child.gameObject);
            }

            List<Email> finalPool = GetEmailsToDisplay(maxDisplayedEmails);

            foreach (var email in finalPool) {
                CreateMailObject(email, contentRoot);
            }
        }
        private void CreateMailObject(Email email, Transform parent) {
            GameObject mailItem = Instantiate(mailPrefab, parent);

            mailItem.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = email.sender;
            mailItem.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = email.subject;
            mailItem.transform.GetChild(5).GetComponent<TextMeshProUGUI>().text = generatedDates[email].ToString("dd.MM.yyyy HH:mm");
            mailItem.transform.GetChild(6).GetComponent<TextMeshProUGUI>().text = email.body;

            Email emailCopy = email;
            GameObject itemRef = mailItem;

            mailItem.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => OpenEmail(emailCopy, itemRef));
            mailItem.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => MarkEmailAsSafe(emailCopy, itemRef));
            mailItem.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() => MarkEmailAsPhishing(emailCopy, itemRef));
        }
        private void MarkEmailAsSafe(Email email, GameObject mailItem) {
            ProcessEmailDecision(email, mailItem, false);
        }

        private void MarkEmailAsPhishing(Email email, GameObject mailItem) {
            ProcessEmailDecision(email, mailItem, true);
        }

        private void ProcessEmailDecision(Email email, GameObject mailItem, bool markedAsPhishing) {
            flaggedEmailIds.Add(email.id);

            mailItem.transform.DOScale(0f, 0.25f).SetEase(Ease.OutQuad).OnKill(() => {
                if (mailItem != null) Destroy(mailItem);
            });

            ClosePreviews();

            bool isCorrect = (markedAsPhishing == email.isPhishing);

            if (isCorrect) {
                correctMarksCount++;
                CorrectMailAnswers++;
                PlayerRatingSystem.Instance.UpdateProgress();
            }
        }
        private void ClosePreviews() {
            if (mainPreviewCanvas != null && mainPreviewCanvas.alpha > 0) {
                mainPreviewCanvas.DOFade(0f, 0.25f).OnKill(() => {
                    if (mailPreview) mailPreview.SetActive(false);
                    if (imagePreview) imagePreview.SetActive(false);
                });
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

        private void OpenEmail(Email email, GameObject mailItem) {
            if (!mailPreview.activeSelf) mailPreview.SetActive(true);
            if (mailPreview.activeSelf) imagePreview.SetActive(false);

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

            Transform attachmentsParent = mailPreview.transform.GetChild(1).GetChild(6).GetChild(1);
            int maxSlots = attachmentsParent.childCount;

            for (int i = 0; i < maxSlots; i++) {
                Transform slot = attachmentsParent.GetChild(i);
                slot.gameObject.SetActive(false);
                slot.GetComponent<Image>().enabled = false;
            }

            if (email.attachments != null) {
                for (int i = 0; i < email.attachments.Count && i < maxSlots; i++) {
                    string fileName = email.attachments[i];
                    string extension = Path.GetExtension(fileName).ToLowerInvariant();
                    string baseName = Path.GetFileNameWithoutExtension(fileName);

                    Transform slot = attachmentsParent.GetChild(i);
                    Button button = slot.GetComponent<Button>();
                    Image imageComp = slot.GetComponent<Image>();
                    Sprite displaySprite = Resources.Load<Sprite>($"Images/Emails/{baseName}");

                    if (displaySprite != null) {
                        imageComp.sprite = displaySprite;
                        imageComp.enabled = true;
                        imageComp.preserveAspect = true;
                        slot.gameObject.SetActive(true);

                        Sprite highlightedSprite = extension switch {
                            ".png" => extensionSprites[0],
                            ".pdf" => extensionSprites[1],
                            ".exe" => extensionSprites[2],
                            _ => null
                        };

                        /// <summary>
                        /// List of attachment filenames related to the email.
                        /// </summary>
                        /// <remarks>
                        /// "attachments": [
                        ///     "exe.exe",         // Executable file (potentially malicious)
                        ///     "pdf.pdf",         // PDF document (e.g. fake report)
                        ///     "id1invoice1.png"  // Image file (e.g. fake invoice screenshot)
                        /// ]
                        /// </remarks>

                        if (highlightedSprite != null) {
                            SpriteState spriteState = button.spriteState;
                            spriteState.highlightedSprite = highlightedSprite;
                            button.spriteState = spriteState;
                        }

                        int capturedIndex = i;
                        button.onClick.RemoveAllListeners();

                        if (extension == ".png") {
                            button.onClick.AddListener(() => {
                                EventSystem.current.SetSelectedGameObject(null);

                                if (!imagePreview.activeSelf)
                                    imagePreview.SetActive(true);

                                imagePreviewCanvas.alpha = 0;
                                imagePreview.transform.localScale = Vector3.one * 0.8f;
                                imagePreviewCanvas.DOFade(1f, 0.25f).SetEase(Ease.OutQuad);
                                imagePreview.transform.DOScale(0.65f, 0.25f).SetEase(Ease.OutBack);
                                Image previewImage = imagePreview.transform.GetChild(2).GetComponent<Image>();
                                previewImage.sprite = displaySprite;
                            });
                        }
                        else if (extension == ".pdf") {
                            button.onClick.AddListener(() => {
                                EventSystem.current.SetSelectedGameObject(null);

                                // TODO: Canvas z PDF Readerem
                                OpenFilePopup(fileName, extension);
                            });
                        }
                        else {
                            button.onClick.AddListener(() => {
                                EventSystem.current.SetSelectedGameObject(null);

                                // TODO: Obsłużenie .exe
                                OpenFilePopup(fileName, extension);
                            });
                        }
                    }
                }
            }
        }
        private void OpenFilePopup(string fileName, string extension) {
            Debug.Log($"Otwieram plik {fileName} o rozszerzeniu {extension}");

            if (extension == ".pdf") {
                // TODO: pokaż okno z informacją, że otwierasz plik PDF
            }
            else if (extension == ".exe") {
                // TODO: ostrzeżenie o potencjalnie niebezpiecznym pliku
            }
        }
    }
}