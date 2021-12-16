using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
// using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public static class TweenMgr {
    public static Vector3 vTempVec3 = Vector3.zero;
    public static Vector3 SetVec3(float x, float y, float z = 0) { vTempVec3.Set(x, y, z); return vTempVec3; }

    public static Sequence ScaleMinToMax(this Transform tf, float min, float max, float time) {
        tf.localScale = SetVec3(min, min, 1);
        var sequence = DOTween.Sequence();
        sequence.Append(tf.DOScale(SetVec3(max + 0.1f, max + 0.1f, 1), time));
        sequence.AppendInterval(0.1f);
        sequence.Append(tf.DOScale(SetVec3(max - 0.05f, max - 0.05f, 1), 0.1f));
        sequence.Append(tf.DOScale(SetVec3(max, max, 1), 0.1f));
        return sequence;
    }
    public static Sequence ScaleMaxToMin(this Transform tf, float max, float min, float time) {
        tf.localScale = SetVec3(max, max, 1);
        var sequence = DOTween.Sequence();
        sequence.Append(tf.DOScale(SetVec3(max + 0.1f, max + 0.1f, 1), 0.05f));
        sequence.AppendInterval(0.1f);
        sequence.Append(tf.DOScale(SetVec3(min, min, 1), time));
        return sequence;
    }

    ///////////////////////////////////////////////////////////////////////////////////////////
    private static int _n_call_value = 0;
    public static Tweener DelayCall(float delay, TweenCallback callback) {
        return DOTween.To(() => _n_call_value, v => _n_call_value = v, 0, delay).OnComplete(callback);
        // DOTween.Sequence ().AppendInterval (delay).AppendCallback (callback);
    }
    public static Tweener NumberTo(float delay, float f_start, float f_end, System.Action<float> callback = null) {
        return DOTween.To(v => {
            if (callback != null) callback(v);
        }, f_start, f_end, delay);
    }

    ///////////////////////////////////////////////////////////////////////////////////////////

}