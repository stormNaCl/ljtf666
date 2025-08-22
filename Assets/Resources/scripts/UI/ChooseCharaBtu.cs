using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChooseCharaBtu : MonoBehaviour
{
    [SerializeField]
    public PlayerRole pr;
    
    public void Init(PlayerRole pre)
    {
        pr = pre;
        GetComponent<Button>().onClick.AddListener(Btu);
    }

    public void Btu()
    {
        NetworkClient.localPlayer.GetComponent<roomPlayerCol>().ChooseChara(pr.self_name);
        GameLogic.GetModule<UImanager>().
            GetUI<ChooseCharaPanel>("ChooseCharaPanel").chosenPanel.GetComponentInChildren<Image>().sprite = pr.icon;
        GameLogic.GetModule<UImanager>().
            GetUI<ChooseCharaPanel>("ChooseCharaPanel").chosenPanel.transform.GetChild(1).GetComponent<TMP_Text>().text = pr.self_name;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
