using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameManager : MonoSingleton<GameManager>
{
    public static FiniteStateMachine Flow = new FiniteStateMachine();
    protected abstract void GameInitial();
    protected abstract void FlowInitial();
    protected abstract void GameStart();
    protected override void Awake()
    {
        base.Awake();
        GameInitial();
        FlowInitial();
        GameStart();
    }

    protected void Update()
    {
        GameLogic.Update();
        Flow.Update();
    }
}
