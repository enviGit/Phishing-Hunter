using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextFile : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Slider sliderA;
    [SerializeField] Slider sliderR;
    [SerializeField] Slider sliderG;
    [SerializeField] Slider sliderB;
    public GameObject fontsDropdownObject;
    public TMP_FontAsset fontA;
    public TMP_FontAsset fontB;
    public TMP_FontAsset fontC;
    public TMP_FontAsset fontD;
    public TMP_FontAsset fontE;
    public TMP_FontAsset fontF;
    public TMPro.TMP_Dropdown fontDropDown;
    [SerializeField] TextMeshProUGUI dropDownText;
    void Start()
    {
        
    }

    void Update()
    {
        if(fontsDropdownObject.transform.childCount > 3)
        {
            fontsDropdownObject.transform.Find("Dropdown List").transform.Find("Viewport").transform.Find("Content").transform.Find("Item 0: Font A").transform.Find("Item Label").GetComponent<TextMeshProUGUI>().font = fontA;
            fontsDropdownObject.transform.Find("Dropdown List").transform.Find("Viewport").transform.Find("Content").transform.Find("Item 1: Font B").transform.Find("Item Label").GetComponent<TextMeshProUGUI>().font = fontB;
            fontsDropdownObject.transform.Find("Dropdown List").transform.Find("Viewport").transform.Find("Content").transform.Find("Item 2: Font C").transform.Find("Item Label").GetComponent<TextMeshProUGUI>().font = fontC;
            fontsDropdownObject.transform.Find("Dropdown List").transform.Find("Viewport").transform.Find("Content").transform.Find("Item 3: Font D").transform.Find("Item Label").GetComponent<TextMeshProUGUI>().font = fontD;
            fontsDropdownObject.transform.Find("Dropdown List").transform.Find("Viewport").transform.Find("Content").transform.Find("Item 4: Font E").transform.Find("Item Label").GetComponent<TextMeshProUGUI>().font = fontE;
            fontsDropdownObject.transform.Find("Dropdown List").transform.Find("Viewport").transform.Find("Content").transform.Find("Item 5: Font F").transform.Find("Item Label").GetComponent<TextMeshProUGUI>().font = fontF;
        }
        text.color = new Color32((byte)sliderR.value, (byte)sliderG.value, (byte)sliderB.value, (byte)sliderA.value);
    }
    public void ChangeFonts()
    {
        if(fontDropDown.value == 0)
        {
            text.font = fontA;
            dropDownText.font = fontA;
        }
        if(fontDropDown.value == 1)
        {
            text.font = fontB;
            dropDownText.font = fontB;
        }
        if(fontDropDown.value == 2)
        {
            text.font = fontC;
            dropDownText.font = fontC;
        }
        if(fontDropDown.value == 3)
        {
            text.font = fontD;
            dropDownText.font = fontD;
        }
        if(fontDropDown.value == 4)
        {
            text.font = fontE;
            dropDownText.font = fontE;
        }
        if(fontDropDown.value == 5)
        {
            text.font = fontF;
            dropDownText.font = fontF;
        }
    }
}
