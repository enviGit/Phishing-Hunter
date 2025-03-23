using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class SettingsApp : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler
{
    private bool isSelected;
    public GameObject selected;
    public GameObject actualSettings;
    public Animator selectedAnim;
    public Animator settingsAnim;
    public bool settingsisRunning;
    public GameObject settingsInTaskbar;
    public Texture2D[] cursorTextures = new Texture2D[10];
    [SerializeField] TextMeshProUGUI oldUsername;
    [SerializeField] TextMeshProUGUI oldDescription;
    [SerializeField] TextMeshProUGUI newUsername;
    [SerializeField] TextMeshProUGUI newDescription;
    public int descChars = 0;
    public TextMeshProUGUI descriptionCharsCounter;
    public VerticalLayoutGroup iconsLayoutGroup;
    public Slider iconsSpacingSlider;
    public Toggle showIconsToggle;
    public GameObject[] icons;

    void Start()
    {
        selected.SetActive(false);
    }

    void Update()
    {
        if(isSelected)
        {
            selected.SetActive(true);
            if(Input.GetKeyDown(KeyCode.Mouse0))
            {
                settingsisRunning = true;
                actualSettings.SetActive(true);
                selectedAnim.SetTrigger("Clicked");
            }
        }
        else
        {
            selected.SetActive(false);
        }
        if(settingsisRunning)
        {
            settingsInTaskbar.SetActive(true);
        }
        else
        {
            settingsInTaskbar.SetActive(false);
        }
    }
    void FixedUpdate()
    {
        iconsLayoutGroup.spacing = iconsSpacingSlider.value;
        descChars = newDescription.textInfo.characterCount;
        descriptionCharsCounter.text = (descChars - 1).ToString() + "/70";
        if(descChars < 51)
        {
            descriptionCharsCounter.color = new Color32(0, 0, 0, 255);
        }
        if(descChars >= 51 && descChars <= 61)
        {
            descriptionCharsCounter.color = new Color32(255, 251, 0, 255);
        }
        if (descChars > 61)
        {
            descriptionCharsCounter.color = new Color32(255, 3, 0, 255);
        }
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        isSelected = true;
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        isSelected = false;
    }

    public void ChangeCursorColor(int id)
    {
        foreach(Texture2D cursor in cursorTextures)
        {
            if(cursor.name == id.ToString())
            {
                Cursor.SetCursor(cursor, Vector2.zero, CursorMode.Auto);
            }
        }
    }

    public void ChangeUsername()
    {
        oldUsername.text = newUsername.text;
    }
    public void ChangeDescription()
    {
        oldDescription.text = newDescription.text;
    }
    public void ToggleShowIcons()
    {
        if(showIconsToggle.isOn == true)
        {
            foreach(GameObject icon in icons)
            {
                icon.SetActive(true);
            }
        }
        else
        {
            foreach(GameObject icon in icons)
            {
                icon.SetActive(false);
            }
        }
    }
    public void Exit()
    {
        settingsisRunning = false;
    }
}