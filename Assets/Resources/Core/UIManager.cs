using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using DG.Tweening;
using UnityEngine.UI;
using Mirror.Examples.Basic;

public class UImanager : BaseManager<UImanager>, IModule
{
    //public static UImanager instance;
    public Transform canvasTF;
    public List<UIbase> uiList;
    private void Awake()
    {
        
    }
    public UIbase ShowUI<T>(string uiname,bool canbeclosebyesc) where T : UIbase
    {
        UIbase ui = Find(uiname);
        if (ui == null)
        {
            canvasTF = GameObject.Find("Canvas").transform;
            GameObject ob = GameObject.Instantiate(Resources.Load("prefabs/UI/" + uiname), canvasTF) as GameObject;
            ob.name = uiname;
            ui = ob.AddComponent<T>();
            ui.canBeCloseByESC = canbeclosebyesc;
            uiList.Add(ui);

        }
        else
        {
            ui.Show();
        }
        return ui;
    }
    public UIbase ShowUI(string uiname)
    {
        UIbase ui = Find(uiname);
        if (ui == null)
        {
            GameObject ob = GameObject.Instantiate(Resources.Load("prefabs/UI/" + uiname), canvasTF) as GameObject;
            ob.name = uiname;
            //ui = ob.AddComponent<T>();
            uiList.Add(ui);

        }
        else
        {
            ui.Show();
        }
        return ui;
    }

    public void HideUI(string uiName)
    {
        UIbase ui = Find(uiName);
        if (ui != null)
        {
            ui.Hide();
        }

    }
    public void CloseUI(string uiName)
    {
        UIbase ui = Find(uiName);
        if (ui != null)
        {
            uiList.Remove(ui);
            GameObject.Destroy(ui.gameObject);
        }
    }
    public void CloseAll()
    {
        for (int i = uiList.Count - 1; i >= 0; i--)
        {
            GameObject.Destroy(uiList[i].gameObject);
        }
        uiList.Clear();
    }
    public UIbase Find(string uiName)
    {
        for (int i = 0; i < uiList.Count; i++)
        {
            if (uiList[i].name == uiName)
            {
                return uiList[i];
            }
        }
        return null;
    }
    public T GetUI<T>(string uiName) where T : UIbase
    {
        UIbase ui = Find(uiName);
        if (ui != null)
        {
            return ui.GetComponent<T>();
        }
        return null;
    }
    public GameObject CreateActionIcon()
    {
        GameObject obj = GameObject.Instantiate(Resources.Load("UI/actionIcon"), canvasTF) as GameObject;
        obj.transform.SetAsFirstSibling();
        return obj;
    }
    public GameObject CreateHpItem()
    {
        GameObject obj = GameObject.Instantiate(Resources.Load("UI/HpItem"), canvasTF) as GameObject;
        obj.transform.SetAsFirstSibling();
        return obj;
    }
    public GameObject CreateZtItem(string path)
    {
        GameObject obj = GameObject.Instantiate(Resources.Load("Model/Image"), canvasTF) as GameObject;
        obj.GetComponent<Image>().sprite = Resources.Load<Sprite>(path);
        // obj.transform.SetAsFirstSibling();
        return obj;
    }
    public void ShowTip(string msg, Color color, System.Action callback = null)
    {
        GameObject obj = GameObject.Instantiate(Resources.Load("UI/Tips"), canvasTF) as GameObject;
        Text text = obj.transform.Find("bg/Text").GetComponent<Text>();
        text.text = msg;
        text.color = color;
        Tween scale = obj.transform.Find("bg").DOScale(1, 0.4f);
        Tween scale2 = obj.transform.Find("bg").DOScale(0, 0.4f);
        Sequence seq = DOTween.Sequence();
        seq.Append(scale);
        seq.AppendInterval(0.5f);
        seq.Append(scale2);
        seq.AppendCallback(delegate ()
        {
            if (callback != null)
            {
                callback();
            }
        });
        MonoBehaviour.Destroy(obj, 2);

    }
    public void OnSceneLoad()
    {
        canvasTF = GameObject.Find("Canvas").transform;
    }
    public void Initial()
    {
        //instance = this;
        canvasTF = GameObject.Find("Canvas").transform;
        uiList = new List<UIbase>();
    }

    public void Update()
    {
       //bool havetoclose = false;
       //if (Input.GetKeyDown(KeyCode.Escape))
       //{
       //    for(int i = uiList.Count-1;i>=0;i--)
       //    {
       //        if (uiList[i].canBeCloseByESC == true)
       //        {
       //            if(uiList[i].GetComponent<Map>()!=null)
       //            {
       //                HideUI(uiList[i].gameObject.name);
       //            }
       //            else
       //                CloseUI(uiList[i].gameObject.name);
       //            havetoclose = true;
       //            break;
       //        }
       //    }
       //    if(!havetoclose)
       //    {
       //        GameLogic.GetModule<PlayerModule>().OpenMyBag();
       //    }
       //    
       //}
    }

    public void Handle(object msg)
    {
        
    }
}
