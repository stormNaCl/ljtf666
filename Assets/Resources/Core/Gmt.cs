using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class Gmt : MonoBehaviour
{
    public static Gmt instance;
    private static Dictionary<NetworkConnection, string> conn_2_Chara;
    public List<ulong> frontids = new List<ulong>();
    public List<ulong> backids = new List<ulong>();
    public int readyCount = 0;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        conn_2_Chara = new Dictionary<NetworkConnection, string>();
        // 确保列表初始化
        if (frontids == null)
            frontids = new List<ulong>();
        if (backids == null)
            backids = new List<ulong>();
    }
    public static void SaveConn2Chara(NetworkConnection conn, string chara)
    {
        Debug.Log(conn+chara);
        if(conn_2_Chara.ContainsKey(conn))
        {
            conn_2_Chara[conn] = chara;
        }
        else
        {
            conn_2_Chara.Add(conn, chara);
        }
    }
    public static void ClearOneConn2Chara(NetworkConnection conn)
    {
        if (conn_2_Chara.ContainsKey(conn))
        {
            conn_2_Chara.Remove(conn);
        }
    }
    public static void ClearAllConn2Chara()
    {
        conn_2_Chara.Clear();
    }
    public static string GetChara(NetworkConnection conn)
    {
        if (conn_2_Chara.ContainsKey(conn))
        {
            return conn_2_Chara[conn];
        }
        else
        {
            return null;
        }
    }
    public static bool CheckifReady(int num)
    {
        Debug.Log(num+"asdasd"+conn_2_Chara.Count);
        if(num == conn_2_Chara.Count)
        {
            return true;
        }
        return false;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
