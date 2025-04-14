using ph.Managers;
using UnityEngine;

namespace ph.TestDebug {
    [ExecuteInEditMode]
    public class ChangePreferredLanguage : MonoBehaviour {
        [ContextMenu("Set Language to English (en)")]
        private void SetLanguageToEnglish() {
            Settings.Language = "en";
            Debug.Log("Language has been set to: en (English)");
        }

        [ContextMenu("Set Language to Polish (pl)")]
        private void SetLanguageToPolish() {
            Settings.Language = "pl";
            Debug.Log("Language has been set to: pl (Polish)");
        }

        [ContextMenu("Set Language to Spanish (es)")]
        private void SetLanguageToSpanish() {
            Settings.Language = "es";
            Debug.Log("Language has been set to: es (Spanish)");
        }

        [ContextMenu("Set Language to French (fr)")]
        private void SetLanguageToFrench() {
            Settings.Language = "fr";
            Debug.Log("Language has been set to: fr (French)");
        }

        [ContextMenu("Set Language to German (de)")]
        private void SetLanguageToGerman() {
            Settings.Language = "de";
            Debug.Log("Language has been set to: de (German)");
        }


        [ContextMenu("Set Language to Russian (ru)")]
        private void SetLanguageToItalian() {
            Settings.Language = "ru";
            Debug.Log("Language has been set to: ru (Russian)");
        }

        [ContextMenu("Set Language to Portuguese (pt-BR)")]
        private void SetLanguageToPortuguese() {
            Settings.Language = "pt-BR";
            Debug.Log("Language has been set to: pt-BR (Portuguese)");
        }

        [ContextMenu("Set Language to Japanese (ja)")]
        private void SetLanguageToJapanese() {
            Settings.Language = "ja";
            Debug.Log("Language has been set to: ja (Japanese)");
        }

        [ContextMenu("Set Language to Korean (ko)")]
        private void SetLanguageToUkrainian() {
            Settings.Language = "ko";
            Debug.Log("Language has been set to: ko (Korean)");
        }

        [ContextMenu("Set Language to Simplified Chinese (zh-Hans)")]
        private void SetLanguageToChinese() {
            Settings.Language = "zh-Hans";
            Debug.Log("Language has been set to: zh-Hans (Simplified Chinese)");
        }
    }
}