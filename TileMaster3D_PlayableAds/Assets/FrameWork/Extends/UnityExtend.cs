using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class UnityExtend {

    public static RectTransform RT (this Component self) => self.transform as RectTransform;
    public static Rect Rect (this Component self) => (self.transform as RectTransform).rect;
    public static void SetWidth (this Transform self, float width) {
        RectTransform rtf = (self as RectTransform);
        Vector2 size = rtf.sizeDelta;
        // MDebug.Log ("=====SetWidth size:" + size + "width:" + width);
        size.x = width;
        rtf.sizeDelta = size;
    }
    public static void SetHeight (this Transform self, float height) {
        RectTransform rtf = (self as RectTransform);
        Vector2 size = rtf.sizeDelta;
        // MDebug.Log ("=====SetHeight size:" + size + "height:" + height);
        size.y = height;
        rtf.sizeDelta = size;
    }
    public static void SetSize (this Transform self, float width, float height) {
        RectTransform rtf = (self as RectTransform);
        Vector2 size = rtf.sizeDelta;
        size.x = width;
        size.y = height;
        rtf.sizeDelta = size;
    }

    public static void SetPivot (this Transform self, float x, float y) {
        RectTransform rtf = (self as RectTransform);
        Vector2 pivot = rtf.pivot;
        pivot.x = x;
        pivot.y = y;
        rtf.pivot = pivot;
    }

    public static void SetLocPos (this Transform self, float x, float y, float z) {
        Vector3 pos = self.localPosition;
        pos.x = x;
        pos.y = y;
        pos.z = z;
        self.localPosition = pos;
    }
    public static void SetLocPos (this Transform self, float x, float y) {
        Vector3 pos = self.localPosition;
        pos.x = x;
        pos.y = y;
        self.localPosition = pos;
    }
    public static void SetLocPos (this Transform self, Vector2 vec2) {
        Vector3 pos = self.localPosition;
        pos.x = vec2.x;
        pos.y = vec2.y;
        self.localPosition = pos;
    }
    public static void SetLocPosX (this Transform self, float x) {
        Vector3 pos = self.localPosition;
        pos.x = x;
        self.localPosition = pos;
    }
    public static void Set3DLocPosX (this Transform self, float x) {
        Vector3 pos = self.localPosition;
        pos.x = x / 100;
        self.localPosition = pos;
    }
    public static void SetLocPosY (this Transform self, float y) {
        Vector3 pos = self.localPosition;
        pos.y = y;
        self.localPosition = pos;
    }
    public static void SetPos (this Transform self, float x, float y) {
        Vector3 pos = self.position;
        pos.x = x;
        pos.y = y;
        self.position = pos;
    }
    public static void SetAnchorPos (this Transform self, float x, float y) {
        RectTransform rtf = (self as RectTransform);
        Vector3 pos = rtf.anchoredPosition;
        pos.x = x;
        pos.y = y;
        rtf.anchoredPosition = pos;
    }
    public static void SetAnchorPosX (this Transform self, float x) {
        RectTransform rtf = (self as RectTransform);
        Vector3 pos = rtf.anchoredPosition;
        pos.x = x;
        rtf.anchoredPosition = pos;
    }
    public static void SetAnchorPosY (this Transform self, float y) {
        RectTransform rtf = (self as RectTransform);
        Vector3 pos = rtf.anchoredPosition;
        pos.y = y;
        rtf.anchoredPosition = pos;
    }

    public static void SetRotation (this Transform self, float z) {
        Quaternion quaternion = self.rotation;
        self.rotation = Quaternion.Euler (quaternion.x, quaternion.y, z);
    }
    public static void SetLocRotation (this Transform self, float z) {
        Quaternion quaternion = self.localRotation;
        self.localRotation = Quaternion.Euler (quaternion.x, quaternion.y, z);
    }

    public static void SetScale (this Transform self, float v) {
        Vector3 scale = self.localScale;
        scale.x = v;
        scale.y = v;
        self.localScale = scale;
    }
    public static void SetScale3 (this Transform self, float v) {
        Vector3 scale = self.localScale;
        scale.x = v;
        scale.y = v;
        scale.z = v;
        self.localScale = scale;
    }

    public static Graphic SetAlpha (this Graphic self, float alpha) {
        Color color = self.color;
        color.a = alpha;
        self.color = color;
        return self;
    }
    public static void SetColors (this Transform self, Color color) {
        Graphic[] graphics = self.GetComponentsInChildren<Graphic> ();
        foreach (var g in graphics) { g.color = color; }
    }

    /// ///////////////////////////////////////////////////////////////
    public static T FindC<T> (this Component self, string name) {
        return self.transform.Find (name).GetComponent<T> ();
    }
    public static T FindC<T> (this Transform self, string name) {
        return self.Find (name).GetComponent<T> ();
    }

    public static void DestroySelf (this Component tf) {
        // if (!UnityEngine.AddressableAssets.Addressables.ReleaseInstance(tf.gameObject)){
            GameObject.Destroy(tf.gameObject);
        // }
    }
    // public static void DestroysInChildren (this Transform self) {
    //     foreach (Transform tf in self) {
    //         tf.DOKill ();
    //         tf.DestroySelf();
    //     }
    // }

    public static bool IsActive (this Component self) {
        return self.gameObject.activeSelf;
    }

    public static void Show (this Component self) {
        self.gameObject.SetActive (true);
    }
    public static void Show (this GameObject self) {
        self.SetActive (true);
    }
    public static void Hide (this Component self) {
        self.gameObject.SetActive (false);
    }
    public static void Hide(this GameObject self) {
        self.SetActive (false);
    }

    public static string GetFullName (this Transform self) {
        List<string> names = new List<string> ();
        string fullname = "";
        Transform tf = self;
        while (tf != null) {
            names.Add (tf.name);
            tf = tf.parent;
        }
        names.Reverse ();
        foreach (var s in names) {
            fullname += s + ".";
        }
        return fullname.Substring (0, fullname.Length - 1).Replace ("Canvas (Environment).", "");
    }

    /// ///////////////////////////////////////////////////////////////

    /// <summary>
    /// A、B两个点，求B相对A的角度
    /// B.ToAngle(A.position);
    /// </summary>
    /// <param name="self"></param>
    /// <param name="pos">世界坐标</param>
    /// <returns>正上方为0，逆时针</returns>
    public static float ToAngle (this Transform self, Vector2 pos) {
        pos = self.InverseTransformPoint (pos);
        float angle = Mathf.Atan (pos.y / pos.x) * Mathf.Rad2Deg;
        angle = angle + (pos.x >= 0.0f ? 90.0f : 270.0f);
        return angle;
    }

}