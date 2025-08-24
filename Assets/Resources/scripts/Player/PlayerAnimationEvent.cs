using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour
{
    private PlayerWeaponController weaponController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        weaponController = GetComponentInParent<PlayerWeaponController>();
    }
    public void OnReloadFinished()
    {
        weaponController.WhenNeedOpRigWeight();
        //TODO fill
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
