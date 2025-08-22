using Mirror;
using System;
using Unity.Cinemachine;
using UnityEngine;

public class CameraCol : MonoBehaviour
{
    public CinemachineCamera cc;
    private void Start()
    {
        cc = GameObject.Find("fc").GetComponent<CinemachineCamera>();
    }

}
