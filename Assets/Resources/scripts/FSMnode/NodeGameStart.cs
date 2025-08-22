using UnityEngine;

public class NodeGameStart : IFsmNode
{
    public string Name { get; }
    public NodeGameStart()
    {
        Name = nameof(NodeGameStart);
    }

    public void OnEnter()
    {
        Debug.Log("ÓÎÏ·¿ªÊ¼");
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
