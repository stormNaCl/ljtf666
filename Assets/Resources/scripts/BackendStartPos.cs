using UnityEngine;

public class BackendStartPos : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        MyNetworkRoomManager.RegisterBackendStartPos(this.transform);
    }
    public void OnDestroy()
    {
        MyNetworkRoomManager.UnRegisterBackendStartPos(this.transform);
    }
}
