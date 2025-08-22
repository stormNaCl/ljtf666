using Mirror;
using System.Collections;
using UnityEngine;

public class BattleManager : MonoBehaviour
{   
    public static BattleManager instance;
    public static int prepareTime = 60;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
    }
    public IEnumerator prepareTimeDJS()
    {
        while(prepareTime>0)
        {
            prepareTime--;
            yield return new WaitForSeconds(1);
        }
        if(prepareTime<=0)
        {
            Game.Flow.Run(nameof(NodeGameStart));
        }
    }
   
    // Update is called once per frame
    void Update()
    {
        
    }
}
