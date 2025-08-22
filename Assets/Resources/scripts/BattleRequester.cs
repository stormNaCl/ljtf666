using JetBrains.Annotations;
using Mirror;
using UnityEngine;

public class BattleRequester : NetworkBehaviour
{
    public string role;
    private PlayCol pc;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pc = GetComponent<PlayCol>();
        role = pc.role;
        
        
        
    }
    void Update()
    {
        if (GameLogic.GetModule<UImanager>().GetUI<FrontUI>("FrontUI")!=null)
        {
            CmdGetPrepareTime();
        }
        
    }
    [Command]
    public void CmdGetPrepareTime()
    {
        int time = BattleManager.prepareTime;
        RpcSetTimeText(time);
    }
    [ClientRpc]
    public void RpcSetTimeText(int time)
    {
        GameLogic.GetModule<UImanager>().GetUI<FrontUI>("FrontUI").time.text = time.ToString();
    }
    // Update is called once per frame
    
}
