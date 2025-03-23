using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskBar : MonoBehaviour
{
    public Transform iconsParent;
    public int k;
    void Start()
    {
        
    }

    void Update()
    {
        k = GetComponentsInChildren<Button>().GetLength(0) - 1;
        GameObject[] icons = GameObject.FindGameObjectsWithTag("Icons");
        foreach(GameObject icon in icons)
        {
            icon.transform.SetSiblingIndex(k +2);
        }
    }
}
