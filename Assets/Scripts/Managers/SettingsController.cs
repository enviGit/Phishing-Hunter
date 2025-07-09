using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace ph.Managers {
    public class SettingsMenu : MonoBehaviour {
        [Header("Graphics")]
        [SerializeField] private Slider resolutionSlider;
        private Resolution[] filteredResolutions;
        private TextMeshProUGUI resolutionText;
        private int currentResolutionIndex = 0;
        private int originalResolutionIndex = 0;
        [SerializeField] private Slider fullscreenSlider;
        private Toggle fullscreenToggle;
        [SerializeField] private Material fullscreenMaterial;
        private bool originalFullscreen;
        [SerializeField] private Slider brightnessSlider;
        [SerializeField] private Slider vsyncSlider;
        private Toggle vsyncToggle;
        private int originalVsync;
        [SerializeField] private Material vsyncMaterial;
        [SerializeField] private TMP_Dropdown qualityDropdown;
        [SerializeField] private RenderPipelineAsset[] qualityLevels;
        private int originalQualityPreset;

        [Header("Post Processing")]
        [SerializeField] private Volume globalVolume;
        private ColorAdjustments brightnessComponent;
        private float originalBrightness;

        [Header("Audio")]
        [SerializeField] private Slider musicSlider;
        [SerializeField] private AudioMixer musicMixer;
        private float originalMusicVolume;
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private AudioMixer sfxMixer;
        private float originalSfxVolume;

        [Header("Accesibility")]
        [SerializeField] private TMP_Dropdown languageDropdown;
        private readonly string[] supportedLanguages = new string[] {
    "English",       // en
    "Polski",        // pl
    "Español",       // es
    "Français",      // fr
    "Deutsch",       // de
    "Italiano",      // it
    "Русский",       // ru
    "Português",     // pt-BR
    "한국어",         // ko
    "简体中文"        // zh-Hans
};
        private readonly string[] languageCodes = new string[] {
    "en", "pl", "es", "fr", "de", "it", "ru", "pt-BR", "ko", "zh-Hans"
};
        private readonly Dictionary<string, List<string>> qualityTranslations = new Dictionary<string, List<string>> {
    { "en", new List<string> { "High", "Medium", "Low" } },
    { "pl", new List<string> { "Wysoka", "Średnia", "Niska" } },
    { "es", new List<string> { "Alta", "Media", "Baja" } },
    { "fr", new List<string> { "Élevée", "Moyenne", "Faible" } },
    { "de", new List<string> { "Hoch", "Mittel", "Niedrig" } },
    { "it", new List<string> { "Alta", "Media", "Bassa" } },
    { "ru", new List<string> { "Высокое", "Среднее", "Низкое" } },
    { "pt-BR", new List<string> { "Alta", "Média", "Baixa" } },
    { "ko", new List<string> { "높음", "중간", "낮음" } },
    { "zh-Hans", new List<string> { "高", "中", "低" } }
};
        private int currentLanguageIndex = 0;
        private int originalLanguageIndex = 0;
        [SerializeField] private Slider sensitivitySlider;
        private float originalSensitivity;
        [SerializeField] private Slider runInBgSlider;
        private Toggle runInBgToggle;
        private bool originalRunInBg;
        [SerializeField] private Material runInBgMaterial;

        private void Awake() {
            vsyncToggle = vsyncSlider.transform.GetChild(0).GetComponent<Toggle>();
            runInBgToggle = runInBgSlider.transform.GetChild(0).GetComponent<Toggle>();

            LoadSettingsOnAwake();
        }
        private IEnumerator Start() {
            resolutionText = resolutionSlider.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            fullscreenToggle = fullscreenSlider.transform.GetChild(0).GetComponent<Toggle>();
            LoadSettingsOnStart();
            yield return LocalizationSettings.InitializationOperation;
            UpdateQualityDropdownLabels(Settings.Language);
        }

        #region Loading Settings
        private void LoadSettingsOnStart() {
            // TODO: Ograniczyć dostępne rozdzielczości do proporcji 16:9
            filteredResolutions = Array.FindAll(Screen.resolutions, resolution => resolution.width >= 800 && resolution.height >= 600);
            resolutionSlider.maxValue = filteredResolutions.Length - 1;
            originalResolutionIndex = Settings.ResolutionIndex;
            originalResolutionIndex = Mathf.Clamp(originalResolutionIndex, 0, filteredResolutions.Length - 1);
            currentResolutionIndex = originalResolutionIndex;
            SetResolution(currentResolutionIndex);
            resolutionSlider.value = currentResolutionIndex;
            resolutionSlider.onValueChanged.AddListener(OnResolutionSliderChanged);
            UpdateResolutionText(filteredResolutions[currentResolutionIndex]);
            originalFullscreen = Settings.FullScreen;
            Screen.fullScreen = originalFullscreen;
            fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggleChanged);
            fullscreenToggle.isOn = originalFullscreen;
            AnimateSlider(Settings.FullScreen, fullscreenSlider);
            originalBrightness = Settings.Brightness;
            brightnessSlider.value = originalBrightness;

            if (globalVolume.profile.TryGet(out ColorAdjustments colorAdjustments)) {
                brightnessComponent = colorAdjustments;
                brightnessComponent.postExposure.value = brightnessSlider.value;
                brightnessSlider.onValueChanged.AddListener(OnBrightnessSliderChanged);
            }

            originalQualityPreset = Settings.QualityPreset;
            qualityDropdown.value = originalQualityPreset;
            originalSensitivity = Settings.Sensitivity;
            SetSensitivity(originalSensitivity);
            originalLanguageIndex = Mathf.Clamp(Array.IndexOf(languageCodes, Settings.Language), 0, supportedLanguages.Length - 1);
            currentLanguageIndex = originalLanguageIndex;
            languageDropdown.value = originalLanguageIndex;
            languageDropdown.onValueChanged.AddListener(OnLanguageDropdownChanged);
            originalMusicVolume = Settings.MusicMixer;
            originalSfxVolume = Settings.SfxMixer;
            SetMusicVolume(originalMusicVolume);
            SetSfxVolume(originalSfxVolume);
        }
        private void LoadSettingsOnAwake() {
            originalVsync = Settings.VSync ? 1 : 0;
            QualitySettings.vSyncCount = originalVsync;
            vsyncToggle.onValueChanged.AddListener(OnVsyncToggleChanged);
            vsyncToggle.isOn = originalVsync != 0;
            AnimateSlider(Settings.VSync, vsyncSlider);
            originalRunInBg = Settings.RunInBg;
            Application.runInBackground = originalRunInBg;
            runInBgToggle.onValueChanged.AddListener(OnRunInBgToggleChanged);
            runInBgToggle.isOn = originalRunInBg;
            AnimateSlider(Settings.RunInBg, runInBgSlider);
        }
        #endregion

        #region Apply, Abort and Quit buttons
        public void ApplyAllChanges() {
            SetAndApplyResolution(currentResolutionIndex);
            ApplyFullscreen();
            ApplyBrightness();
            ApplyVsync();
            ApplyQuality();
            ApplyLanguage();
            ApplySensitivity();
            ApplyVolume();
            ApplyRunInBg();
            PlayerPrefs.Save();
        }
        public void AbortChanges() {
            RestoreResolution();
            RestoreFullscreen();
            RestoreBrightness();
            RestoreVsync();
            RestoreQuality();
            RestoreLanguage();
            RestoreSensitivity();
            RestoreVolume();
            RestoreRunInBg();
            PlayerPrefs.Save();
        }
        public void QuitGame() {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
        #endregion

        #region Settings

        #region Resolution
        private void SetResolution(int index) {
            currentResolutionIndex = index;
            UpdateResolutionText(filteredResolutions[currentResolutionIndex]);
        }
        private void RestoreResolution() {
            SetAndApplyResolution(originalResolutionIndex);
            resolutionSlider.value = originalResolutionIndex;
        }
        private void ApplyResolution(Resolution resolution) {
            UpdateResolutionText(resolution);
            Screen.SetResolution(resolution.width, resolution.height, Settings.FullScreen);
            originalResolutionIndex = currentResolutionIndex;
            Settings.ResolutionIndex = originalResolutionIndex;

            Debug.Log($"Resolution applied: {resolution.width} x {resolution.height}");
        }
        private void SetAndApplyResolution(int newResolutionIndex) {
            currentResolutionIndex = newResolutionIndex;
            ApplyResolution(filteredResolutions[currentResolutionIndex]);
        }
        private void UpdateResolutionText(Resolution res) {
            resolutionText.text = $"{res.width} x {res.height}";
        }
        private void OnResolutionSliderChanged(float value) {
            int newIndex = Mathf.RoundToInt(value);
            newIndex = Mathf.Clamp(newIndex, 0, filteredResolutions.Length - 1);

            if (newIndex != currentResolutionIndex) {
                SetResolution(newIndex);
            }
        }
        #endregion

        #region FullScreen
        private void RestoreFullscreen() {
            fullscreenToggle.isOn = originalFullscreen;
            Settings.FullScreen = fullscreenToggle.isOn;

            Debug.Log($"Fullscreen restored to: {(fullscreenToggle.isOn ? "enabled" : "disabled")}");
        }
        private void ApplyFullscreen() {
            originalFullscreen = fullscreenToggle.isOn;
            Settings.FullScreen = originalFullscreen;

            Debug.Log($"Fullscreen set to: {(originalFullscreen ? "enabled" : "disabled")}");
        }
        private void OnFullscreenToggleChanged(bool newValue) {
            Settings.FullScreen = newValue;
            Screen.fullScreen = !Screen.fullScreen;
            AnimateSlider(newValue, fullscreenSlider);
        }

        #endregion

        #region Brightness
        private void RestoreBrightness() {
            brightnessSlider.value = originalBrightness;
            Settings.Brightness = brightnessSlider.value;
        }
        private void ApplyBrightness() {
            originalBrightness = brightnessSlider.value;
            Settings.Brightness = originalBrightness;
        }
        private void OnBrightnessSliderChanged(float value) {
            brightnessComponent.postExposure.value = value;
        }
        #endregion

        #region Vsync
        private void RestoreVsync() {
            vsyncToggle.isOn = originalVsync != 0;
            Settings.VSync = vsyncToggle.isOn;
            QualitySettings.vSyncCount = originalVsync;
        }
        private void ApplyVsync() {
            originalVsync = vsyncToggle.isOn ? 1 : 0;
            Settings.VSync = vsyncToggle.isOn;
            QualitySettings.vSyncCount = originalVsync;
        }
        private void OnVsyncToggleChanged(bool newValue) {
            Settings.VSync = newValue;
            QualitySettings.vSyncCount = newValue ? 1 : 0;
            AnimateSlider(newValue, vsyncSlider);
        }
        #endregion

        #region Quality
        public void ChangeQualityLevel(int value) {
            QualitySettings.SetQualityLevel(value, true);
            QualitySettings.renderPipeline = qualityLevels[value];
        }
        private void RestoreQuality() {
            qualityDropdown.value = originalQualityPreset;
            Settings.QualityPreset = qualityDropdown.value;
        }
        private void ApplyQuality() {
            originalQualityPreset = qualityDropdown.value;
            Settings.QualityPreset = originalQualityPreset;
        }
        private void UpdateQualityDropdownLabels(string langCode) {
            if (qualityTranslations.TryGetValue(langCode, out var labels)) {
                qualityDropdown.ClearOptions();
                qualityDropdown.AddOptions(labels);
                qualityDropdown.value = Settings.QualityPreset;
                qualityDropdown.RefreshShownValue();
            }
        }
        #endregion

        #region Interface Language
        public void OnLanguageDropdownChanged(int value) {
            currentLanguageIndex = Mathf.Clamp(value, 0, supportedLanguages.Length - 1);
            Settings.Language = languageCodes[currentLanguageIndex];
            UpdateQualityDropdownLabels(languageCodes[currentLanguageIndex]);
            var locale = GetLocaleFromLanguageCode(Settings.Language);
            StartCoroutine(SetLocaleCoroutine(locale));
        }
        private void RestoreLanguage() {
            currentLanguageIndex = originalLanguageIndex;
            languageDropdown.value = originalLanguageIndex;
            Settings.Language = languageCodes[originalLanguageIndex];
            UpdateQualityDropdownLabels(languageCodes[originalLanguageIndex]);
            var locale = GetLocaleFromLanguageCode(Settings.Language);
            StartCoroutine(SetLocaleCoroutine(locale));
        }
        private void ApplyLanguage() {
            Settings.Language = languageCodes[currentLanguageIndex];
            originalLanguageIndex = currentLanguageIndex;
            UpdateQualityDropdownLabels(languageCodes[currentLanguageIndex]);
            var locale = GetLocaleFromLanguageCode(Settings.Language);
            StartCoroutine(SetLocaleCoroutine(locale));
        }
        private IEnumerator SetLocaleCoroutine(Locale locale) {
            var op = LocalizationSettings.InitializationOperation;
            yield return op;

            yield return LocalizationSettings.SelectedLocale = locale;
            UpdateQualityDropdownLabels(locale.Identifier.Code);
        }
        private Locale GetLocaleFromLanguageCode(string languageCode) {
            switch (languageCode) {
                case "en":
                    return LocalizationSettings.AvailableLocales.GetLocale("en");
                case "pl":
                    return LocalizationSettings.AvailableLocales.GetLocale("pl");
                case "es":
                    return LocalizationSettings.AvailableLocales.GetLocale("es");
                case "fr":
                    return LocalizationSettings.AvailableLocales.GetLocale("fr");
                case "de":
                    return LocalizationSettings.AvailableLocales.GetLocale("de");
                case "it":
                    return LocalizationSettings.AvailableLocales.GetLocale("it");
                case "ru":
                    return LocalizationSettings.AvailableLocales.GetLocale("ru");
                case "pt-BR":
                    return LocalizationSettings.AvailableLocales.GetLocale("pt-BR");
                case "ko":
                    return LocalizationSettings.AvailableLocales.GetLocale("ko");
                case "zh-Hans":
                    return LocalizationSettings.AvailableLocales.GetLocale("zh-Hans");
                default:
                    return LocalizationSettings.AvailableLocales.GetLocale("en"); // Domyślnie English
            }
        }
        #endregion

        #region Mouse Sensitivity
        private void SetSensitivity(float _value) {
            RefreshSensitivitySlider(_value);
        }
        public void SetFromSensitivitySlider() {
            SetSensitivity(sensitivitySlider.value);
        }
        private void RefreshSensitivitySlider(float _value) {
            sensitivitySlider.value = _value;
        }
        private void RestoreSensitivity() {
            SetSensitivity(originalSensitivity);
            Settings.Sensitivity = originalSensitivity;
        }
        private void ApplySensitivity() {
            originalSensitivity = sensitivitySlider.value;
            Settings.Sensitivity = originalSensitivity;
        }
        #endregion

        #region Volume
        private void SetMusicVolume(float _value) {
            if (_value < 1)
                _value = .001f;

            RefreshSlider(_value, musicSlider);
            musicMixer.SetFloat("musicVolume", Mathf.Log10(_value / 100) * 20f);
        }
        public void SetVolumeFromMusicSlider() {
            SetMusicVolume(musicSlider.value);
        }
        private void SetSfxVolume(float _value) {
            if (_value < 1)
                _value = .001f;

            RefreshSlider(_value, sfxSlider);
            sfxMixer.SetFloat("sfxVolume", Mathf.Log10(_value / 100) * 20f);
        }
        public void SetVolumeFromSfxSlider() {
            SetSfxVolume(sfxSlider.value);
        }
        private void RefreshSlider(float _value, Slider slider) {
            slider.value = _value;
        }
        private void RestoreVolume() {
            SetMusicVolume(originalMusicVolume);
            SetSfxVolume(originalSfxVolume);
            Settings.MusicMixer = originalMusicVolume;
            Settings.SfxMixer = originalSfxVolume;
        }
        private void ApplyVolume() {
            originalMusicVolume = musicSlider.value;
            originalSfxVolume = sfxSlider.value;
            Settings.MusicMixer = originalMusicVolume;
            Settings.SfxMixer = originalSfxVolume;
        }
        #endregion

        #region Run In Background
        private void RestoreRunInBg() {
            runInBgToggle.isOn = originalRunInBg;
            Settings.RunInBg = runInBgToggle.isOn;
        }
        private void ApplyRunInBg() {
            originalRunInBg = runInBgToggle.isOn;
            Settings.RunInBg = originalRunInBg;
        }
        private void OnRunInBgToggleChanged(bool newValue) {
            Settings.RunInBg = newValue;
            Application.runInBackground = !Application.runInBackground;
            AnimateSlider(newValue, runInBgSlider);
        }
        #endregion

        #region Utility Methods
        private void AnimateSlider(bool value, Slider slider) {
            Material targetMaterial = null;

            switch (slider.name) {
                case "Fullscreen":
                    targetMaterial = fullscreenMaterial;
                    break;
                case "VSync":
                    targetMaterial = vsyncMaterial;
                    break;
                case "RunInBg":
                    targetMaterial = runInBgMaterial;
                    break;
                default:
                    Debug.LogWarning($"Unrecognized slider: {slider.name}");
                    return;
            }

            if (targetMaterial == null) {
                Debug.LogWarning("Material not assigned!");
                return;
            }

            // Animacja koloru materiału
            Color fromColor = targetMaterial.GetColor("_Color");
            Color toColor = value
        ? new Color(0.2f, 0.5f, 1f)       // niebieski
        : new Color(0.15f, 0.15f, 0.15f); // szary

            DOTween.To(() => fromColor, x => {
                targetMaterial.SetColor("_Color", x);
            }, toColor, 0.3f)
            .SetEase(Ease.InOutQuad);

            // Animacja samego slidera
            float targetSliderValue = value ? 1f : 0f;
            slider.DOValue(targetSliderValue, 0.3f)
                .SetEase(Ease.InOutQuad);
        }
        #endregion

        #endregion
    }
}