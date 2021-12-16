using System;
using System.Collections;
using System.Collections.Generic;
using ManyBlockScene.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject scenePrefab;
    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject quitButton;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private GameObject timePanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Image transparentPanel;
    [SerializeField] private GameObject overPanel;
    [SerializeField] private GameObject title;
    [SerializeField] private GameObject restart;
    [SerializeField] private AudioManager audioManager;

    private MainScene _mainScene;
    private GameObject _scene;

    private void Awake()
    {
        timePanel.SetActive(false);
        pausePanel.SetActive(false);
        transparentPanel.gameObject.SetActive(false);
        overPanel.SetActive(false);
        restart.SetActive(false);
        
        title.SetActive(true);
    }

    private void Start()
    {
        EventManager.GetInstance().AddListener(EventType.GameFail,GameOver);
        EventManager.GetInstance().AddListener(EventType.GameWin,GameOver);
    }

    public void GameStart()
    {
        _scene = Instantiate(scenePrefab, transform, false);
        _scene.name = "PlayableAds";
        _scene.transform.SetSiblingIndex(1);
        _mainScene = _scene.GetComponentInChildren<MainScene>();
        _mainScene.audioManager = audioManager;
        startButton.SetActive(false);
        quitButton.SetActive(false);
        pausePanel.SetActive(false);
        restart.SetActive(false);
        title.SetActive(false);
        transparentPanel.gameObject.SetActive(false);
        overPanel.SetActive(false);
        timePanel.SetActive(true);
        var timer = timePanel.transform.Find("Timer").gameObject.AddComponent<Timer>();
        timer.mainScene = _mainScene;
        Handheld.Vibrate();
    }

    public void Quit()
    {
        Application.Quit();
    }
    
    public void AudioCrl()
    {
        foreach (var tmpAudio in audioManager.audioSources)
        {
            tmpAudio.volume = tmpAudio.volume == 0 ? 1 : 0;
        }
    
        audioSource.volume = audioSource.volume == 0 ? 1 : 0;
    }

    public void Pause()
    {
        _mainScene._kStatus = GameStatus.Waiting;
        transparentPanel.gameObject.SetActive(true);
        pausePanel.SetActive(true);
    }

    public void Unpause()
    {
        _mainScene._kStatus = GameStatus.Play;
        transparentPanel.gameObject.SetActive(false);
        pausePanel.SetActive(false);
    }

    public void BackToMenu()
    {
        _mainScene._kStatus = GameStatus.Waiting;
        GameObject.Find("PlayableAds").SetActive(false);
        timePanel.SetActive(false);
        startButton.SetActive(true);
        quitButton.SetActive(true);
        transparentPanel.gameObject.SetActive(false);
        pausePanel.SetActive(false);
        restart.SetActive(true);
        title.SetActive(true);
    }

    public void Continue()
    {
        _mainScene._kStatus = GameStatus.Play;
        _scene.SetActive(true);
        timePanel.SetActive(true);
        startButton.SetActive(false);
        quitButton.SetActive(false);
        transparentPanel.gameObject.SetActive(false);
        pausePanel.SetActive(false);
        restart.SetActive(false);
        title.SetActive(false);
    }

    public void Exit()
    {
        Destroy(_scene);
        _scene = null;
        overPanel.SetActive(false);
        startButton.SetActive(true);
        title.SetActive(true);
        timePanel.SetActive(false);
        quitButton.SetActive(true);
        transparentPanel.gameObject.SetActive(false);
    }

    private void GameOver(EventType type, Message data)
    {
        _mainScene._kStatus = type == EventType.GameFail ? GameStatus.Lose : GameStatus.Victory;
        Text text = overPanel.transform.Find("OverText").GetComponent<Text>();
        text.text = data.message;
        overPanel.SetActive(true);
        transparentPanel.gameObject.SetActive(true);
    }

    public void Restart()
    {
        Destroy(_scene);
        _scene = null;
        GameStart();
    }
}
