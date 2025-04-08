using System;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace ph.Managers {
    public class SettingsMenu : MonoBehaviour {
        [Header("Graphics")]
        [SerializeField] private Slider resolutionSlider;
        private Resolution[] resolutions;
        private TextMeshProUGUI resolutionText;
        private int currentResolutionIndex = 0;
        private int originalResolutionIndex = 0;
        [SerializeField] private Toggle fullscreenToggle;
        private bool originalFullscreen;
        [SerializeField] private Slider brightnessSlider;
        [SerializeField] private Toggle vsyncToggle;
        private int originalVsync;
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
        [SerializeField] private Slider languageSlider;
        private readonly string[] supportedLanguages = new string[] {
    "English",       // en
    "Polski",        // pl
    "Español",       // es
    "Français",      // fr
    "Deutsch",       // de
    "Русский",       // ru
    "Português",     // pt-BR
    "日本語",         // ja
    "한국어",         // ko
    "简体中文"        // zh-Hans
};

        private readonly string[] languageCodes = new string[] {
    "en", "pl", "es", "fr", "de", "ru", "pt-BR", "ja", "ko", "zh-Hans"
};
        private TextMeshProUGUI languageText;
        private int currentLanguageIndex = 0;
        private int originalLanguageIndex = 0;
        [SerializeField] private Slider sensitivitySlider;
        private float originalSensitivity;
        [SerializeField] private Toggle runInBgToggle;
        private bool originalRunInBg;

        private void Awake() {
            LoadSettingsOnAwake();
        }
        private void Start() {
            resolutionText = resolutionSlider.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            languageText = languageSlider.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

            LoadSettingsOnStart();
        }

        #region Loading Settings
        private void LoadSettingsOnStart() {
            resolutions = Screen.resolutions;
            originalResolutionIndex = Settings.ResolutionIndex;
            currentResolutionIndex = originalResolutionIndex;
            SetResolution(currentResolutionIndex);
            resolutionSlider.value = (float)currentResolutionIndex / (resolutions.Length - 1);
            resolutionSlider.onValueChanged.AddListener(OnResolutionSliderChanged);
            UpdateResolutionText(resolutions[currentResolutionIndex]);
            originalFullscreen = Settings.FullScreen;
            Screen.fullScreen = originalFullscreen;
            fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggleChanged);
            fullscreenToggle.isOn = originalFullscreen;
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
            languageSlider.value = (float)currentLanguageIndex / (supportedLanguages.Length - 1);
            languageSlider.onValueChanged.AddListener(OnLanguageSliderChanged);
            UpdateLanguageText(currentLanguageIndex);
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
            originalRunInBg = Settings.RunInBg;
            Application.runInBackground = originalRunInBg;
            runInBgToggle.onValueChanged.AddListener(OnRunInBgToggleChanged);
            runInBgToggle.isOn = originalRunInBg;
        }
        #endregion

        #region Apply & Abort buttons
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
        #endregion

        public void QuitGame() {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }

        #region Settings

        #region Resolution
        private void SetResolution(int index) {
            currentResolutionIndex = index;
            UpdateResolutionText(resolutions[currentResolutionIndex]);
        }
        private void RestoreResolution() {
            SetAndApplyResolution(originalResolutionIndex);
            resolutionSlider.value = (float)originalResolutionIndex / (resolutions.Length - 1);
        }
        private void ApplyResolution(Resolution resolution) {
            UpdateResolutionText(resolution);
            Screen.SetResolution(resolution.width, resolution.height, Settings.FullScreen);
            originalResolutionIndex = currentResolutionIndex;
            Settings.ResolutionIndex = originalResolutionIndex;
        }
        private void SetAndApplyResolution(int newResolutionIndex) {
            currentResolutionIndex = newResolutionIndex;
            ApplyResolution(resolutions[currentResolutionIndex]);
        }
        private void UpdateResolutionText(Resolution res) {
            resolutionText.text = $"{res.width} x {res.height}";
        }
        private void OnResolutionSliderChanged(float value) {
            int newIndex = Mathf.RoundToInt(value * (resolutions.Length - 1));

            if (newIndex != currentResolutionIndex)
                SetResolution(newIndex);
        }
        #endregion

        #region FullScreen
        public void ChangeFullscreen() {
            Screen.fullScreen = !Screen.fullScreen;
        }
        private void RestoreFullscreen() {
            fullscreenToggle.isOn = originalFullscreen;
            Settings.FullScreen = fullscreenToggle.isOn;
        }
        private void ApplyFullscreen() {
            originalFullscreen = fullscreenToggle.isOn;
            Settings.FullScreen = originalFullscreen;
        }
        private void OnFullscreenToggleChanged(bool newValue) {
            Settings.FullScreen = newValue;
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
        public void ChangeVsync() {
            Settings.VSync = !Settings.VSync;
            QualitySettings.vSyncCount = Settings.VSync ? 1 : 0;
        }
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
        #endregion

        #region Interface Language
        public void OnLanguageSliderChanged(float value) {
            int maxIndex = supportedLanguages.Length - 1;
            int newIndex = Mathf.Clamp(Mathf.RoundToInt(value * maxIndex), 0, maxIndex);

            if (newIndex != currentLanguageIndex) {
                currentLanguageIndex = newIndex;
                UpdateLanguageText(currentLanguageIndex);
            }
        }
        private void UpdateLanguageText(int index) {
            languageText.text = supportedLanguages[index];
            string selectedLang = supportedLanguages[index];
            RectTransform textRect = languageText.GetComponent<RectTransform>();

            if (selectedLang == "日本語" || selectedLang == "한국어" || selectedLang == "简体中文") {
                textRect.anchoredPosition = new Vector2(textRect.anchoredPosition.x, 5f);
            }
            else {
                textRect.anchoredPosition = new Vector2(textRect.anchoredPosition.x, 0f);
            }
        }
        private void RestoreLanguage() {
            currentLanguageIndex = originalLanguageIndex;
            languageSlider.value = (float)originalLanguageIndex / (supportedLanguages.Length - 1);
            UpdateLanguageText(currentLanguageIndex);
        }
        private void ApplyLanguage() {
            Settings.Language = languageCodes[currentLanguageIndex];
            originalLanguageIndex = currentLanguageIndex;
            // Tutaj można dodać reload sceny lub system do dynamicznej zmiany języka
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
        public void ChangeRunInBg() {
            Application.runInBackground = !Application.runInBackground;
        }
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
        }
        #endregion

        #endregion
    }
}