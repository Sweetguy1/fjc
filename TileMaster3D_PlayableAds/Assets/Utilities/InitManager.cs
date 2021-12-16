using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitManager : MonoBehaviour {
    public Transform tfMainRoot, tfTopRoot, tfSuperRoot;
    public Transform tfMainRoot3D, tfTopRoot3D;

    public string sLoadScene = "";

    private void Awake() {
#if UNITY_EDITOR
        Debug.unityLogger.logEnabled = true;
        DG.Tweening.DOTween.Init(false, false, DG.Tweening.LogBehaviour.Default);
#else 
        Debug.unityLogger.logEnabled = false;
        Application.targetFrameRate = 60;
        Input.multiTouchEnabled = true;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
#endif

        DG.Tweening.DOTween.SetTweensCapacity(500, 50);
        SceneMgr.tfMainRoot = this.tfMainRoot;
        SceneMgr.tfTopRoot = this.tfTopRoot;
        SceneMgr.tfSuperRoot = this.tfSuperRoot;

        SceneMgr.tfMainRoot3D = this.tfMainRoot3D;
        SceneMgr.tfTopRoot3D = this.tfTopRoot3D;
    }
    private void Start() {
        SceneMgr.LoadScene(this.sLoadScene);
    }
}