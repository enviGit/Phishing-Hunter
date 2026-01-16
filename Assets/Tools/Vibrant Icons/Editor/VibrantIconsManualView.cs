#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Envi.VibrantIcons
{
    /// <summary>
    /// Controls the "Manual Override" view in the main window, allowing users to
    /// assign icons to specific scripts via Drag & Drop or file selection.
    /// </summary>
    public class VibrantIconsManualView
    {
        #region Fields

        private List<MonoScript> _droppedScripts = new List<MonoScript>();
        private VisualElement _listContainer;
        private Label _countLabel;

        #endregion

        #region UI Creation

        /// <summary>
        /// Creates and returns the visual hierarchy for the manual assignment view.
        /// </summary>
        public VisualElement CreateView()
        {
            var root = new VisualElement();
            root.style.flexGrow = 1;
            root.style.paddingTop = 10;

            var dropZone = new VisualElement();
            dropZone.style.height = 90;
            dropZone.style.backgroundColor = new Color(0, 0, 0, 0.2f);

            dropZone.style.borderTopWidth = 2; dropZone.style.borderBottomWidth = 2;
            dropZone.style.borderLeftWidth = 2; dropZone.style.borderRightWidth = 2;
            dropZone.style.borderTopColor = new Color(1, 1, 1, 0.1f);
            dropZone.style.borderBottomColor = new Color(1, 1, 1, 0.1f);
            dropZone.style.borderTopLeftRadius = 10; dropZone.style.borderTopRightRadius = 10;
            dropZone.style.borderBottomLeftRadius = 10; dropZone.style.borderBottomRightRadius = 10;

            dropZone.style.justifyContent = Justify.Center;
            dropZone.style.alignItems = Align.Center;
            dropZone.style.marginBottom = 10;

            var dropLabel = new Label("Drag scripts here");
            dropLabel.style.opacity = 0.5f;
            dropLabel.style.fontSize = 12;
            dropLabel.pickingMode = PickingMode.Ignore;
            dropZone.Add(dropLabel);

            dropZone.RegisterCallback<DragUpdatedEvent>(OnDragUpdated);
            dropZone.RegisterCallback<DragPerformEvent>(OnDragPerform);

            root.Add(dropZone);

            var addButtonsRow = new VisualElement();
            addButtonsRow.style.flexDirection = FlexDirection.Row;
            addButtonsRow.style.marginBottom = 15;

            var explorerBtn = new Button(OnAddFromExplorerClicked) { text = "📂  From Disk..." };
            explorerBtn.style.flexGrow = 1;
            explorerBtn.style.height = 25;
            explorerBtn.style.marginRight = 5;
            explorerBtn.tooltip = "Opens system file picker (single file).";
            addButtonsRow.Add(explorerBtn);

            var selectionBtn = new Button(OnAddSelectedClicked) { text = "📋  Selected in Project" };
            selectionBtn.style.flexGrow = 1;
            selectionBtn.style.height = 25;
            selectionBtn.style.marginLeft = 5;
            selectionBtn.tooltip = "Adds all scripts currently selected in the Project view.";
            addButtonsRow.Add(selectionBtn);

            root.Add(addButtonsRow);

            var actionsRow = new VisualElement();
            actionsRow.style.flexDirection = FlexDirection.Row;
            actionsRow.style.marginBottom = 10;

            var selectIconBtn = new Button(OnSelectIconClicked) { text = "Select Icon & Apply" };
            selectIconBtn.style.flexGrow = 1;
            selectIconBtn.style.height = 30;
            selectIconBtn.style.backgroundColor = new Color(0.2f, 0.4f, 0.6f);
            selectIconBtn.style.color = Color.white;
            selectIconBtn.style.unityFontStyleAndWeight = FontStyle.Bold;

            var clearBtn = new Button(() => {
                _droppedScripts.Clear();
                RefreshList();
            }) { text = "Clear" };
            clearBtn.style.width = 60;

            actionsRow.Add(selectIconBtn);
            actionsRow.Add(clearBtn);
            root.Add(actionsRow);

            _countLabel = new Label("Selected files: 0");
            _countLabel.style.marginBottom = 5;
            root.Add(_countLabel);

            var scrollView = new ScrollView();
            scrollView.style.flexGrow = 1;
            scrollView.style.backgroundColor = new Color(0, 0, 0, 0.1f);
            _listContainer = new VisualElement();
            scrollView.Add(_listContainer);
            root.Add(scrollView);

            return root;
        }

        #endregion

        #region Event Handlers

        private void OnAddSelectedClicked()
        {
            var selectedObjects = Selection.objects;
            int addedCount = 0;

            foreach (var obj in selectedObjects)
            {
                if (obj is MonoScript script)
                {
                    if (!_droppedScripts.Contains(script))
                    {
                        _droppedScripts.Add(script);
                        addedCount++;
                    }
                }
            }

            if (addedCount > 0) RefreshList();
            else EditorUtility.DisplayDialog("Vibrant Icons", "No C# scripts selected in Project view.", "OK");
        }

        private void OnAddFromExplorerClicked()
        {
            string path = EditorUtility.OpenFilePanel("Select C# Script", Application.dataPath, "cs");

            if (!string.IsNullOrEmpty(path))
            {
                if (path.StartsWith(Application.dataPath))
                {
                    string relativePath = "Assets" + path.Substring(Application.dataPath.Length);
                    var script = AssetDatabase.LoadAssetAtPath<MonoScript>(relativePath);

                    if (script != null && !_droppedScripts.Contains(script))
                    {
                        _droppedScripts.Add(script);
                        RefreshList();
                    }
                }
                else EditorUtility.DisplayDialog("Error", "Please select a file inside the Assets folder.", "OK");
            }
        }

        private void OnDragUpdated(DragUpdatedEvent evt)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
        }

        private void OnDragPerform(DragPerformEvent evt)
        {
            DragAndDrop.AcceptDrag();
            foreach (var obj in DragAndDrop.objectReferences)
            {
                if (obj is MonoScript script && !_droppedScripts.Contains(script))
                {
                    _droppedScripts.Add(script);
                }
                else if (obj is DefaultAsset)
                {
                    string path = AssetDatabase.GetAssetPath(obj);
                    if (AssetDatabase.IsValidFolder(path))
                    {
                        var scriptsInFolder = AssetDatabase.FindAssets("t:MonoScript", new[] { path });
                        foreach (var guid in scriptsInFolder)
                        {
                            var s = AssetDatabase.LoadAssetAtPath<MonoScript>(AssetDatabase.GUIDToAssetPath(guid));
                            if(s != null && !_droppedScripts.Contains(s)) _droppedScripts.Add(s);
                        }
                    }
                }
            }
            RefreshList();
        }

        #endregion

        #region Logic

        private void RefreshList()
        {
            _listContainer.Clear();
            _countLabel.text = $"Selected files: {_droppedScripts.Count}";

            foreach (var script in _droppedScripts)
            {
                var row = new VisualElement();
                row.style.flexDirection = FlexDirection.Row;
                row.style.paddingLeft = 5; row.style.paddingRight = 5;
                row.style.paddingTop = 2; row.style.paddingBottom = 2;
                row.style.borderBottomWidth = 1;
                row.style.borderBottomColor = new Color(0.5f, 0.5f, 0.5f, 0.2f);
                row.style.alignItems = Align.Center;

                var icon = new Image();
                icon.image = EditorGUIUtility.IconContent("cs Script Icon").image;
                icon.style.width = 16; icon.style.height = 16;
                row.Add(icon);

                var label = new Label(script.name);
                label.style.marginLeft = 5;
                label.style.flexGrow = 1;
                row.Add(label);

                var removeBtn = new Button(() => {
                    _droppedScripts.Remove(script);
                    RefreshList();
                }) { text = "X" };
                row.Add(removeBtn);

                _listContainer.Add(row);
            }
        }

        private void OnSelectIconClicked()
        {
            if (_droppedScripts.Count == 0)
            {
                EditorUtility.DisplayDialog("Vibrant Icons", "Please select scripts first.", "OK");
                return;
            }

            IconPickerWindow.Show((selectedIcon) => {
                ApplyIconToDroppedScripts(selectedIcon);
            });
        }

        private void ApplyIconToDroppedScripts(Texture2D selectedIcon)
        {
            bool isDark = EditorGUIUtility.isProSkin;
            string targetTheme = isDark ? "Dark" : "Light";
            string iconName = selectedIcon.name;

            Texture2D finalIcon = selectedIcon;
            string[] guids = AssetDatabase.FindAssets($"{iconName} t:Texture2D");
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.Contains(targetTheme) && System.IO.Path.GetFileNameWithoutExtension(path) == iconName)
                {
                    finalIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                    break;
                }
            }

            int count = 0;
            foreach (var script in _droppedScripts)
            {
                string path = AssetDatabase.GetAssetPath(script);
                var importer = AssetImporter.GetAtPath(path) as MonoImporter;
                if (importer != null)
                {
                    importer.SetIcon(finalIcon);
                    EditorUtility.SetDirty(importer);
                    importer.SaveAndReimport();
                    count++;
                }
            }

            _droppedScripts.Clear();
            RefreshList();
            AssetDatabase.Refresh();
            Debug.Log($"Vibrant Icons: Manually changed icon for {count} scripts.");
        }

        #endregion
    }
}
#endif
