using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System;

public class MusicApp : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler
{
    private bool isSelected;
    public GameObject selected;
    public GameObject actualMusicApp;
    public Animator selectedAnim;
    public Animator musicAppAnim;
    public bool musicAppIsRunning;
    public GameObject musicAppInTaskbar;
    public TMPro.TMP_Dropdown songsDropdown;
    [SerializeField] Slider songSlider;
    [SerializeField] TextMeshProUGUI songTime;
    [SerializeField] TextMeshProUGUI songTitle;
    public string playingSongName;
    public GameObject pauseIcon;
    public GameObject songCover;
    public GameObject[] songsGameObjects;
    public Texture blankTexture;
    void Start()
    {
        List<string> songsNames = new List<string>();
        foreach(GameObject songGameObject in songsGameObjects)
        {
            AudioSource song = songGameObject.GetComponent<AudioSource>();
            songsNames.Add(song.clip.name);
            songsDropdown.AddOptions(songsNames);
        }
    }
    void Update()
    {
        if(isSelected)
        {
            selected.SetActive(true);
            if(Input.GetKeyDown(KeyCode.Mouse0))
            {
                musicAppIsRunning = true;
                actualMusicApp.SetActive(true);
                selectedAnim.SetTrigger("Clicked");
            }
        }
        else
        {
            selected.SetActive(false);
        }
        foreach(GameObject songGameObject in songsGameObjects)
        {
            AudioSource song = songGameObject.GetComponent<AudioSource>();
            if(song.clip.name == playingSongName)
            {
                songSlider.maxValue = song.clip.length;
                songSlider.value = song.time;
                songTime.text = TimeSpan.FromSeconds(song.time).Minutes + ":" + TimeSpan.FromSeconds(song.time).Seconds + "/" + TimeSpan.FromSeconds(song.clip.length).Minutes + ":" + TimeSpan.FromSeconds(song.clip.length).Seconds; 
            }
            else
            {
                songCover.GetComponent<RawImage>().texture = blankTexture;
                songSlider.maxValue = 1;
                songSlider.value = 0;
                songTime.text = "00:00";
            }
        }
        if(musicAppIsRunning)
        {
            musicAppInTaskbar.SetActive(true);
        }
        else
        {
            musicAppInTaskbar.SetActive(false);
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
    
    public void PlaySong()
    {
        playingSongName = songsDropdown.options[songsDropdown.value].text;
        foreach(GameObject songGameObject in songsGameObjects)
        {
            AudioSource song = songGameObject.GetComponent<AudioSource>();
            RawImage songCoverTexture = songGameObject.GetComponent<RawImage>();
            if(song.clip.name == playingSongName)
            {
                pauseIcon.SetActive(true);
                songTitle.text = playingSongName;
                songCover.GetComponent<RawImage>().texture = songCoverTexture.texture;
                song.Play(0);
            }
            else
            {
                song.Stop();
                pauseIcon.SetActive(false);
                songTitle.text = "NO SONG";
            }
        }
    }

    public void PlayPause()
    {
        if(pauseIcon.activeSelf)
        {
            pauseIcon.SetActive(false);
            foreach(GameObject songGameObject in songsGameObjects)
            {
                AudioSource song = songGameObject.GetComponent<AudioSource>();
                if(song.clip.name == playingSongName)
                {
                    song.Pause();
                }
            }
        }
        else
        {
            pauseIcon.SetActive(true);
            foreach(GameObject songGameObject in songsGameObjects)
            {
                AudioSource song = songGameObject.GetComponent<AudioSource>();
                if(song.clip.name == playingSongName)
                {
                    song.Play();
                }
            }           
        }
    }
    
    public void Repeat()
    {
        foreach(GameObject songGameObject in songsGameObjects)
        {
            AudioSource song = songGameObject.GetComponent<AudioSource>();
            if(song.clip.name == playingSongName)
            {
                song.Play(0);
            }
        }
    }

    public void Exit()
    {
        musicAppIsRunning = false;
    }
}
