using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppExit : MonoBehaviour
{
    public GameObject app;
    public void ExitApp()
    {
        app.SetActive(false);
    }
}
