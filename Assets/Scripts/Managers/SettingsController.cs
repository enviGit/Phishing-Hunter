using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using TMPro;

namespace ph.Managers {
    public class SettingsMenu : MonoBehaviour {
        [Header("UI References")]
        public Slider mouseSensSlider;
        public Slider sfxSlider;
        public Slider musicSlider;
        public Slider brightnessSlider;
        public Toggle fullscreenToggle;
        public Slider resolutionSlider;
        public Toggle runInBgToggle;
        public Toggle vsyncToggle;
        public TMP_Dropdown qualityDropdown;
        public Slider languageSlider;

        [Header("Audio")]
        public AudioMixer musicMixer;
        public AudioMixer sfxMixer;

        [Header("Post Processing")]
        public Volume globalVolume;
        private ColorAdjustments brightnessComponent;

        private Resolution[] resolutions;
        private TextMeshProUGUI resolutionText;
        private TextMeshProUGUI brightnessText;
        private TextMeshProUGUI languageText;
        private TextMeshProUGUI mouseSensitivityText;
        private TextMeshProUGUI musicVolumeText;
        private TextMeshProUGUI sfxVolumeText;
        private float initialMouseSensitivity;
        private float initialSfxVolume;
        private float initialMusicVolume;
        private float initialBrightness;
        private bool initialFullScreen;
        private int initialResolutionIndex;
        private bool initialRunInBackground;
        private bool initialVSync;
        private int initialQualityPreset;
        private string initialLanguage;

        private void Start() {
            resolutionText = resolutionSlider.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            brightnessText = brightnessSlider.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            mouseSensitivityText = mouseSensSlider.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            languageText = languageSlider.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            musicVolumeText = musicSlider.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            sfxVolumeText = sfxSlider.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

            initialMouseSensitivity = Settings.Sensitivity;
            initialSfxVolume = Settings.SfxMixer;
            initialMusicVolume = Settings.MusicMixer;
            initialBrightness = Settings.Brightness;
            initialFullScreen = Settings.FullScreen;
            initialResolutionIndex = Settings.ResolutionIndex;
            initialRunInBackground = Settings.RunInBg;
            initialVSync = Settings.VSync;
            initialQualityPreset = Settings.QualityPreset;
            initialLanguage = Settings.Language;


            // Post-processing setup
            if (globalVolume.profile.TryGet(out ColorAdjustments colorAdjustments)) {
                brightnessComponent = colorAdjustments;
            }

            // Resolution setup
            resolutions = Screen.resolutions;
            resolutionSlider.maxValue = resolutions.Length - 1;
            resolutionSlider.minValue = 0;

            LoadSettings();

            // Assign listeners
            mouseSensSlider.onValueChanged.AddListener(val => {
                Settings.Sensitivity = val;
                UpdateMouseSensitivityText(val);
            });

            sfxSlider.onValueChanged.AddListener(val => {
                if (val < 1)
                    val = .001f;

                Settings.SfxMixer = val;
                sfxMixer.SetFloat("sfxVolume", Mathf.Log10(val / 100) * 20f);
                UpdateSfxVolumeText(val);
            });

            musicSlider.onValueChanged.AddListener(val => {
                if (val < 1)
                    val = .001f;

                Settings.MusicMixer = val;
                musicMixer.SetFloat("musicVolume", Mathf.Log10(val / 100) * 20f);
                UpdateMusicVolumeText(val);
            });

            brightnessSlider.onValueChanged.AddListener(val => {
                Settings.Brightness = val;
                if (brightnessComponent != null)
                    brightnessComponent.postExposure.value = val;
                UpdateBrightnessText(val);
            });

            fullscreenToggle.onValueChanged.AddListener(val => {
                Settings.FullScreen = val;
                Screen.fullScreen = val;
            });

            resolutionSlider.onValueChanged.AddListener(val => {
                Settings.ResolutionIndex = (int)val;
                Resolution res = resolutions[(int)val];
                Screen.SetResolution(res.width, res.height, Screen.fullScreen);
                UpdateResolutionText(res);
            });

            runInBgToggle.onValueChanged.AddListener(val => {
                Settings.RunInBg = val;
                Application.runInBackground = val;
            });

            vsyncToggle.onValueChanged.AddListener(val => {
                Settings.VSync = val;
                QualitySettings.vSyncCount = val ? 1 : 0;
            });

            qualityDropdown.onValueChanged.AddListener(val => {
                Settings.QualityPreset = val;
                QualitySettings.SetQualityLevel(val);
            });

            languageSlider.onValueChanged.AddListener(val => {
                Settings.Language = val == 0 ? "en" : "pl";
                UpdateLanguageText(val);
            });
        }

