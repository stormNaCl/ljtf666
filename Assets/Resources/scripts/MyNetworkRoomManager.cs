using Mirror;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MyNetworkRoomManager : NetworkRoomManager
{
    public static MyNetworkRoomManager instance;
    
    public Text debugText;
    Gmt gmt;
    public static List<Transform> frontStartPos;
    public static List<Transform> backendStartPos;
    [SerializeField]
    public bool canStart;
    [Header("Lobby Scenes")]
    [Scene]
    public string lobbyScene = "LobbyScene"; // 玩家准备的大厅场景

    [Scene]
    public string assignmentScene = "AssignmentScene"; // 分工大厅场景

    [Scene]
    public string gameScene = "GameScene"; // 实际游戏场景

    public static void RegisterFrontStartPos(Transform pos)
    {
        frontStartPos.Add(pos);
    }
    public static void RegisterBackendStartPos(Transform pos)
    {
        backendStartPos.Add(pos);
    }
    public static void UnRegisterFrontStartPos(Transform pos)
    {
        frontStartPos.Remove(pos);
    }
    public static void UnRegisterBackendStartPos(Transform pos)
    {
        backendStartPos.Remove(pos);
    }
    //public override Transform GetStartPosition()
    //{
    //    // first remove any dead transforms
    //    startPositions.RemoveAll(t => t == null);
    //
    //    if (startPositions.Count == 0)
    //        return null;
    //
    //    if (playerSpawnMethod == PlayerSpawnMethod.Random)
    //    {
    //        return startPositions[UnityEngine.Random.Range(0, startPositions.Count)];
    //    }
    //    else
    //    {
    //        Transform startPosition = startPositions[startPositionIndex];
    //        startPositionIndex = (startPositionIndex + 1) % startPositions.Count;
    //        return startPosition;
    //    }
    //    
    //}
    public override void ServerChangeScene(string newSceneName)
    {
        if (newSceneName == RoomScene)
        {
            gmt.frontids.Clear();
            gmt.backids.Clear();
            canStart = false;
        }
        base.ServerChangeScene(newSceneName);

    }
    public override void Start()
    {
        base.Start();
        
        instance = this;
        gmt = GameObject.Find("gameManager").GetComponent<Gmt>();
        

       //frontids = new SyncList<ulong>();
       //backids = new SyncList<ulong>();
    }
    public override void ReadyStatusChanged()
    {
        if(gmt == null)
        {
            gmt = GameObject.Find("gameManager").GetComponent<Gmt>();
        }
        if (!(gmt.backids.Count+gmt.frontids.Count==roomSlots.Count))
        {
            allPlayersReady = false;
            canStart = false;
            return;
        }
        if(!canStart)
        {
            allPlayersReady = false;
            return;
        }
        int CurrentPlayers = 0;
        int ReadyPlayers = 0;

        foreach (NetworkRoomPlayer item in roomSlots)
        {
            if (item != null)
            {
                CurrentPlayers++;
                if (item.readyToBegin)
                    ReadyPlayers++;
            }
        }

        if (CurrentPlayers == ReadyPlayers)
            CheckReadyToBegin();
        else
        {
            allPlayersReady = false;
            canStart = false;
        }
            
    }
    public override void OnServerReady(NetworkConnectionToClient conn)
    {
        if (conn.identity == null)
        {
            // this is now allowed (was not for a while)
            //Debug.Log("Ready with no player object");
        }
        NetworkServer.SetClientReady(conn);

        if (conn != null && conn.identity != null)
        {
            GameObject roomPlayer = conn.identity.gameObject;

            // if null or not a room player, don't replace it
            if (roomPlayer != null && roomPlayer.GetComponent<NetworkRoomPlayer>() != null)
                SceneLoadedForPlayer(conn, roomPlayer);
        }
    }
    void SceneLoadedForPlayer(NetworkConnectionToClient conn, GameObject roomPlayer)
    {
        //Debug.Log($"NetworkRoom SceneLoadedForPlayer scene: {SceneManager.GetActiveScene().path} {conn}");

        if (Utils.IsSceneActive(RoomScene))
        {
            // cant be ready in room, add to ready list
            PendingPlayer pending;
            pending.conn = conn;
            pending.roomPlayer = roomPlayer;
            pendingPlayers.Add(pending);
            return;
        }

        GameObject gamePlayer = OnRoomServerCreateGamePlayer(conn, roomPlayer);
        if (gamePlayer == null)
        {
            // get start position from base class
            Transform startPos = GetStartPosition();
            Debug.Log("scasdasdcasdasca");
            gamePlayer = startPos != null
                ? Instantiate(Resources.Load<GameObject>("prefabs/"+Gmt.GetChara(conn))
                , startPos.position, startPos.rotation)
                : Instantiate(Resources.Load<GameObject>("prefabs/" + Gmt.GetChara(conn))
                , Vector3.zero, Quaternion.identity);
        }

        if (!OnRoomServerSceneLoadedForPlayer(conn, roomPlayer, gamePlayer))
            return;

        // replace room player with game player
        NetworkServer.ReplacePlayerForConnection(conn, gamePlayer, ReplacePlayerOptions.KeepAuthority);
    }
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
         base.OnServerAddPlayer(conn);
    }
    
    





}
