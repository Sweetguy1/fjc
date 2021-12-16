using System;
using UnityEngine;

public class ListenerRectChange : MonoBehaviour
{
    public enum EnumMode {
        None = 0,
        ScaleX,
        ScaleY,
        ScaleMin,
        ScaleMax
    }
    public enum EnumRule {
        Uniform, //等比
        notUniform,//非等比
        notUniformX,
        notUniformY,
    }
    public EnumRule kScaleRule = EnumRule.Uniform; //是否等比缩放
    public EnumMode kScaleMode = EnumMode.None; //适配调整点 缩放
    public EnumMode kSizeMode = EnumMode.None; //缩放的属性

    private Vector3 _v3Scale;
    private Rect _rect;
    private void Awake()
    {
        RectChangeListenerManager.AddRectChangeListener(OnRectChange);
        var transform1 = transform;
        _v3Scale = transform1.localScale;
        _rect = ((RectTransform) transform1).rect;
    }

    private void OnEnable()
    {
        OnRectChange(RectChangeListenerManager.scaleX, RectChangeListenerManager.scaleY);
    }

    private void OnDestroy()
    {
        RectChangeListenerManager.DelRectChangeListener(OnRectChange);
    }

    private void OnRectChange(float x, float y)
    {
        InitScale(x, y);
        InitSize(x, y);
    }
    
    
    private void GetScaleRatio(EnumMode kMode, ref float scaleX, ref float scaleY) {
        switch (kMode) {
            case EnumMode.ScaleX:
                if (this.kScaleRule == EnumRule.Uniform) scaleY = scaleX;
                break;
            case EnumMode.ScaleY:
                if (this.kScaleRule == EnumRule.Uniform) scaleX = scaleY;
                break;
            case EnumMode.ScaleMin:
                if (this.kScaleRule == EnumRule.notUniformX) scaleX = Mathf.Min(scaleX, scaleY);
                else if(this.kScaleRule == EnumRule.notUniformY) scaleY = Mathf.Min(scaleX, scaleY);
                else 
                    scaleY = scaleX = Mathf.Min(scaleX, scaleY);
                break;
            case EnumMode.ScaleMax:
                if (this.kScaleRule == EnumRule.notUniformX) scaleX = Mathf.Max(scaleX, scaleY);
                else if(this.kScaleRule == EnumRule.notUniformY) scaleY = Mathf.Max(scaleX, scaleY);
                else 
                    scaleY = scaleX = Mathf.Max(scaleX, scaleY);
                break;
        }
    }
    private void InitScale(float scaleX, float scaleY) {
        if (this.kScaleMode == EnumMode.None) return;
        this.GetScaleRatio(this.kScaleMode, ref scaleX, ref scaleY);

        var transform1 = this.transform;
        var scale = transform1.localScale;
        scale.x = _v3Scale.x * scaleX;
        scale.y = _v3Scale.x * scaleY;
        transform1.localScale = scale;
    }
    private void InitSize(float scaleX, float scaleY) {
        if (this.kSizeMode == EnumMode.None) return;
        this.GetScaleRatio(this.kSizeMode, ref scaleX, ref scaleY);
        var rtf = (this.transform as RectTransform);
        var size = rtf.sizeDelta;
        size.x = _rect.width * scaleX;
        size.y = _rect.height * scaleY;
        rtf.sizeDelta = size;
    }
}