        #region UpdatingTextComponents
        private void UpdateResolutionText(Resolution res) {
            resolutionText.text = $"{res.width} x {res.height}";
        }

        private void UpdateMouseSensitivityText(float value) {
            mouseSensitivityText.text = $"{(int)value}";
        }

        private void UpdateSfxVolumeText(float value) {
            sfxVolumeText.text = $"{(int)value}";
        }

        private void UpdateMusicVolumeText(float value) {
            musicVolumeText.text = $"{(int)value}";
        }

        private void UpdateBrightnessText(float value) {
            brightnessText.text = $"{(int)value}";
        }

        private void UpdateLanguageText(float value) {
            languageText.text = value == 0 ? "English" : "Polski";
        }
        #endregion

        private void LoadSettings() {
            mouseSensSlider.value = Settings.Sensitivity;
            sfxSlider.value = Settings.SfxMixer;
            musicSlider.value = Settings.MusicMixer;
            brightnessSlider.value = Settings.Brightness;

            fullscreenToggle.isOn = Settings.FullScreen;
            resolutionSlider.value = Settings.ResolutionIndex;
            runInBgToggle.isOn = Settings.RunInBg;
            vsyncToggle.isOn = Settings.VSync;

            qualityDropdown.value = Settings.QualityPreset;
            languageSlider.value = Settings.Language == "pl" ? 1 : 0;

            if (brightnessComponent != null)
                brightnessComponent.postExposure.value = Settings.Brightness;

            Resolution res = resolutions[Settings.ResolutionIndex];
            Screen.SetResolution(res.width, res.height, Screen.fullScreen);

            musicMixer.SetFloat("musicVolume", Mathf.Lerp(-80f, 0f, Settings.MusicMixer / 100f));
            sfxMixer.SetFloat("sfxVolume", Mathf.Lerp(-80f, 0f, Settings.SfxMixer / 100f));

            Application.runInBackground = Settings.RunInBg;
            QualitySettings.vSyncCount = Settings.VSync ? 1 : 0;
            QualitySettings.SetQualityLevel(Settings.QualityPreset);
        }

