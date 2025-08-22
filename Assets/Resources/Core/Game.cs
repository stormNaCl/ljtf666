
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using TMPro;

public class Game : GameManager
{
    
    protected override void GameInitial()
    {
        
        GameLogic.CreateModule<EventModule>();
        GameLogic.CreateModule<ABManager>();
        GameLogic.CreateModule<UImanager>();
        
        GameLogic.CreateModule<MapManager>();
        
       
    }
    protected override void FlowInitial()
    {
        Flow.AddNode(new NodeGameStart());
        Flow.AddNode(new NodePrepareStart());
    }
    
    protected override void GameStart()
    {
        

    }
    public new void Update()
    {
        
    }

}
