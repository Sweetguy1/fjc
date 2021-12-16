using System;
using UnityEngine;
using UnityEngine.UI;

public class RectChangeListenerManager : UnityEngine.EventSystems.UIBehaviour
{
    [SerializeField] private CanvasScaler _canvasScaler = null;
    private static Action<float, float> _eventRectChange;
    public static float scaleX, scaleY;
    
    public static void AddRectChangeListener(Action<float, float> callback)
    {
        _eventRectChange += callback;
    }
    public static void DelRectChangeListener(Action<float, float> callback)
    {
        _eventRectChange -= callback;
    }
    
    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();
        if(_canvasScaler == null) return;
        
        SetBasicValues();
        _eventRectChange?.Invoke(scaleX, scaleY);
    }
    
    public void SetBasicValues()
    {
        var rect = ((RectTransform) transform).rect;
        scaleX = rect.width / _canvasScaler.referenceResolution.x;
        scaleY = rect.height / _canvasScaler.referenceResolution.y;
        Debug.Log($"scale:{scaleX},{scaleY}   resolution:{_canvasScaler.referenceResolution}");
        // Debug.Log ("====SetBasicValues scale:" + scale.x + "," + scale.y + " w,h"+width+","+height +" rwh:"+canvasScaler.referenceResolution.x+","+canvasScaler.referenceResolution.y);
    }
}
