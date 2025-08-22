using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerMovement : NetworkBehaviour
{
    private PlayControl playControl;
    private Player player;
    [SerializeField] private CharacterController characterController;
    [Header("移动属性")]
    [SerializeField] private Vector2 moveInput;
    [SerializeField] private Vector2 aimInput;
    [SerializeField] private Vector3 moveDirection;
    [SerializeField] private float verticalVelocity;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private bool isRunning;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpSpeed = 10f;
    [SerializeField] private bool jumpTrigger;
    private Transform dirTransform;
    private GameObject dirObj;
    [Header("射击属性")]
    [SerializeField] private LayerMask aimLayerMask;
    [SerializeField] private Vector3 lookDirection;
    [SerializeField] private Transform aim;
    [Header("动画属性")]
    [SerializeField] private Animator animator;

    
    private void AssignInputSystem()
    {
        
        playControl.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        playControl.Player.Move.canceled += ctx => moveInput = Vector2.zero;
        playControl.Player.Look.performed += ctx => aimInput = ctx.ReadValue<Vector2>();
        playControl.Player.Look.canceled += ctx => aimInput = Vector2.zero;
        playControl.Player.Sprint.performed += ctx => HandleRun(true);
        playControl.Player.Sprint.canceled += ctx => HandleRun(false);
        playControl.Player.Jump.performed += ctx => jumpTrigger = true;
        playControl.Player.Jump.canceled += ctx => jumpTrigger = false;
        
        
    }
    private void HandleRun(bool res)
    {
        isRunning = res;
        animator.SetBool("isRunning", res);
    }
    
    
    private void AnimatorController()
    {
        Vector3 ss = Camera.main.transform.InverseTransformDirection(moveDirection);
        ss = transform.TransformDirection(ss);
        
        float xx = Vector3.Dot(moveDirection.normalized, transform.right);
        float yy = Vector3.Dot(moveDirection.normalized, transform.forward);
        animator.SetFloat("xv", xx, .1f, Time.deltaTime);
        animator.SetFloat("yv", yy, .1f, Time.deltaTime);
    }
    public void HandleMovement()
    {
        float currentSpeed = isRunning ? moveSpeed*1.6f : moveSpeed;
        moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
        HandleGravity();
        
        //HandleJump();
        if (moveDirection.magnitude > 0||jumpTrigger)
        {
            moveDirection = dirTransform.TransformDirection(moveDirection);
            characterController.Move(moveDirection * Time.deltaTime * currentSpeed);
        }
    }
    public void HandleGravity()
    {
        if(characterController.isGrounded)
        {
            verticalVelocity = -.5f;
            if(jumpTrigger)
            {
                verticalVelocity = jumpSpeed;
            }
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
            moveDirection.y = verticalVelocity;
        }
        
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GetComponent<Player>();
        playControl = player.playControl;
        if (!isLocalPlayer)
            return;
        aim = GameObject.Find("aim").transform;
        characterController = GetComponent<CharacterController>();
        dirObj = new GameObject();
        dirTransform = dirObj.transform;
        dirTransform.position = Camera.main.transform.position;
        //dirTransform.rotation = Camera.main.
        dirTransform.rotation = Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0);
        animator = GetComponentInChildren<Animator>();
        AssignInputSystem();
    }
    private void HandleAim()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, aimLayerMask))
        {
            lookDirection = hitInfo.point - transform.position;
            lookDirection.y = 0;
            lookDirection.Normalize();
            transform.forward = lookDirection;
            //aim.position = new Vector3(hitInfo.point.x, transform.position.y, hitInfo.point.z);
        }
        Ray ray2 = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray2, out var hitInfo2, Mathf.Infinity, aimLayerMask))
        {
            aim.gameObject.SetActive(true);
            aim.position = new Vector3(hitInfo2.point.x, transform.position.y, hitInfo2.point.z);
        }
        else
        {
            aim.gameObject.SetActive(false);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
            return;
        HandleMovement();
        AnimatorController();
        dirTransform = dirObj.transform;
        dirTransform.position = Camera.main.transform.position;
        //dirTransform.rotation = Camera.main.
        dirTransform.rotation = Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0);
    }
    private void FixedUpdate()
    {
        if (!isLocalPlayer)
            return;
        HandleAim();
    }
}
