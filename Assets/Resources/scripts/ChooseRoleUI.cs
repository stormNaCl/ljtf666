using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Steamworks;
using Mirror;
using Unity.VisualScripting;
using TMPro;
using UnityEngine.SceneManagement;

public class ChooseRoleUI : NetworkBehaviour
{
    [SerializeField]
    public GameObject leftPanel;
    [SerializeField]
    public GameObject rightPanel;
    [SerializeField]
    public List<ulong> m_front_playerIDs;
    [SerializeField]
    public List<ulong> m_backend_playerIDs;
    [SerializeField]
    public MyNetworkRoomManager _roomManager;
    public Object _playerHeadImg;
    public ulong tmp_playerID;
    [SerializeField]
    public Dictionary<ulong, GameObject> m_playerHeadUI;
    [SerializeField]
    public int readyCount;
    [Scene]
    public string realGameScence;
    NetworkConnectionToClient conn = null;
    Gmt gmt = Gmt.instance;
    public TMP_Text test_txt;
    private ulong m_ID;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _roomManager = MyNetworkRoomManager.instance;
       // m_front_playerIDs = _roomManager.frontids;
        //m_backend_playerIDs = _roomManager.backids;
        m_playerHeadUI = new Dictionary<ulong, GameObject>();
        if (isServer)
        {
            conn = connectionToClient;
            Debug.Log($"{conn}");
        }
        if(!isServer)
        {
            
            m_ID = SteamUser.GetSteamID().m_SteamID;
            test_txt.text = m_ID.ToString();
        }
    }
    
    public void StartGame()
    {
        
        
        if (!isServer)
        {
            // TODO:提示只有腐竹能点
            return;
        }
        Debug.Log(_roomManager.numPlayers);
        _roomManager.gameScene = realGameScence;
        if(readyCount!=_roomManager.numPlayers)
        {
            Debug.Log("人数不够");
            // TODO:提示人数不够
            return;
        }
        if(!Gmt.CheckifReady(MyNetworkRoomManager.instance.roomSlots.Count))
        {
            Debug.Log("有人没选角色");
            return;
        }
        _roomManager.canStart = true;
        _roomManager.CheckReadyToBegin();
        
    }
    public Sprite SteamAvatarToSprite(int avatarHandle)
    {
        if (avatarHandle == 0)
        {
            Debug.LogWarning("Invalid avatar handle");
            return null;
        }

        // 获取头像尺寸
        uint width, height;
        if (!SteamUtils.GetImageSize(avatarHandle, out width, out height))
        {
            Debug.LogWarning("Failed to get avatar image size");
            return null;
        }

        // 获取头像数据
        byte[] avatarData = new byte[width * height * 4]; // RGBA格式
        if (!SteamUtils.GetImageRGBA(avatarHandle, avatarData, (int)(width * height * 4)))
        {
            Debug.LogWarning("Failed to get avatar image data");
            return null;
        }

        // 创建Texture2D
        Texture2D texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false);
        texture.LoadRawTextureData(avatarData);
        texture.Apply();

        // 转换为Sprite
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                                      new Vector2(0.5f, 0.5f), 100.0f);

        return sprite;
    }
    public void jf()
    {
        NetworkClient.localPlayer.GetComponent<roomPlayerCol>().jf();
    }
    public void jb()
    {
        NetworkClient.localPlayer.GetComponent<roomPlayerCol>().jb();
    }
    public void ShowChooseCharaUI()
    {
        GameLogic.GetModule<UImanager>().ShowUI<ChooseCharaPanel>("ChooseCharaPanel", true);
    }
    public void GetFriendsSteamIds()
    {
        if (SteamManager.Initialized)
        {
            int friendCount = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate);
            Debug.Log($"好友数量: {friendCount}");

            for (int i = 0; i < friendCount; i++)
            {
                CSteamID friendSteamId = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagImmediate);
                //SteamFriends.get
                string friendName = SteamFriends.GetFriendPersonaName(friendSteamId);
                ulong steamIdValue = friendSteamId.m_SteamID;

                Debug.Log($"好友 {i}: {friendName} - SteamID: {steamIdValue}");
            }
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
