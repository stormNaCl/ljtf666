using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    private Player player;
    private PlayControl control;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        player = GetComponent<Player>();
        control = player.playControl;
        control.Player.Attack.performed += (ctx) => Shoot();
        
    }
    public void Shoot()
    {
        GetComponentInChildren<Animator>().SetTrigger("Fire");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
