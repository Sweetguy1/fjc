using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MouseTouchListening : MonoBehaviour {

    private Dictionary<int, Vector2> dictTouchs = new Dictionary<int, Vector2> ();
    private bool bMove = false;

    [Serializable] public class FunTouch : UnityEvent<Vector3> { }

    [Serializable] public class FunScale : UnityEvent<Vector3, float> { }
    public FunTouch FunTouchOptDown;
    public FunTouch FunTouchOptClick;
    public FunTouch FunTouchOptMove;
    public FunScale FunTouchOptScale;

    private void Awake () {
        EventTrigger eventTrigger = this.gameObject.GetComponent<EventTrigger> ();
        if (eventTrigger == null) {
            eventTrigger = this.gameObject.AddComponent<EventTrigger> ();
        }
        EventTrigger.Entry entryPointerDown = new EventTrigger.Entry ();
        entryPointerDown.eventID = EventTriggerType.PointerDown;
        entryPointerDown.callback.AddListener (OnPointerDown);

        EventTrigger.Entry entryPointerUp = new EventTrigger.Entry ();
        entryPointerUp.eventID = EventTriggerType.PointerUp;
        entryPointerUp.callback.AddListener (OnPointerUp);

        EventTrigger.Entry entryDrag = new EventTrigger.Entry ();
        entryDrag.eventID = EventTriggerType.Drag;
        entryDrag.callback.AddListener (OnDrag);

        EventTrigger.Entry entryCancel = new EventTrigger.Entry ();
        entryCancel.eventID = EventTriggerType.Cancel;
        entryCancel.callback.AddListener (OnCancel);

        eventTrigger.triggers.Add (entryPointerDown);
        eventTrigger.triggers.Add (entryPointerUp);
        eventTrigger.triggers.Add (entryDrag);
        eventTrigger.triggers.Add (entryCancel);
    }

    private void Update () {
// #if UNITY_EDITOR_WIN //镜头拉近
        float value = Input.GetAxis ("Mouse ScrollWheel");
        if (value == 0) return;
        this.FunTouchOptScale?.Invoke (new Vector2 (0, 0), -value * 500);
// #endif
    }

    public void OnPointerDown (BaseEventData base_data) {
        PointerEventData pdate = base_data as PointerEventData;
        // MDebug.Log ("======OnPointerDown:" + pdate.position + pdate.pressPosition + pdate.delta + pdate.pointerId);
        this.dictTouchs[pdate.pointerId] = pdate.position;
        if (this.dictTouchs.Count != 2) {
            this.bMove = false;
            this.FunTouchOptDown?.Invoke(pdate.position);
        }
    }
    public void OnPointerUp (BaseEventData base_data) {
        PointerEventData pdate = base_data as PointerEventData;
        // MDebug.Log("======OnPointerUp:" + pdate.position + pdate.pressPosition + pdate.delta);
        this.dictTouchs.Remove (pdate.pointerId);
        if (this.dictTouchs.Count != 0 || this.bMove) return;
        this.FunTouchOptClick?.Invoke (pdate.position);
    }
    public void OnDrag (BaseEventData base_data) {
        PointerEventData pdate = base_data as PointerEventData;
        // MDebug.Log ("======OnMove:" + pdate.position + pdate.pressPosition + pdate.delta + pdate.pointerId);
        Vector2 vCur = pdate.position;
        // Vector2 vSub = vCur - pdate.pressPosition;
        this.bMove = true;

        if (this.dictTouchs.Count == 1) { //单手指 移动
            this.FunTouchOptMove?.Invoke (vCur - this.dictTouchs[pdate.pointerId]);
            this.dictTouchs[pdate.pointerId] = vCur;
        } else if (this.dictTouchs.Count == 2) { //双手指 放大
            Vector2[] vars = this.dictTouchs.Values.ToArray ();
            Vector2 vCenter = vars[0] + ((vars[1] - vars[0]) * 0.5f);
            float nMagLast = Vector2.Distance (vars[1], vars[0]);

            this.dictTouchs[pdate.pointerId] = pdate.position;
            vars = this.dictTouchs.Values.ToArray ();
            float nMagCur = Vector2.Distance (vars[1], vars[0]);
            float nMagSub = nMagCur - nMagLast;
            if (nMagSub != 0) this.FunTouchOptScale?.Invoke (vCenter, -nMagSub);
        }

    }
    public void OnCancel (BaseEventData base_data) {
        // MDebug.Log("======OnCancel:");
        this.OnPointerUp (base_data);
    }
}