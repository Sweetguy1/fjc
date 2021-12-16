using System.Security.Cryptography.X509Certificates;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DG.Tweening;
using UnityEngine;
// using UnityEngine.AddressableAssets;
// using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public static class SceneMgr {
    public static Transform tfMainRoot, tfTopRoot, tfSuperRoot;
    public static Transform tfMainRoot3D, tfTopRoot3D;
    public static string sCurSceneName, sEnterIn; //进入的场景
    public static List<GameObject> showingDialogs = new List<GameObject>();

    private static GameObject objCurScene = null, objCurScene3D = null;

    private static object data = null;
    private static System.Action callback_complete;
    public static void SetData(object _data) { data = _data; }
    public static object GetData() { object _data = data; data = null; return _data; }

    public static GameObject LoadScene(string s_file, string s_alias = null, System.Action callback = null) {
        if (objCurScene3D != null) GameObject.Destroy(objCurScene3D);
        return _LoadScene2D(s_file, s_alias, callback);
    }
    private static GameObject _LoadScene2D(string s_file, string s_alias = null, System.Action callback = null) {
        showingDialogs.Clear();
        sCurSceneName = s_file;
        if (string.IsNullOrEmpty(s_alias)) sEnterIn = sCurSceneName;
        else sEnterIn = s_alias;
        callback_complete = callback;

        var objNewScene = LoadMgr.LoadRes<GameObject>($"_ScenePrefabs/{s_file}");
        objNewScene.transform.SetParent(tfMainRoot, false);
        objNewScene.transform.SetSiblingIndex(0);

        SwitchEff_Fade(objNewScene);
        return objNewScene;
    }

    public static void LoadScene3D(string s_file, string s_alias = null, System.Action callback = null) {
        _LoadScene2D(s_file, s_alias, callback);
        if (objCurScene3D != null) GameObject.Destroy(objCurScene3D);
        objCurScene3D = LoadMgr.LoadRes<GameObject>($"_ScenePrefabs/{s_file}3D");
        objCurScene3D.transform.SetParent(tfMainRoot3D, false);
    }

    //渐变出现效果
    private static void SwitchEff_Fade(GameObject objNewScene) {
        CanvasGroup groupCurScene = null;
        if (objCurScene != null) {
            groupCurScene = objCurScene.transform.GetComponent<CanvasGroup>();
            groupCurScene.interactable = false;
            groupCurScene.blocksRaycasts = false;
        }

        var groupNewScene = objNewScene.GetComponent<CanvasGroup>();
        if (groupNewScene == null) groupNewScene = objNewScene.AddComponent<CanvasGroup>();
        groupNewScene.interactable = false;

        var sequence = DOTween.Sequence();
        if (groupCurScene != null) {
            // groupCurScene.transform.SpineFade(0, 0.2f);
            sequence.Append(groupCurScene.DOFade(0, 0.2f));
        }
        // groupNewScene.alpha = 0;
        // sequence.Append (groupNewScene.DOFade (1, 0.2f));
        sequence.OnComplete(() => {
            if (objCurScene != null) GameObject.Destroy(objCurScene);
            objCurScene = objNewScene;
            groupNewScene.interactable = true;
            if (groupCurScene != null) groupCurScene.blocksRaycasts = true;
            callback_complete?.Invoke();
            callback_complete = null;
        });
    }
    public static T AddNode<T>(string s_file, Transform parent, bool worldPositionStays = false) where T : Component {
        var node = LoadMgr.LoadRes<T>(s_file);
        if (!parent) parent = tfMainRoot;
        node.transform.SetParent(parent, worldPositionStays);
        showingDialogs.Add(node.gameObject);
        return node;
    }
    public static T AddNode<T>(string s_file, bool worldPositionStays = false) where T : Component {
        return AddNode<T>(s_file, tfMainRoot, worldPositionStays);
    }
    public static T AddTopNode<T>(string s_file, bool worldPositionStays = false) where T : Component {
        return AddNode<T>(s_file, tfTopRoot, worldPositionStays);
    }
    public static T AddSuperNode<T>(string s_file, bool worldPositionStays = false) where T : Component {
        return AddNode<T>(s_file, tfSuperRoot, worldPositionStays);
    }
    public static T AddSceneNode<T>(string s_file, bool worldPositionStays = false) where T : Component {
        return AddNode<T>(s_file, objCurScene.transform, worldPositionStays);
    }
}