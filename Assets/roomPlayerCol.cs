using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Steamworks;
using Mirror;
using Unity.VisualScripting;
using TMPro;
using UnityEngine.SceneManagement;
public class roomPlayerCol : NetworkBehaviour
{
    private ulong m_ID;
    Gmt gmt = Gmt.instance;
    public ChooseRoleUI cru;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(isLocalPlayer)
        {
            m_ID = SteamUser.GetSteamID().m_SteamID;
        }
        if(gmt == null)
        {
            gmt = Gmt.instance;
        }
        cru = GameObject.Find("crp").GetComponent<ChooseRoleUI>();
    }
    [Command]
    public void ChooseChara(string playerPre)
    {
        Debug.Log(playerPre);
        Gmt.SaveConn2Chara(connectionToClient, playerPre);
        
    }
    public void jf()
    {
       // test_txt.text = "dianlejf";
        if (isLocalPlayer)
        {
            if(cru == null)
                cru = GameObject.Find("crp").GetComponent<ChooseRoleUI>();
            Debug.Log("本地请求front");
            //test_txt.text = "本地请求front";
            CmdJoinFront(m_ID);

        }
        else if (isServer)
        {
            ServerJoinFront();
        }

    }
    public void jb()
    {
        if (cru == null)
            cru = GameObject.Find("crp").GetComponent<ChooseRoleUI>();
        //test_txt.text = "dianlejb";
        if (isLocalPlayer)
        {
            Debug.Log("本地请求front");
            //t/est_txt.text = "本地请求front";
            CmdJoinBackend(m_ID);
        }
        else if (isServer)
        {
            ServerJoinBackend();
        }
    }
    [Command]
    public void CmdJoinFront(ulong id)
    {
        //test_txt.text = "jinlefmdjf";
        //ulong id = SteamUser.GetSteamID().m_SteamID;
        if (gmt.frontids.Contains(id))
        {
            Debug.Log("重复");
            return;
        }

        if (gmt.backids.Contains(id))
        {
            Debug.Log("huanbian");
            gmt.backids.Remove(id);
            RpcRemovePlayerHeadImgUI(id);
        }
        else
        {
            cru.readyCount++;
        }
        
        Debug.Log(id);
        gmt.frontids.Add(id);
        RpcCreatePlayerHeadImgUItoLeft(id);
        cru._roomManager.ReadyStatusChanged();
    }
    [Command]
    public void CmdJoinBackend(ulong id)
    {
        //test_txt.text = "jinlefmdjb";
        //ulong id = SteamUser.GetSteamID().m_SteamID;
        if (gmt.backids.Contains(id))
        {
            Debug.Log("重复");
            return;
        }

        if (gmt.frontids.Contains(id))
        {
            Debug.Log("huanbian");
            gmt.frontids.Remove(id);
            RpcRemovePlayerHeadImgUI(id);
        }
        else
        {
            cru.readyCount++;
        }
        //tmp_playerID = id;
        Debug.Log(id);
        gmt.backids.Add(id);
        RpcCreatePlayerHeadImgUItoRight(id);
        cru._roomManager.ReadyStatusChanged();
    }
    [Server]
    public void ServerJoinFront()
    {
        ulong id = SteamUser.GetSteamID().m_SteamID;
        if (gmt.frontids.Contains(id))
            return;
        if (gmt.backids.Contains(id))
        {
            gmt.backids.Remove(id);
            RpcRemovePlayerHeadImgUI(id);
        }
        else
        {
            cru.readyCount++;
        }
        //tmp_playerID = id;
        Debug.Log(id);
        gmt.frontids.Add(id);
        RpcCreatePlayerHeadImgUItoLeft(id);
        cru._roomManager.ReadyStatusChanged();
    }
    [Server]
    public void ServerJoinBackend()
    {
        ulong id = SteamUser.GetSteamID().m_SteamID;
        if (gmt.backids.Contains(id))
            return;
        if (gmt.frontids.Contains(id))
        {
            gmt.frontids.Remove(id);
            RpcRemovePlayerHeadImgUI(id);
        }
        else
        {
            cru.readyCount++;
        }
        //tmp_playerID = id;
        Debug.Log(id);
        gmt.backids.Add(id);
        RpcCreatePlayerHeadImgUItoRight(id);
        cru._roomManager.ReadyStatusChanged();
    }
    [ClientRpc]
    public void RpcCreatePlayerHeadImgUItoLeft(ulong id)
    {
        Debug.Log("xuanran");
        Image sb = Instantiate(cru._playerHeadImg, cru.leftPanel.transform).GetComponent<Image>();
        TMP_Text dsb = sb.GetComponentInChildren<TMP_Text>();
        dsb.text = SteamFriends.GetFriendPersonaName(new CSteamID(id));
        sb.sprite = cru.SteamAvatarToSprite(SteamFriends.GetSmallFriendAvatar(new CSteamID(id)));

        cru.m_playerHeadUI.Add(id, sb.gameObject);
    }
    [ClientRpc]
    public void RpcCreatePlayerHeadImgUItoRight(ulong id)
    {
        Debug.Log("xuanran");
        Image sb = Instantiate(cru._playerHeadImg, cru.rightPanel.transform).GetComponent<Image>();
        TMP_Text dsb = sb.GetComponentInChildren<TMP_Text>();
        dsb.text = SteamFriends.GetFriendPersonaName(new CSteamID(id));
        sb.sprite = cru.SteamAvatarToSprite(SteamFriends.GetSmallFriendAvatar(new CSteamID(id)));

        cru.m_playerHeadUI.Add(id, sb.gameObject);
    }
    [ClientRpc]
    public void RpcRemovePlayerHeadImgUI(ulong id)
    {
        foreach (var item in cru.m_playerHeadUI)
        {
            if (item.Key == id)
            {
                Destroy(item.Value);
            }
        }
        cru.m_playerHeadUI.Remove(id);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
