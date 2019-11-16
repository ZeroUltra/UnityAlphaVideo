using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
public class EventTriggerListener : EventTrigger
{
    public EventTriggerConstom<PointerEventData> OnMouseClick = new EventTriggerConstom<PointerEventData>();
    public EventTriggerConstom<PointerEventData> OnMouseDown=new EventTriggerConstom<PointerEventData>();
    public EventTriggerConstom<PointerEventData> OnMouseEnter=new EventTriggerConstom<PointerEventData>();
    public EventTriggerConstom<PointerEventData> OnMouseExit=new EventTriggerConstom<PointerEventData>();
    public EventTriggerConstom<PointerEventData> OnMouseUp=new EventTriggerConstom<PointerEventData>();
    public EventTriggerConstom<PointerEventData> OnMouseDrag=new EventTriggerConstom<PointerEventData>();
    public EventTriggerConstom<PointerEventData> OnMouseBeginDrag=new EventTriggerConstom<PointerEventData>();
    public EventTriggerConstom<PointerEventData> OnMouseEndDrag=new EventTriggerConstom<PointerEventData>();
    public EventTriggerConstom<BaseEventData> OnMouseSelect=new EventTriggerConstom<BaseEventData>();
    public EventTriggerConstom<BaseEventData> OnMouseUpdateSelect=new EventTriggerConstom<BaseEventData>();
    public EventTriggerConstom<AxisEventData> OnMouseMove=new EventTriggerConstom<AxisEventData>();

    public static EventTriggerListener Get(GameObject go)
    {
        EventTriggerListener listener = go.GetComponent<EventTriggerListener>() ?? go.AddComponent<EventTriggerListener>();
        return listener;
    }
    public override void OnPointerClick(PointerEventData eventData)
    {

        if (OnMouseClick != null) OnMouseClick.Invoke(eventData);
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        if (OnMouseDown != null) OnMouseDown.Invoke(eventData);
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (OnMouseEnter != null) OnMouseEnter.Invoke(eventData);
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        if (OnMouseExit != null) OnMouseExit.Invoke(eventData);
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        if (OnMouseUp != null) OnMouseUp.Invoke(eventData);
    }
    public override void OnSelect(BaseEventData eventData)
    {
        if (OnMouseSelect != null) OnMouseSelect.Invoke(eventData);
    }
    public override void OnUpdateSelected(BaseEventData eventData)
    {
        if (OnMouseUpdateSelect != null) OnMouseUpdateSelect.Invoke(eventData);
    }
    public override void OnMove(AxisEventData eventData)
    {
        if (OnMouseMove != null) OnMouseMove.Invoke(eventData);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        OnMouseDrag?.Invoke(eventData);
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        OnMouseBeginDrag?.Invoke(eventData);
    }
    public override void OnEndDrag(PointerEventData eventData)
    {
        OnMouseEndDrag?.Invoke(eventData);
    }
    //-------------
    public class EventTriggerConstom<T> : UnityEvent<T>
    { }
}
