#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Envi.VibrantIcons
{
    /// <summary>
    /// Main Editor Window for the Vibrant Icons tool.
    /// Handles the tabs navigation (Auto vs Manual) and initialization of the settings file.
    /// </summary>
    public class VibrantIconsWindow : EditorWindow
    {
        #region Fields

        private VibrantIconsSettings _settings;
        private VisualElement _contentContainer;
        private VibrantIconsManualView _manualView;
        private ToolbarToggle _btnAuto;
        private ToolbarToggle _btnManual;

        #endregion

        #region Window Lifecycle

        [MenuItem("Window/Vibrant Icons")]
        public static void ShowWindow()
        {
            var wnd = GetWindow<VibrantIconsWindow>();
            wnd.titleContent = new GUIContent("Vibrant Icons");
            wnd.minSize = new Vector2(475, 550);
        }

        public void CreateGUI()
        {
            _settings = VibrantIconsSettings.GetOrCreateSettings();
            _manualView = new VibrantIconsManualView();

            VisualElement root = rootVisualElement;
            root.Clear();

            var toolbarContainer = new VisualElement();
            toolbarContainer.style.paddingTop = 5;
            toolbarContainer.style.paddingBottom = 5;
            toolbarContainer.style.paddingLeft = 5;
            toolbarContainer.style.paddingRight = 5;
            toolbarContainer.style.backgroundColor = new Color(0, 0, 0, 0.1f);

            var toolbar = new Toolbar();
            toolbar.style.height = 30;
            toolbar.style.backgroundColor = Color.clear;

            _btnAuto = new ToolbarToggle() { text = "Auto Rules", value = true };
            _btnAuto.style.flexGrow = 1;
            _btnAuto.style.unityTextAlign = TextAnchor.MiddleCenter;
            _btnAuto.style.unityFontStyleAndWeight = FontStyle.Bold;

            _btnManual = new ToolbarToggle() { text = "Manual Mode" };
            _btnManual.style.flexGrow = 1;
            _btnManual.style.unityTextAlign = TextAnchor.MiddleCenter;
            _btnManual.style.unityFontStyleAndWeight = FontStyle.Bold;

            _btnAuto.RegisterValueChangedCallback(evt => {
                if (evt.newValue) {
                    _btnManual.value = false;
                    ShowAutoView();
                } else if (!_btnManual.value) _btnAuto.value = true;
            });

            _btnManual.RegisterValueChangedCallback(evt => {
                if (evt.newValue) {
                    _btnAuto.value = false;
                    ShowManualView();
                } else if (!_btnAuto.value) _btnManual.value = true;
            });

            toolbar.Add(_btnAuto);
            toolbar.Add(_btnManual);

            toolbarContainer.Add(toolbar);
            root.Add(toolbarContainer);

            _contentContainer = new VisualElement();
            _contentContainer.style.flexGrow = 1;
            _contentContainer.style.paddingTop = 15;
            _contentContainer.style.paddingBottom = 15;
            _contentContainer.style.paddingLeft = 15;
            _contentContainer.style.paddingRight = 15;
            root.Add(_contentContainer);

            ShowAutoView();
        }

        #endregion

        #region View Logic

        private void ShowAutoView()
        {
            _contentContainer.Clear();

            var titleLabel = new Label("Auto Rules Manager");
            titleLabel.style.fontSize = 20;
            titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            titleLabel.style.marginBottom = 5;
            titleLabel.style.color = new Color(0.9f, 0.9f, 0.9f);
            titleLabel.style.alignSelf = Align.Center;
            _contentContainer.Add(titleLabel);

            var separator = new VisualElement();
            separator.style.height = 1;
            separator.style.backgroundColor = new Color(0.5f, 0.5f, 0.5f, 0.3f);
            separator.style.marginBottom = 15;
            separator.style.marginLeft = 5;
            separator.style.marginRight = 5;
            _contentContainer.Add(separator);

            if (_settings == null)
            {
                var helpBox = new HelpBox("Settings file not found (VibrantIconsSettings).", HelpBoxMessageType.Warning);
                _contentContainer.Add(helpBox);

                var createBtn = new Button(() =>
                {
                    CreateSettingsFile();
                    _settings = VibrantIconsSettings.GetOrCreateSettings();
                    ShowAutoView();
                })
                { text = "Create Settings File" };

                createBtn.style.marginTop = 10;
                createBtn.style.height = 30;
                _contentContainer.Add(createBtn);
                return;
            }

            var scanButton = new Button(() => { VibrantIconsProcessor.ApplyIconsToAllScripts(); })
            { text = "APPLY RULES" };

            scanButton.style.height = 45;
            scanButton.style.marginBottom = 20;
            scanButton.style.backgroundColor = new Color(0.25f, 0.7f, 0.3f);
            scanButton.style.color = Color.white;
            scanButton.style.fontSize = 14;
            scanButton.style.unityFontStyleAndWeight = FontStyle.Bold;
            scanButton.style.borderTopLeftRadius = 6; scanButton.style.borderTopRightRadius = 6;
            scanButton.style.borderBottomLeftRadius = 6; scanButton.style.borderBottomRightRadius = 6;

            _contentContainer.Add(scanButton);

            var settingsLabel = new Label("Configuration");
            settingsLabel.style.fontSize = 12;
            settingsLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            settingsLabel.style.marginBottom = 5;
            settingsLabel.style.opacity = 0.7f;
            _contentContainer.Add(settingsLabel);

            var settingsContainer = new VisualElement();

            Color borderColor = new Color(0, 0, 0, 0.3f);
            settingsContainer.style.borderTopColor = borderColor; settingsContainer.style.borderBottomColor = borderColor;
            settingsContainer.style.borderLeftColor = borderColor; settingsContainer.style.borderRightColor = borderColor;
            settingsContainer.style.borderTopWidth = 1; settingsContainer.style.borderBottomWidth = 1;
            settingsContainer.style.borderLeftWidth = 1; settingsContainer.style.borderRightWidth = 1;
            settingsContainer.style.borderTopLeftRadius = 10; settingsContainer.style.borderTopRightRadius = 10;
            settingsContainer.style.borderBottomLeftRadius = 10; settingsContainer.style.borderBottomRightRadius = 10;

            settingsContainer.style.paddingTop = 15; settingsContainer.style.paddingBottom = 15;
            settingsContainer.style.paddingLeft = 10; settingsContainer.style.paddingRight = 10;
            settingsContainer.style.backgroundColor = new Color(0, 0, 0, 0.15f);

            var inspector = new InspectorElement(_settings);
            settingsContainer.Add(inspector);

            var scrollView = new ScrollView();
            scrollView.Add(settingsContainer);
            _contentContainer.Add(scrollView);
        }

        private void ShowManualView()
        {
            _contentContainer.Clear();

            var titleLabel = new Label("Manual Override");
            titleLabel.style.fontSize = 20;
            titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            titleLabel.style.marginBottom = 5;
            titleLabel.style.alignSelf = Align.Center;
            _contentContainer.Add(titleLabel);

            var separator = new VisualElement();
            separator.style.height = 1;
            separator.style.backgroundColor = new Color(0.5f, 0.5f, 0.5f, 0.3f);
            separator.style.marginBottom = 15;
            separator.style.marginLeft = 5;
            separator.style.marginRight = 5;
            _contentContainer.Add(separator);

            var infoLabel = new Label("Drag scripts to manually override their icons.");
            infoLabel.style.fontSize = 12;
            infoLabel.style.opacity = 0.7f;
            infoLabel.style.marginBottom = 15;
            infoLabel.style.alignSelf = Align.Center;
            _contentContainer.Add(infoLabel);

            _contentContainer.Add(_manualView.CreateView());
        }

        #endregion

        #region IO Operations

        private void CreateSettingsFile()
        {
            var asset = ScriptableObject.CreateInstance<VibrantIconsSettings>();
            string path = "Assets/VibrantIconsSettings.asset";

            var scriptGuids = AssetDatabase.FindAssets("VibrantIconsWindow t:MonoScript");
            if (scriptGuids.Length > 0)
            {
                string scriptPath = AssetDatabase.GUIDToAssetPath(scriptGuids[0]);
                string editorDir = Path.GetDirectoryName(scriptPath);
                string packageRoot = Path.GetDirectoryName(editorDir);
                packageRoot = packageRoot.Replace("\\", "/");

                string targetDir = packageRoot + "/Scriptable Objects";

                if (!AssetDatabase.IsValidFolder(targetDir))
                {
                    Directory.CreateDirectory(targetDir);
                    AssetDatabase.Refresh();
                }
                path = targetDir + "/VibrantIconsSettings.asset";
            }
            path = AssetDatabase.GenerateUniqueAssetPath(path);

            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
            Debug.Log($"Vibrant Icons: Settings file created at: {path}");
        }

        #endregion
    }
}
#endif
