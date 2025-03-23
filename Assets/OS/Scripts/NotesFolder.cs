using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class NotesFolder : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool isSelected;
    public GameObject selected;
    public GameObject actualFolder;
    public Animator selectedAnim;
    private bool folderIsRunning = false;
    public GameObject folderInTaskbar;
    public GameObject textFile;
    public GameObject panel;
    public Transform newTextParent;
    public Transform newPanelParent;
    public Animator newTextAnim;
    public int textNumber;
    public int panelNumber;
    public bool inSelectionState = false;
    [SerializeField] TextMeshProUGUI error;
    void Start()
    {
    }
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        isSelected = true;
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        isSelected = false;
    }
    void Update()
    {
        panelNumber = GameObject.FindGameObjectsWithTag("NewTextPanels").Length + 1;
        textNumber = panelNumber;
        if(isSelected)
        {
            selected.SetActive(true);
            if(Input.GetKeyDown(KeyCode.Mouse0))
            {
                folderIsRunning = true;
                actualFolder.SetActive(true);
                selectedAnim.SetTrigger("Clicked");
            }
        }
        else
        {
            selected.SetActive(false);
        }
        if(folderIsRunning)
        {
            folderInTaskbar.SetActive(true);
        }
        else
        {
            folderInTaskbar.SetActive(false);
        }
        if(textNumber == 1)
        {
            inSelectionState = false;
        }
    }
    public void CreateTextFile()
    {
        if(!inSelectionState)
        {
            GameObject newText = Instantiate(textFile);
            newText.gameObject.tag = "NewTextFiles";
            newText.transform.SetParent(newTextParent);
            newText.transform.localScale = new Vector2(1,1);
            newText.gameObject.SetActive(true);
            newText.gameObject.transform.Find("Title").GetComponent<TextMeshProUGUI>().text = "Text - " + textNumber.ToString();
            newTextAnim = newText.gameObject.GetComponent<Animator>();
            GameObject newPanel = Instantiate(panel);
            newPanel.gameObject.tag = "NewTextPanels";
            newPanel.gameObject.transform.Find("Title").GetComponent<TextMeshProUGUI>().text = "Text - " + panelNumber.ToString();
            newPanel.transform.SetParent(newPanelParent);
            newPanel.GetComponent<RectTransform>().localScale = panel.GetComponent<RectTransform>().localScale;
            newPanel.GetComponent<RectTransform>().sizeDelta = panel.GetComponent<RectTransform>().sizeDelta;
            newPanel.gameObject.SetActive(true);
            newText.GetComponent<Button>().onClick.AddListener(() => newPanel.transform.position = actualFolder.transform.position);
        }
        else
        {
            error.text = "Please quit the selection state.";
        }
    }
    public void SelectState()
    {
        GameObject[] newTexts = GameObject.FindGameObjectsWithTag("NewTextFiles");
        if(textNumber > 1)
        {
            error.text = "";
            foreach(GameObject newText in newTexts)
            {
                if(newText.GetComponent<Animator>().GetBool("inSelectState") == true)
                {
                    inSelectionState = false;
                    newText.GetComponent<Animator>().SetBool("inSelectState", false);
                }
                else
                {
                    inSelectionState = true;
                    newText.GetComponent<Animator>().SetBool("inSelectState", true);
                }
            }
        }
        else if(textNumber == 1)
        {
            error.text = "Please Create a text file.";
        }
    }
    public void SelectAll()
    {
        GameObject[] newTexts = GameObject.FindGameObjectsWithTag("NewTextFiles");
        foreach(GameObject newText in newTexts)
        {
            if(newText.GetComponent<Animator>().GetBool("inSelectState") == true)
            {
                newText.transform.Find("Toggle").GetComponent<Toggle>().isOn = true;
                error.text = "";
            }
            else
            {
                error.text = "Please enter the selection state.";
            }
        }
        if(textNumber == 1)
        {
            error.text = "Please Create a text file.";
        }
    }
    public void DeleteImage()
    {
        GameObject[] newTexts = GameObject.FindGameObjectsWithTag("NewTextFiles");
        GameObject[] newPanels = GameObject.FindGameObjectsWithTag("NewTextPanels");
        if(inSelectionState == true)
        {
            error.text = "";
            foreach(GameObject newText in newTexts)
            {
                foreach(GameObject newPanel in newPanels)
                {
                    if(newText.transform.Find("Toggle").GetComponent<Toggle>().isOn)
                    {
                        if(newPanel.gameObject.transform.Find("Title").GetComponent<TextMeshProUGUI>().text == newText.transform.Find("Title").GetComponent<TextMeshProUGUI>().text)
                        {
                            Destroy(newPanel);
                            Destroy(newText);
                        }
                    }
                }
            }
        }
        if(inSelectionState == false && textNumber > 1)
        {
            error.text = "Please Select a text file.";
        }
        if(inSelectionState == false && textNumber == 1)
        {
            error.text = "Please Create a text file.";
        }
    }
    public void Exit()
    {
        folderIsRunning = false;
    }
}