        public void ApplySettings() {
            // Sensitivity
            Settings.Sensitivity = mouseSensSlider.value;
            PlayerPrefs.SetFloat("mouseSens", Settings.Sensitivity);

            // Audio
            Settings.SfxMixer = sfxSlider.value;
            Settings.MusicMixer = musicSlider.value;
            PlayerPrefs.SetFloat("sfxMixer", Settings.SfxMixer);
            PlayerPrefs.SetFloat("musicMixer", Settings.MusicMixer);
            sfxMixer.SetFloat("sfxVolume", Mathf.Lerp(-80f, 0f, sfxSlider.value / 100f));
            musicMixer.SetFloat("musicVolume", Mathf.Lerp(-80f, 0f, musicSlider.value / 100f));

            // Brightness
            Settings.Brightness = brightnessSlider.value;
            PlayerPrefs.SetFloat("brightness", Settings.Brightness);
            if (brightnessComponent != null)
                brightnessComponent.postExposure.value = brightnessSlider.value;

            // Fullscreen
            Settings.FullScreen = fullscreenToggle.isOn;
            PlayerPrefs.SetInt("fullScreen", Settings.FullScreen ? 1 : 0);
            Screen.fullScreen = fullscreenToggle.isOn;

            // Resolution
            Settings.ResolutionIndex = (int)resolutionSlider.value;
            PlayerPrefs.SetInt("resolutionIndex", Settings.ResolutionIndex);
            Resolution res = resolutions[(int)resolutionSlider.value];
            Screen.SetResolution(res.width, res.height, Screen.fullScreen);

            // Run in background
            Settings.RunInBg = runInBgToggle.isOn;
            PlayerPrefs.SetInt("runInBg", Settings.RunInBg ? 1 : 0);
            Application.runInBackground = runInBgToggle.isOn;

            // VSync
            Settings.VSync = vsyncToggle.isOn;
            PlayerPrefs.SetInt("vsyncCount", Settings.VSync ? 1 : 0);
            QualitySettings.vSyncCount = vsyncToggle.isOn ? 1 : 0;

            // Quality
            Settings.QualityPreset = qualityDropdown.value;
            PlayerPrefs.SetInt("quality", Settings.QualityPreset);
            QualitySettings.SetQualityLevel(qualityDropdown.value);

            // Language
            Settings.Language = languageSlider.value == 0 ? "en" : "pl";
            PlayerPrefs.SetString("language", Settings.Language);

            // Save all changes to PlayerPrefs
            PlayerPrefs.Save();
        }
        public void ResetToInitial() {
            // Resetowanie wartości do domyślnych ustawień

            // Sensitivity
            mouseSensSlider.value = initialMouseSensitivity;
            Settings.Sensitivity = initialMouseSensitivity;

            // Audio
            sfxSlider.value = initialSfxVolume;
            musicSlider.value = initialMusicVolume;
            Settings.SfxMixer = initialSfxVolume;
            Settings.MusicMixer = initialMusicVolume;
            sfxMixer.SetFloat("sfxVolume", Mathf.Lerp(-80f, 0f, initialSfxVolume / 100f));
            musicMixer.SetFloat("musicVolume", Mathf.Lerp(-80f, 0f, initialMusicVolume / 100f));

            // Brightness
            brightnessSlider.value = initialBrightness;
            Settings.Brightness = initialBrightness;
            if (brightnessComponent != null)
                brightnessComponent.postExposure.value = initialBrightness;

            // Fullscreen
            fullscreenToggle.isOn = initialFullScreen;
            Settings.FullScreen = initialFullScreen;
            Screen.fullScreen = initialFullScreen;

            // Resolution
            resolutionSlider.value = initialResolutionIndex;
            Settings.ResolutionIndex = initialResolutionIndex;
            Resolution res = resolutions[initialResolutionIndex];
            Screen.SetResolution(res.width, res.height, Screen.fullScreen);

            // Run in background
            runInBgToggle.isOn = initialRunInBackground;
            Settings.RunInBg = initialRunInBackground;
            Application.runInBackground = initialRunInBackground;

            // VSync
            vsyncToggle.isOn = initialVSync;
            Settings.VSync = initialVSync;
            QualitySettings.vSyncCount = initialVSync ? 1 : 0;

            // Quality
            qualityDropdown.value = initialQualityPreset;
            Settings.QualityPreset = initialQualityPreset;
            QualitySettings.SetQualityLevel(initialQualityPreset);

            // Language
            languageSlider.value = initialLanguage == "pl" ? 1 : 0;
            Settings.Language = initialLanguage;

            // Zapisz zmiany do PlayerPrefs
            PlayerPrefs.SetFloat("mouseSens", initialMouseSensitivity);
            PlayerPrefs.SetFloat("sfxMixer", initialSfxVolume);
            PlayerPrefs.SetFloat("musicMixer", initialMusicVolume);
            PlayerPrefs.SetFloat("brightness", initialBrightness);
            PlayerPrefs.SetInt("fullScreen", initialFullScreen ? 1 : 0);
            PlayerPrefs.SetInt("resolutionIndex", initialResolutionIndex);
            PlayerPrefs.SetInt("runInBg", initialRunInBackground ? 1 : 0);
            PlayerPrefs.SetInt("vsyncCount", initialVSync ? 1 : 0);
            PlayerPrefs.SetInt("quality", initialQualityPreset);
            PlayerPrefs.SetString("language", initialLanguage);

            // Zapisz wszystkie zmiany w PlayerPrefs
            PlayerPrefs.Save();
        }
        public void ResetToDefaults() {
            // Resetowanie wartości do domyślnych ustawień
            Settings.ResetToDefaults();

            // Ustawienie UI na domyślne wartości
            mouseSensSlider.value = Settings.Sensitivity;
            sfxSlider.value = Settings.SfxMixer;
            musicSlider.value = Settings.MusicMixer;
            brightnessSlider.value = Settings.Brightness;

            fullscreenToggle.isOn = Settings.FullScreen;
            resolutionSlider.value = Settings.ResolutionIndex;
            runInBgToggle.isOn = Settings.RunInBg;
            vsyncToggle.isOn = Settings.VSync;

            qualityDropdown.value = Settings.QualityPreset;
            languageSlider.value = Settings.Language == "pl" ? 1 : 0;

            // Ustawienie post-processingu
            if (brightnessComponent != null)
                brightnessComponent.postExposure.value = Settings.Brightness;

            // Ustawienie rozdzielczości
            Resolution res = Screen.resolutions[Settings.ResolutionIndex];
            Screen.SetResolution(res.width, res.height, Screen.fullScreen);

            // Ustawienie audio
            musicMixer.SetFloat("musicVolume", Mathf.Lerp(-80f, 0f, Settings.MusicMixer / 100f));
            sfxMixer.SetFloat("sfxVolume", Mathf.Lerp(-80f, 0f, Settings.SfxMixer / 100f));

            // Zaktualizowanie aplikacji
            Application.runInBackground = Settings.RunInBg;
            QualitySettings.vSyncCount = Settings.VSync ? 1 : 0;
            QualitySettings.SetQualityLevel(Settings.QualityPreset);
        }
    }
}
