using ph.Managers.Save;
using UnityEngine;

namespace ph.UI {
    public class MainMenu : MonoBehaviour {
        public void OnNewGameClicked() {
            DataPersistence.instance.NewGame();
        }
        public void OnLoadGameClicked() {
            DataPersistence.instance.LoadGame();
        }
    }
}
