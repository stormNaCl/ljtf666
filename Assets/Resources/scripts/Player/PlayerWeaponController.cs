using Mirror;
using UnityEngine;
using UnityEngine.Animations.Rigging;

[System.Serializable]
public struct Weapon
{
    public string weaponName;
    public int damage;
    public GameObject weaponModel;
    public Animator weaponAnimator;
    public AudioClip switchSound;
    public int usingLayer;
    public GrabType grabType;
    // 其他武器属性...
}
public enum GrabType { side, back };
public class PlayerWeaponController : NetworkBehaviour
{
    private Player player;
    private PlayControl control;
    [SyncVar(hook = nameof(OnWeaponChanged))]
    public int currentWeaponIndex = 0;
    public Weapon[] weapons;
    public Transform tar;
    public Transform hint;
    private Animator animator;
    private Rig rig;
    private bool needOpRig2One;
    [SerializeField]
    private float OpRigStep = 2.5f;

    

    void Update()
    {
        if (!isLocalPlayer) return;

        // 检测切枪输入
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            CmdRequestSwitchWeapon(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            CmdRequestSwitchWeapon(1);
        }
        OpRigWeight();
        // 添加更多武器按键...
    }
    public void PlayGrabAnimation(GrabType grabType)
    {
        animator.SetFloat("WeaponGrabType", (float)grabType);
        animator.SetTrigger("grab");
        animator.SetBool("BusyUseWeapon",true);
    }
    private void OpRigWeight()
    {
        if(needOpRig2One)
        {
            rig.weight += OpRigStep * Time.deltaTime;
        }
        if(rig.weight >= 1)
        {
            needOpRig2One = false;
            //rig.weight = 1;
            return;
        }
    }
    public void WhenNeedOpRigWeight()=>needOpRig2One = true; 
    
    [Command]
    void CmdRequestSwitchWeapon(int newWeaponIndex)
    {
        Debug.Log("needqq");
        // 服务器端验证
        if (newWeaponIndex >= 0 && newWeaponIndex < weapons.Length)
        {
            currentWeaponIndex = newWeaponIndex;
            RpcOnWeaponSwitched(newWeaponIndex);
        }

    }
    void OnWeaponChanged(int oldIndex, int newIndex)
    {
        // 禁用所有武器模型
        foreach (Weapon weapon in weapons)
        {
            weapon.weaponModel.SetActive(false);
        }

        // 启用当前武器模型
        //weapons[newIndex].weaponModel.SetActive(true);
        SwitchUsingLayer();
        PlayGrabAnimation(weapons[newIndex].grabType);
        AttechTargetTransform();
        // 本地处理音效、动画等
        //PlaySwitchEffects(newIndex);
    }
    [ClientRpc]
    public void CanActivateWeaponModel() => weapons[currentWeaponIndex].weaponModel.SetActive(true);
    public void OnGrabFinished()=> animator.SetBool("BusyUseWeapon", false);

    [ClientRpc]
    void RpcOnWeaponSwitched(int newIndex)
    {
        // 所有客户端都会执行，但isLocalPlayer检查确保只对非本地玩家处理
        if (!isLocalPlayer)
        {
            OnWeaponChanged(currentWeaponIndex, newIndex);
        }
    }
    private void AttechTargetTransform()
    {
        //tar.SetParent(weapons[currentWeaponIndex].weaponModel.transform.parent);
        tar.localPosition = weapons[currentWeaponIndex].weaponModel.GetComponentInChildren<LeftTransformChecker>().transform.localPosition;
        tar.localRotation = weapons[currentWeaponIndex].weaponModel.GetComponentInChildren<LeftTransformChecker>().transform.localRotation;
        hint.localPosition = weapons[currentWeaponIndex].weaponModel.GetComponentInChildren<LeftTransformChecker>().transform.localPosition;
        hint.localRotation = weapons[currentWeaponIndex].weaponModel.GetComponentInChildren<LeftTransformChecker>().transform.localRotation;
    }
    public void SwitchUsingLayer()
    {
        for(int i = 1;i<animator.layerCount;i++)
        {
            animator.SetLayerWeight(i, 0);
        }
        animator.SetLayerWeight(weapons[currentWeaponIndex].usingLayer, 1);
    }
   //void PlaySwitchEffects(int index)
   //{
   //    // 播放切枪音效
   //    AudioSource.PlayClipAtPoint(weapons[index].switchSound, transform.position);
   //
   //    // 播放切枪动画
   //    if (weapons[index].weaponAnimator != null)
   //    {
   //        weapons[index].weaponAnimator.Play("SwitchWeapon");
   //    }
   //}
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        player = GetComponent<Player>();
        animator = GetComponentInChildren<Animator>();
        control = player.playControl;
        rig = GetComponentInChildren<Rig>();
        control.Player.Attack.performed += (ctx) => Shoot();
        control.Player.Reload.performed += (ctx) => Reload();
        CmdRequestSwitchWeapon(0);
        currentWeaponIndex = 0;

    }
    public void Reload()
    {
        animator.SetTrigger("reload");
        rig.weight = 0;
    }
    public void Shoot()
    {
        GetComponentInChildren<Animator>().SetTrigger("Fire");
    }
    // Update is called once per frame
    
}
