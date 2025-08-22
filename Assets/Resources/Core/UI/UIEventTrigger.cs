using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIEventTrigger : MonoBehaviour, IPointerClickHandler
{
    public Action<GameObject, PointerEventData> onClick;
    public static UIEventTrigger Get(GameObject obj)
    {
        UIEventTrigger uitrigger = obj.GetComponent<UIEventTrigger>();
        if (uitrigger == null)
        {
            uitrigger = obj.AddComponent<UIEventTrigger>();
        }
        return uitrigger;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData != null)
        {
            onClick(gameObject, eventData);
        }
    }
}
