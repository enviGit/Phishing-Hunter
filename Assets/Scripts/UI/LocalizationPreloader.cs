using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace ph.UI {
    public class LocalizationPreloader : MonoBehaviour {
        public string[] stringTableNames = { "Main Menu Labels", "Email Item Table", "Epilepsy Labels", "OS Labels" };
        public string[] assetTableNames = { };

        private IEnumerator Start() {
            // Czekaj na pełną inicjalizację systemu lokalizacji
            yield return LocalizationSettings.InitializationOperation;

            // Poczekaj na załadowanie wszystkich string tables
            foreach (var tableName in stringTableNames) {
                var loadOp = LocalizationSettings.StringDatabase.GetTableAsync(tableName);
                yield return new WaitUntil(() => loadOp.IsDone);
            }

            // Poczekaj na załadowanie wszystkich asset tables
            foreach (var tableName in assetTableNames) {
                var loadOp = LocalizationSettings.AssetDatabase.GetTableAsync(tableName);
                yield return new WaitUntil(() => loadOp.IsDone);
            }

            // Teraz możesz zainicjować lub odblokować UI albo przejść do właściwej sceny
            Debug.Log("All localization tables loaded, safe to show UI.");
            // Możesz np. włączyć canvas z menu lub wywołać event, że lokalizacje gotowe
        }
    }
}
