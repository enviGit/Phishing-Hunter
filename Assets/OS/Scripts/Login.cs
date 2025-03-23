using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Login : MonoBehaviour
{
    public static Login instance;
    public GameObject[] dontDestroy;
    public TextMeshProUGUI user1;
    public TextMeshProUGUI description;
    public int descChars = 0;
    public TextMeshProUGUI descriptionCharsCounter;
    public GameObject loginAnimation;
    [SerializeField] TextMeshProUGUI user2;
    void Update()
    {
        descChars = description.textInfo.characterCount;
        descriptionCharsCounter.text = (descChars - 1).ToString() + "/70";
        if(descChars < 51)
        {
            descriptionCharsCounter.color = new Color32(255, 255, 255, 255);
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
    
    private void Awake()
    {
        instance = this;
        foreach(GameObject dontDestroyOnLoad in dontDestroy)
        {
            DontDestroyOnLoad(dontDestroyOnLoad);
        }
    }
    public void LogIn()
    {
        loginAnimation.SetActive(true);
        user2.text = user1.text;
    }
    public void OpenDesktop()
    {
        SceneManager.LoadScene(1);
    }
}
