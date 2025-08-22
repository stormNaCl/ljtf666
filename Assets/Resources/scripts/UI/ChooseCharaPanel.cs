using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class ChooseCharaPanel : UIbase
{
    public List<PlayerRole> playerRole;

    private GameObject content;
    private GameObject iconPre;
    private Button cancel;
    private Button confirm;
    public GameObject chosenPanel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameLogic.GetModule<ABManager>().LoadAndCacheBundle("so");
        playerRole = new List<PlayerRole>();
        foreach(var sb in GameLogic.GetModule<ABManager>().LoadAllAssetFromCachedBundle<PlayerRole>("so"))
        {
            playerRole.Add(sb);
        }
        GameLogic.GetModule<ABManager>().LoadAndCacheBundle("ui");
        iconPre = GameLogic.GetModule<ABManager>().LoadAssetFromCachedBundle<GameObject>("ui", "chooseCharaBtu");
        
        content = transform.GetChild(0).GetChild(0).GetChild(0).gameObject;
        CreateChooseIcon();
        cancel = transform.Find("cancel").GetComponent<Button>();
        cancel.onClick.AddListener(Close);
        confirm = transform.Find("confirm").GetComponent<Button>();
        confirm.onClick.AddListener(Close);
        chosenPanel = transform.Find("chosenPanel").gameObject;
    }
    public void CreateChooseIcon()
    {
        foreach(var sb in playerRole)
        {
            if(sb == null) continue;
            GameObject dsb = Instantiate(iconPre,content.transform);
            dsb.GetComponentInChildren<TMP_Text>().text = sb.self_name;
            dsb.GetComponent<Image>().sprite = sb.icon;
            sb.prefab = GameLogic.GetModule<ABManager>().LoadAssetFromCachedBundle<GameObject>("so", sb.name);
            Debug.Log(sb.prefab.name);
            dsb.AddComponent<ChooseCharaBtu>();
            dsb.GetComponent<ChooseCharaBtu>().Init(sb);
        }
    }
    public void CloseSelf()
    {
        Close();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
