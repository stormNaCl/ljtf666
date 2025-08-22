using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIbase : MonoBehaviour
{
    public bool canBeCloseByESC;
    public UIEventTrigger Register(string name)
    {
        Transform tf = transform.Find(name);
        return UIEventTrigger.Get(tf.gameObject);
    }
    public virtual void Close()
    {
        GameLogic.GetModule<UImanager>().CloseUI(gameObject.name);
    }
    public virtual void Show()
    {
        gameObject.SetActive(true);
    }
    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        
    }
}
