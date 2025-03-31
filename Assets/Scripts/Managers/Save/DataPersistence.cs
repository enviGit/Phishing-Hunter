using UnityEngine;

namespace ph.Managers.Save {
    public class DataPersistence : MonoBehaviour {
        private GameData gameData;

        public static DataPersistence instance { get; private set; }

        private void Awake() {
            if (instance != null) {
                Debug.LogError("Found more than one Data Persistence Manager in the scene.");
            }

            instance = this;
        }
        public void NewGame() {
            this.gameData = new GameData();
        }
        public void LoadGame() {
            //TODO: Load any saved data from a file using the data handler

            if (this.gameData == null) {
                Debug.Log("No data was found. Initializing data to defaults.");
                NewGame();
            }

            //TODO: Push the loaded data to all other scripts that need it
        }
        public void SaveGame() {
            //TODO: Pass the data to other scripts so they can update it

            //TODO: Save that data to a file using the data handler
        }
    }
}
