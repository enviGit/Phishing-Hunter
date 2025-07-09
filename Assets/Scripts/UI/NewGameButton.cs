using ph.Managers;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ph.UI {
    public class NewGameButton : MonoBehaviour {
        [SerializeField] private float preloadDelay = 0.5f;

        private void Start() {
            StartCoroutine(DelayedPreload());

            GetComponent<Button>().onClick.AddListener(() => {
                if (SceneLoader.Instance != null) {
                    SceneLoader.Instance.ActivateNextScene();
                }
                else {
                    Debug.LogError("SceneLoader.Instance is null!");
                }
            });
        }
        private IEnumerator DelayedPreload() {
            yield return new WaitForSeconds(preloadDelay);

            if (SceneLoader.Instance != null) {
                SceneLoader.Instance.PreloadNextScene();
            }
            else {
                Debug.LogError("SceneLoader.Instance is null!");
            }
        }
    }
}
