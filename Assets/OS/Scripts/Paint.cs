using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Paint : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler
{
    private bool isSelected;
    public GameObject selected;
    public GameObject actualPaint;
    public Animator selectedAnim;
    private bool paintIsRunning = false;
    public GameObject paintInTaskbar;
    public Animator paintAnim;

    void Update()
    {
        if(isSelected)
        {
            selected.SetActive(true);
            if(Input.GetKeyDown(KeyCode.Mouse0))
            {
                paintIsRunning = true;
                actualPaint.SetActive(true);
                selectedAnim.SetTrigger("Clicked");
            }
        }
        else
        {
            selected.SetActive(false);
        }
        if(paintIsRunning)
        {
            paintInTaskbar.SetActive(true);
            if(paintAnim.GetBool("exit") == true)
            {
                paintIsRunning = false;
            }
        }
        else
        {
            paintInTaskbar.SetActive(false);
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
}
