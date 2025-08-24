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
    public void OnGrabFinished()
    {
        weaponController.OnGrabFinished();
    }
    public void WhenCanShowWeaponModel()
    {
        weaponController.CanActivateWeaponModel();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
