using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ph.Managers.Save {
    public class DataPersistence : MonoBehaviour {
        [Header("File Storage Config")]
        [SerializeField] private string fileName = "savegame.dat";
        [SerializeField] private bool useEncryption = true;

        [Header("Auto Save Config")]
        [SerializeField] private float autoSaveTimeSeconds = 300f;

        private GameData gameData;
        private List<IDataPersistence> dataPersistenceObjects;
        private FileDataHandler dataHandler;
        private Coroutine autoSaveCoroutine;

        public static DataPersistence instance { get; private set; }

        private void Awake() {
            if (instance != null) {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);

            this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);
        }

        public void OnNewGameClicked() {
            this.gameData = new GameData();

            dataHandler.Save(this.gameData);

            StartGameplay();
        }

        public void OnLoadGameClicked() {
            this.gameData = dataHandler.Load();

            if (this.gameData == null) {
                Debug.LogWarning("Brak pliku zapisu mimo kliknięcia Load. Tworzę nową grę.");
                OnNewGameClicked();
                return;
            }

            StartGameplay();
        }

        public bool HasSaveFile() {
            return dataHandler.Load() != null;
        }

        private void StartGameplay() {
            GlobalSceneManager.Instance.SwitchToScene("Desktop");

            if (autoSaveCoroutine != null) StopCoroutine(autoSaveCoroutine);
            autoSaveCoroutine = StartCoroutine(AutoSave());
        }

        public void LoadDataOnObject(IDataPersistence persistenceObj) {
            if (this.gameData == null) {
                this.gameData = dataHandler.Load();
                if (this.gameData == null) this.gameData = new GameData();
            }

            persistenceObj.LoadData(this.gameData);
        }

        public void SaveGame() {
            if (this.gameData == null) return;

            IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
                .OfType<IDataPersistence>();

            foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects) {
                dataPersistenceObj.SaveData(ref gameData);
            }

            dataHandler.Save(gameData);
            Debug.Log($"Gra zapisana! {System.DateTime.Now}");
        }

        private IEnumerator AutoSave() {
            while (true) {
                yield return new WaitForSeconds(autoSaveTimeSeconds);
                SaveGame();
            }
        }
    }
}