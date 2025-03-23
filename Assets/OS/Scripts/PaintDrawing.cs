using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PaintDrawing : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler
{
    private bool isSelected;
    public Camera m_camera;
    public GameObject brush;
    public GameObject panel;

    LineRenderer currentLineRenderer;

    Vector2 lastPos;

    private void Update()
    {
        if(isSelected)
        {
            Drawing();
        }
        GameObject[] clones = GameObject.FindGameObjectsWithTag("BrushClones");
        foreach(GameObject clone in clones)
        {
            clone.transform.position = panel.transform.position;
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

    void Drawing() 
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            CreateBrush();
        }
        else if (Input.GetKey(KeyCode.Mouse0))
        {
            PointToMousePos();
        }
        else 
        {
            currentLineRenderer = null;
        }
    }

    void CreateBrush() 
    {
        GameObject brushInstance = Instantiate(brush);
        brushInstance.gameObject.tag = "BrushClones";
        currentLineRenderer = brushInstance.GetComponent<LineRenderer>();
        Vector2 mousePos = m_camera.ScreenToWorldPoint(Input.mousePosition);

        currentLineRenderer.SetPosition(0, mousePos);
        currentLineRenderer.SetPosition(1, mousePos);

    }

    void AddAPoint(Vector2 pointPos)
    {
        currentLineRenderer.positionCount++;
        int positionIndex = currentLineRenderer.positionCount - 1;
        currentLineRenderer.SetPosition(positionIndex, pointPos);
    }

    void PointToMousePos() 
    {
        Vector2 mousePos = m_camera.ScreenToWorldPoint(Input.mousePosition);
        if (lastPos != mousePos) 
        {
            AddAPoint(mousePos);
            lastPos = mousePos;
        }
    }

    public void EraseBack()
    {
        Destroy(GameObject.FindWithTag("BrushClones"));
    }
    public void EraseAll()
    {
        GameObject[] clones = GameObject.FindGameObjectsWithTag("BrushClones");
        foreach(GameObject clone in clones)
        {
            GameObject.Destroy(clone);
        }
    }
}
