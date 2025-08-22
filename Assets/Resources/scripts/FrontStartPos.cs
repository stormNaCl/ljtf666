using UnityEngine;

public class FrontStartPos : MonoBehaviour
{
    private void Awake()
    {
        MyNetworkRoomManager.RegisterFrontStartPos(this.transform);
    }
    public void OnDestroy()
    {
        MyNetworkRoomManager.UnRegisterFrontStartPos(this.transform);
    }
}
