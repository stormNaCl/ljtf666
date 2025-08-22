using Mirror;
using UnityEngine;

public class NodePrepareStart : IFsmNode
{
    public string Name { get; }
    public NodePrepareStart()
    {
        Name = nameof(NodePrepareStart);
    }

    public void OnEnter()
    {
        if (NetworkServer.active)
        {
            Debug.Log("准备时间开始倒计时");
            
            Game.Instance.StartCoroutine(BattleManager.instance.prepareTimeDJS());
        }
    }

    public void OnExit()
    {
        
    }

    public void OnFixedUpdate()
    {
        
    }

    public void OnHandleMessage(object msg)
    {
        
    }

    public void OnUpdate()
    {
        
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
