using TMPro;
using UnityEngine;
using Steamworks;
using System.Collections.Generic;
using Edgegap;
using Mirror.FizzySteam;

public class SteamLobby : MonoBehaviour
{
    public static SteamLobby instance;
    public TMP_Text debugText;
    public MyNetworkRoomManager _roomManager;
    private const string hostAddressKey = "HostAddress";
    protected Callback<LobbyCreated_t> lobbyCreate_t;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested_t;
    protected Callback<LobbyEnter_t> LobbyEnter_t;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(!SteamManager.Initialized)
        {
            debugText.text = "Steam初始化失败或未连接到Steam服务器";
            return;
        }
        debugText.text = "Steam初始化成功";
        
        _roomManager = GetComponent<MyNetworkRoomManager>();

        lobbyCreate_t = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        gameLobbyJoinRequested_t = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        LobbyEnter_t = Callback<LobbyEnter_t>.Create(OnLobbyEntered);

    }
    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if(callback.m_eResult != EResult.k_EResultOK)
        {
            debugText.text = "创建房间失败";
            return;
        }
        debugText.text = "创建房间成功";
        _roomManager.StartHost();
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), hostAddressKey, SteamUser.GetSteamID().ToString());
    }
    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        debugText.text = "收到申请";
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }
    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        debugText.text = "有玩家进入大厅";
        string hostAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), hostAddressKey);
        _roomManager.networkAddress = hostAddress;
        if(!_roomManager.isNetworkActive)
        {
            _roomManager.StartClient();
            debugText.text = "玩家正在连接到主机";
        }
    }
    public void HostLobby()
    {
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic,_roomManager.maxConnections);
    }    
    // Update is called once per frame
    void Update()
    {
        
    }
}
