using UnityEngine;
using Mirror;
using Unity.Burst.Intrinsics;

public class PlayerAim : NetworkBehaviour
{
    public Player player;
    public PlayControl playControl;
    public PlayerWeaponController playerWeaponController;
    private Vector2 aimInput;
    public LineRenderer lineRenderer;
    [Header("…‰ª˜ Ù–‘")]
    [SerializeField] private LayerMask aimLayerMask;
    [SerializeField] private Vector3 lookDirection;
    [SerializeField] private Transform aim;
    [SerializeField] private float maxDistance = 1.5f;
    public Vector3 shootDir;
    public GameObject cameraFollowInstance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        playerWeaponController = GetComponent<PlayerWeaponController>();
        player = GetComponent<Player>();
        //cameraFollowInstance = transform.Find("CameraFollowInstance").gameObject;
        AssignInputSystem();
    }
    public void AssignInputSystem()
    {
        playControl = player.playControl;
        playControl.Player.Look.performed += ctx => aimInput = ctx.ReadValue<Vector2>();
        playControl.Player.Look.canceled += ctx => aimInput = Vector2.zero;
    }
    private void HandleAim()
    {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, aimLayerMask))
        {
            lookDirection = hitInfo.point - transform.position;
            lookDirection.y = 0;
            lookDirection.Normalize();
            transform.forward = lookDirection;
            //aim.gameObject.SetActive(true);
            //aim.position =hitInfo.point;

        }
        else
        {
            //aim.gameObject.SetActive(false);

        }

        if (hitInfo.collider != null)
        {
            Ray ray2 = new Ray(new Vector3(transform.position.x, transform.position.y + .5f, transform.position.z), hitInfo.point - new Vector3(transform.position.x, transform.position.y + .5f, transform.position.z));
            Vector3 f = (hitInfo.point - new Vector3(transform.position.x, transform.position.y + .5f, transform.position.z)).normalized * maxDistance;
            f.y = transform.position.y+.5f;
            //cameraFollowInstance.transform.position = f;
            Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + .5f, transform.position.z), hitInfo.point - new Vector3(transform.position.x, transform.position.y + .5f, transform.position.z), Color.red);
            if (Physics.Raycast(ray2, out var hitInfo2, Mathf.Infinity, aimLayerMask))
            {
                aim.gameObject.SetActive(true);
                aim.position = hitInfo2.point;
                Vector3 ff = (hitInfo2.point-transform.position).normalized;
                cameraFollowInstance.transform.position = transform.position+ff*maxDistance;
                shootDir = (hitInfo2.point - new Vector3(transform.position.x, transform.position.y + .5f, transform.position.z)).normalized;
                //aim.position = new Vector3(hitInfo2.point.x, headTransform.position.y, hitInfo2.point.z);
            }
            else
            {
                aim.gameObject.SetActive(false);
                Vector3 fff = transform.forward * maxDistance;
                fff.y = transform.position.y + .5f;
                cameraFollowInstance.transform.position = transform.position + fff;
            }
        }
        else
        {
            Vector3 f = transform.forward * maxDistance;
            f.y = transform.position.y + .5f;
            cameraFollowInstance.transform.position = transform.position+f;
        }
        lineRenderer.SetPosition(0, playerWeaponController.weapons[playerWeaponController.currentWeaponIndex].bulletHole.position);
        lineRenderer.SetPosition(1, aim.position);
    }
    private void FixedUpdate()
    {
        if (!isLocalPlayer)
            return;
        HandleAim();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
