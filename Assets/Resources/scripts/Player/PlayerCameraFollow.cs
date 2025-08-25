using UnityEngine;
using Mirror;
using Unity.Cinemachine;

public class PlayerCameraFollow : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!isLocalPlayer)
            return;
        GameObject.Find("cc").GetComponent<CinemachineCamera>().Follow = transform.Find("CameraFollowInstance");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
