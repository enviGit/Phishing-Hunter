using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace ph.OS {
    public class FileBrowserUpdate : MonoBehaviour {
        public static FileBrowserUpdate instance;
        public RawImage rawImage;

        private void Awake() {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        public void OpenFileBrowser() {
            var bp = new BrowserProperties();
            bp.filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            bp.filterIndex = 0;

            new FileBrowser().OpenFileBrowser(bp, path => {
                StartCoroutine(LoadImage(path));
            });
        }

        IEnumerator LoadImage(string path) {
            using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(path)) {
                yield return uwr.SendWebRequest();

                if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError) {
                    Debug.Log(uwr.error);
                }
                else {
                    var uwrTexture = DownloadHandlerTexture.GetContent(uwr);
                    rawImage.texture = uwrTexture;
                }
            }
        }
    }
}
