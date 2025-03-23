using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Start : MonoBehaviour
{
    public Animator anim;
    public GameObject shutDownCanvas;
    public void StartMenu()
    {
        if(anim.GetBool("isRunning") == true)
        {
            anim.SetBool("isRunning", false);
        }
        else
        {
            anim.SetBool("isRunning", true);
        }
    }
    public void ShutDown()
    {
        shutDownCanvas.SetActive(true);
        // Add your custom shutdown logic here.
    }

    // Add code related to the start menu buttons or any other relevant functionality.
}