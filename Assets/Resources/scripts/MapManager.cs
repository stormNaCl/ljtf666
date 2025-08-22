using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.U2D;


public class MapManager : BaseManager<MapManager>, IModule
{
    public GameObject roadPre;
    public UnityEngine.Transform roadParent;
    public void Handle(object msg)
    {
        
    }

    public void Initial()
    {
        GetAB("prefabs/roadPre");
    }
    public void GenerateMap()
    {
        if (!NetworkServer.active)
        {
            Debug.Log("不是服务端");
            return;
        }
        Debug.Log("开始生成地图");
        GameObject mft = new GameObject("MapFatherTransform");
        mft.transform.position = Vector3.zero;
        roadParent = mft.transform;


    }
    public void GetAB(string bundleName)
    {
        GameLogic.GetModule<ABManager>().LoadAndCacheBundle(bundleName);
        
        roadPre = GameLogic.GetModule<ABManager>().LoadAssetFromCachedBundle<GameObject>(bundleName, "roadpre 1");
        
    }
    public void CreateMainRoad()
    {

    }
    public void Update()
    {
        
    }


}



