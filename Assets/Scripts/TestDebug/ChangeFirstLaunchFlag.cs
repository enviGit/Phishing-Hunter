using ph.Managers;
using UnityEngine;

namespace ph.TestDebug {
    [ExecuteInEditMode]
    public class ChangeFirstLaunchFlag : MonoBehaviour {
        [ContextMenu("Set First Launch to TRUE")]
        private void SetFirstLaunchTrue() {
            Settings.IsFirstLaunch = true;
            Debug.Log("IsFirstLaunch has been set to: TRUE");
        }

        [ContextMenu("Set First Launch to FALSE")]
        private void SetFirstLaunchFalse() {
            Settings.IsFirstLaunch = false;
            Debug.Log("IsFirstLaunch has been set to: FALSE");
        }
    }
}
