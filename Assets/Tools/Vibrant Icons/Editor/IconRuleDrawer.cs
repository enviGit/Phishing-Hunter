#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Envi.VibrantIcons
{
    /// <summary>
    /// Custom PropertyDrawer for IconRule to display it cleanly in the inspector using UI Toolkit.
    /// Handles the custom layout for priority, keyword, exact match toggle, and icon preview button.
    /// </summary>
    [CustomPropertyDrawer(typeof(IconRule))]
    public class IconRuleDrawer : PropertyDrawer
    {
        #region GUI Creation

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();
            root.style.flexDirection = FlexDirection.Row;
            root.style.marginBottom = 5;
            root.style.backgroundColor = new Color(0, 0, 0, 0.1f);

            root.style.paddingTop = 5; root.style.paddingBottom = 5;
            root.style.paddingLeft = 5; root.style.paddingRight = 5;

            float r = 5f;
            root.style.borderTopLeftRadius = r; root.style.borderTopRightRadius = r;
            root.style.borderBottomLeftRadius = r; root.style.borderBottomRightRadius = r;

            root.style.alignItems = Align.Center;

            var priorityProp = property.FindPropertyRelative("Priority");
            var priorityField = new IntegerField();
            priorityField.BindProperty(priorityProp);
            priorityField.style.width = 30;
            priorityField.style.marginRight = 5;
            priorityField.tooltip = "Rule priority.\nHigher number = Higher priority.";
            priorityField.style.unityTextAlign = TextAnchor.MiddleCenter;
            root.Add(priorityField);

            var keywordProp = property.FindPropertyRelative("Keyword");
            var keywordField = new TextField();
            keywordField.BindProperty(keywordProp);
            keywordField.style.flexGrow = 1;
            keywordField.style.marginRight = 10;
            keywordField.tooltip = "Keyword to search for in script names (e.g., 'Manager').";
            keywordField.isDelayed = true;
            root.Add(keywordField);

            var toggleContainer = new VisualElement();
            toggleContainer.style.flexDirection = FlexDirection.Row;
            toggleContainer.style.alignItems = Align.Center;
            toggleContainer.style.marginRight = 10;

            var exactProp = property.FindPropertyRelative("ExactMatch");
            var exactToggle = new Toggle();
            exactToggle.BindProperty(exactProp);
            exactToggle.label = "";
            exactToggle.style.width = 20;
            exactToggle.style.marginRight = 0;

            var exactLabel = new Label("Exact");
            exactLabel.style.fontSize = 10;
            exactLabel.tooltip = "If checked, the file name must match the keyword exactly.";
            toggleContainer.Add(exactToggle);
            toggleContainer.Add(exactLabel);
            root.Add(toggleContainer);

            var iconProp = property.FindPropertyRelative("Icon");
            var iconBtn = new Button();
            iconBtn.style.width = 40;
            iconBtn.style.height = 40;
            iconBtn.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);

            void UpdateButtonImage(SerializedProperty prop)
            {
                var tex = prop.objectReferenceValue as Texture2D;
                if (tex != null)
                {
                    iconBtn.style.backgroundImage = new StyleBackground(tex);
                    iconBtn.text = "";
                }
                else
                {
                    iconBtn.style.backgroundImage = null;
                    iconBtn.text = "?";
                }
                iconBtn.MarkDirtyRepaint();
            }

            UpdateButtonImage(iconProp);

            root.TrackPropertyValue(iconProp, UpdateButtonImage);

            iconBtn.clicked += () =>
            {
                IconPickerWindow.Show((selectedTexture) =>
                {
                    iconProp.objectReferenceValue = selectedTexture;
                    iconProp.serializedObject.ApplyModifiedProperties();
                });
            };

            root.Add(iconBtn);

            return root;
        }

        #endregion
    }
}
#endif
