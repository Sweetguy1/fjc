using System;
using System.Collections;
using System.Collections.Generic;
using ManyBlockScene.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    private float seconds;
    private Text _text;
    public MainScene mainScene;

    private void Awake()
    {
        seconds = 300;
        _text = GetComponent<Text>();
        mainScene = GameObject.Find("Main Canvas").GetComponent<MainScene>();
    }
    
    private void CountDown()
    {
        seconds -= Time.deltaTime;
        int minutes = (int)(seconds /60f);
        int tmpSeconds = (int)seconds % 60;
        _text.text = $"Time:{minutes}:{tmpSeconds}";
    }

    private void Update()
    {
        if (mainScene._kStatus != GameStatus.Play) return;
        if (seconds > 0)
        {
            CountDown();
        }
        else
        {
            _text.text = "Time Out";
            EventManager.GetInstance().Dispatcher(EventType.GameFail,new Message("failed"));
        }
    }
}
