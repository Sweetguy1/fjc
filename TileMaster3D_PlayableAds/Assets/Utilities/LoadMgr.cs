using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LoadMgr {
    public static T LoadRes<T>(string s_file) where T : Object {
        var res = Resources.Load<T>(s_file);
        if (res == null) {
            MDebug.Log($"load res error file:{s_file}", MDebug.Color.yellow);
            return null;
        }
        T obj = GameObject.Instantiate<T>(res);
        obj.name = res.name;
        return obj;
    }
    public static Sprite LoadSprite(string s_file) {
        var res = Resources.Load<Sprite>(s_file);
        if (res == null) {
            MDebug.Log($"load texture2d error file:{s_file}", MDebug.Color.yellow);
            return null;
        }
        return res;
    }
    public static string LoadText(string s_file, string type = "") {
        var res = Resources.Load(s_file);
        if (res == null) {
            MDebug.Log($"load text error file:{s_file}", MDebug.Color.yellow);
            return null;
        }
        return (res as TextAsset).text;
    }

    public static T Load<T>(string s_file) where T : Object {
        var res = Resources.Load<T>(s_file);
        if (res == null) {
            MDebug.Log($"load error file:{s_file}", MDebug.Color.yellow);
            return null;
        }
        return res;
    }

    public static T LoadJson<T>(string s_file) {
        var ajson = LoadText(s_file, ".json");
        if (ajson == null) return default(T);
        // MDebug.Log($"====LoadJson ajson:{ajson}");
        return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(ajson);
    }
}