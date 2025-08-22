using Mirror;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public PlayControl playControl;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        playControl = new PlayControl();
    }
    private void OnEnable()
    {

        playControl.Enable();
    }
    private void OnDisable()
    {

        playControl.Disable();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
