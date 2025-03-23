using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class CMD : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler
{
    private bool isSelected;
    public GameObject selected;
    public GameObject actualCMD;
    public Animator selectedAnim;
    public Animator cmdAnim;
    public TMP_InputField cmdInput;
    [SerializeField] TextMeshProUGUI cmdFinalInput;
    string[] validCommands = new string[]{"help", "exit", "showfps", "hidefps"};
    public bool cmdisRunning;
    public GameObject cmdInTaskbar;
    public GameObject fpsCounter;
    public Animator fpsAnim;
    public GameObject fpsCounterInTaskbar;
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
                cmdisRunning = true;
                actualCMD.SetActive(true);
                selectedAnim.SetTrigger("Clicked");
                cmdFinalInput.text = string.Empty;
            }
        }
        else
        {
            selected.SetActive(false);
        }
        if(cmdisRunning)
        {
            cmdInTaskbar.SetActive(true);
        }
        else
        {
            cmdInTaskbar.SetActive(false);
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

    public void CMDInput()
    {
        if(cmdInput.text != "")
        {
            if(!validCommands.Any(cmdInput.text.Contains))
            {
                if(cmdFinalInput.text == string.Empty)
                {
                    cmdFinalInput.text = ">Invalid command.";
                    cmdInput.text = "";
                }
                else
                {
                    cmdFinalInput.text = cmdFinalInput.text + "\n" + ">Invalid command.";
                    cmdInput.text = "";             
                }
            }
        }
        if(cmdInput.text.Equals(validCommands[0]))
        {
            if(cmdFinalInput.text == string.Empty)
            {
                cmdFinalInput.text = ">Current Commands : \n help, shutdown, exit, showfps, hidefps";
                cmdInput.text = "";
            }
            else
            {
                cmdFinalInput.text = cmdFinalInput.text + "\n" + ">Current Commands : \n help, shutdown, exit, showfps, hidefps";
                cmdInput.text = "";
            }
        }
        if(cmdInput.text.Equals(validCommands[1]))
        {
            if(cmdFinalInput.text == string.Empty)
            {
                cmdFinalInput.text = ">...";
                cmdInput.text = "";
                cmdAnim.SetTrigger("exit");
                cmdisRunning = false;
            }
            else
            {
                cmdFinalInput.text = cmdFinalInput.text + "\n" + ">...";
                cmdInput.text = "";
                cmdAnim.SetTrigger("exit");
                cmdisRunning = false;
            }
        }
        if(cmdInput.text.Equals(validCommands[2]))
        {
            if(cmdFinalInput.text == string.Empty)
            {
                cmdFinalInput.text = ">fpsCounter | On";
                cmdInput.text = "";
                fpsCounter.SetActive(true);
                fpsCounterInTaskbar.SetActive(true);
            }
            else
            {
                cmdFinalInput.text = cmdFinalInput.text + "\n" + "fpsCounter | On";
                cmdInput.text = "";
                fpsCounter.SetActive(true);
                fpsCounterInTaskbar.SetActive(true);
            }
        }
        if(cmdInput.text.Equals(validCommands[3]))
        {
            if(cmdFinalInput.text == string.Empty)
            {
                cmdFinalInput.text = ">fpsCounter | Off";
                cmdInput.text = "";
                fpsAnim.SetTrigger("exit");
                fpsCounterInTaskbar.SetActive(false);
            }
            else
            {
                cmdFinalInput.text = cmdFinalInput.text + "\n" + "fpsCounter | Off";
                cmdInput.text = "";
                fpsAnim.SetTrigger("exit");
                fpsCounterInTaskbar.SetActive(false);
            }
        }
    }
}
