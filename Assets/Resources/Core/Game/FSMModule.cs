using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMModule : BaseManager<FSMModule>, IModule
{
    private FiniteStateMachine _fsm;
    public void Handle(object msg)
    {
        throw new System.NotImplementedException();
    }
    public void AddNode(IFsmNode node)
    {
        _fsm.AddNode(node);
    }
    public void Initial()
    {
        _fsm = new FiniteStateMachine();
    }

    void IModule.Update()
    {
        _fsm.Update();
    }
}
