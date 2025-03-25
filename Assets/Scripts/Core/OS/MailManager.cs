using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ph.Managers;

namespace ph.Core.OS {
    public class MailManager : MonoBehaviour {
        public TextAsset jsonFile;
        public Transform workSpace;
        public GameObject mailPrefab;
        public float verticalSpacing = 25f;
        private List<Email> emailList;

        private void Start() {
            LoadEmails();
            DisplayEmails();
        }

        private void LoadEmails() {
            if (jsonFile != null) {
                emailList = JsonUtility.FromJson<EmailList>(jsonFile.text).emails;
            }
            else {
                Debug.LogError("Brak pliku mails.json w folderze Resources.");
                emailList = new List<Email>();
            }
        }
        private string GenerateRandomDate() {
            DateTime now = DateTime.Now;
            DateTime randomDate = now.AddDays(UnityEngine.Random.Range(-30, 0))
                                 .AddHours(UnityEngine.Random.Range(-23, 0))
                                 .AddMinutes(UnityEngine.Random.Range(-59, 0));
            return randomDate.ToString("dd.MM.yyyy HH:mm");
        }
        private void DisplayEmails() {
            string selectedDifficulty = SettingsManager.Difficulty == 0 ? "newbie" : "cybersecurity_analyst";
            float currentY = -5f;

            foreach (var email in emailList) {
                if (email.difficulty != selectedDifficulty) continue;

                GameObject mailItem = Instantiate(mailPrefab, workSpace.GetChild(0));

                RectTransform rectTransform = mailItem.GetComponent<RectTransform>();
                rectTransform.localPosition = new Vector3(0, currentY, 0);
                currentY -= verticalSpacing;

                TextMeshProUGUI senderText = mailItem.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI subjectText = mailItem.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI dateTimeText = mailItem.transform.GetChild(2).GetComponent<TextMeshProUGUI>();

                senderText.text = email.sender;
                subjectText.text = email.subject;
                dateTimeText.text = GenerateRandomDate();
            }
        }
    }
}
