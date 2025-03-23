using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FpsCounter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI fpsValue;
    int frameCounter = 0;
    float timeCounter = 0.0f;
    float lastFramerate = 0.0f;
    public float refreshTime = 0.5f;
    int finalFps = 0;
    void Update()
    {
        if( timeCounter < refreshTime )
        {
            timeCounter += Time.deltaTime;
            frameCounter++;
        }
        else
        {
            lastFramerate = (float)frameCounter/timeCounter;
            frameCounter = 0;
            timeCounter = 0.0f;
        }
        finalFps = (int)lastFramerate;
        fpsValue.text = finalFps.ToString();
    }
}
