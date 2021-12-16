using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using ManyBlockScene.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class UpdateScoreControl : MonoBehaviour
{
    public Text scoreText;
    public GameObject prop;
    public Text combo;
    public Slider comboSlider;
    public GameObject fillArea;
    public MainScene mainScene;
    

    public void CreateProp(Transform content,Vector3 position,int comboCount,AudioSource audio,AudioClip clip)
    {
        for (int i = 0; i < 3+comboCount; i++)
        {
            var tmpProp = GameObject.Instantiate(prop, content,true);
            tmpProp.transform.localScale *= 0.9f;
            tmpProp.transform.position = position;
            tmpProp.SetActive(true);
            tmpProp.transform.DOMove(transform.position, 0.5f).SetEase(Ease.OutCubic).SetDelay(0.1f*i).OnComplete((() =>
            {
                Combo();
                ComboText(comboCount);
                SetScore(1);
                Destroy(tmpProp);
                
                audio.clip = clip;
                audio.Play();
            }));
            
        }
    }

    private void Update()
    {
        if (mainScene._kStatus != GameStatus.Play) return;
        if (comboSlider.value > 0)
        {
            comboSlider.value -= Time.deltaTime * 0.1f;
        }
        else
        {
            if (fillArea.activeSelf == true)
            {
                mainScene.comboCount = 0;
                combo.text = "Combo X 0";
                fillArea.SetActive(false);
            }
        }
    }

    private void ComboText(int comboCount)
    {
        combo.text = "Combo X " + comboCount;
    }

    private void Combo()
    {
        comboSlider.value = 1;
        fillArea.SetActive(true);
    }

    private void SetScore(int score)
    {
        int tmpScore =  Int32.Parse(scoreText.text);
        tmpScore += score;
        scoreText.text = tmpScore.ToString();
    }
}
