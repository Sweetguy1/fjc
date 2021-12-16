using DG.Tweening;
using UnityEngine;

public class TweenScale : MonoBehaviour {

    public float fDelay = 0.3f;
    public float fWait = 0.3f;
    public float fStart = 0;
    public float fEnd = 1;

    public bool bLoop = false;
    private Sequence sequence;

    private void OnEnable() {
        this.transform.SetScale(fStart);
        sequence = DOTween.Sequence().SetId("TweenScale_Start");
        sequence.Append(this.transform.DOScale(new Vector3(fEnd + 0.1f, fEnd + 0.1f, 1), fDelay));
        sequence.AppendInterval(0.1f);
        sequence.Append(this.transform.DOScale(new Vector3(fEnd, fEnd, 1), fDelay));
        if (!this.bLoop) return;
        sequence.AppendInterval(fWait);
        sequence.SetLoops(-1);
    }
    private void OnDisable() {
        sequence.Kill();
    }
}