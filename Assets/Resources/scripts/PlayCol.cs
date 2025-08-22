using UnityEngine;
using Mirror;
using TMPro;
using Steamworks;
using Unity.VisualScripting;
using UnityEngine.AI;
using Mirror.Examples.Common;

public class PlayCol : NetworkBehaviour
{
    [SerializeField]
    private float speed = 2f;
    [SerializeField]
    private float boostMutiplayer = 4f;
    [SerializeField]
    private float jumpforce = 5f;
    [SerializeField]
    public TMP_Text nameText;
    [SerializeField]
    private LayerMask groundLayer;
    [SerializeField]
    private GameObject flipObject;
    [SerializeField]
    private Transform groundCheckTransform;

    MyNetworkRoomManager roomManager = MyNetworkRoomManager.instance;
    Gmt gmt = Gmt.instance;
    private Rigidbody rb;
    private bool isBoost;
    private bool isOnGround;
    private Vector2 moveDir;
    private bool facingRight = true;
    private ulong _mID;
    [Header("Movement Settings")]
    public float walkSpeed = 4.0f;
    public float runSpeed = 8.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public float turnSmoothTime = 0.1f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.2f;
    public LayerMask groundMask;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private float currentSpeed;
    private float turnSmoothVelocity;
    private Animator animator;

    // 寻路相关
    [Header("Navigation")]
    public bool useNavMesh = true; // 是否使用NavMesh寻路
    private NavMeshAgent navMeshAgent;
    [Header("Mouse Movement")]
    public float stoppingDistance = 0.5f;
    // 鼠标移动相关
    private Vector3 targetPosition;
    private bool hasTarget = false;
    private Camera playerCamera;
    //狗子
    [SyncVar(hook = nameof(OnNameChanged))]
    private string palyerName;
    [SyncVar(hook = nameof(OnRoleChanged))]
    public string role;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        //rb.gravityScale = 1.0f;

        if (!isLocalPlayer)
            return;
        // 添加或获取NavMeshAgent组件
        if (useNavMesh)
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            if (navMeshAgent == null)
            {
                navMeshAgent = gameObject.AddComponent<NavMeshAgent>();
            }

            // 配置NavMeshAgent
            navMeshAgent.speed = walkSpeed;
            navMeshAgent.angularSpeed = 120f;
            navMeshAgent.acceleration = 8f;
            navMeshAgent.stoppingDistance = stoppingDistance;
            navMeshAgent.autoBraking = true;
            navMeshAgent.enabled = false; // 初始禁用，只在需要时启用
        }
        string steamName = SteamFriends.GetPersonaName();
        if(NetworkClient.ready)
        {
            CmdSetPlayerName(steamName);
            _mID = SteamUser.GetSteamID().m_SteamID;
            Debug.Log(_mID);
            CmdSetPlayerRole(_mID);
        }
        playerCamera = Camera.main;
        Unity.Cinemachine.CameraTarget tar = new Unity.Cinemachine.CameraTarget();
        tar.TrackingTarget = gameObject.transform;
        tar.LookAtTarget = gameObject.transform;
        playerCamera.GetComponent<CameraCol>().cc.Target = tar;
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        
    }
    
    public void OnRoleChanged(string oldName, string newName)
    {
        nameText.text += role;
    }
    private void Update()
    {
        if (!isLocalPlayer)
            return;
        if (role == "Front")
        {
 
            CheckGrounded();
            HandleMovement();
            HandleJump();
            ApplyGravity();

            //Camera.main.transform.position = new Vector3(transform.position.x
            //, transform.position.y + 7f,
            //transform.position.z - ((Mathf.Atan(Camera.main.transform.rotation.eulerAngles.x)) * 7)+2);
            //Debug.Log(Mathf.Atan(Camera.main.transform.rotation.eulerAngles.x));
            //controller.Move(velocity * Time.deltaTime);
            //if (Input.GetButtonDown("Jump") && isOnGround)
            //{
            //    Jump();
            //}
            
            nameText.text = SteamFriends.GetPersonaName() + role;


            //isOnGround = Physics2D.OverlapCircle(groundCheckTransform.position, 0.1f, groundLayer);
        }
    }
        private void FixedUpdate()
        {
            if (!isLocalPlayer)
                return;
            if (role == "Front")
            {

                //float currentMoveSpeed = isBoost ? speed * boostMutiplayer : speed;
                //transform.Translate(moveDir * currentMoveSpeed * Time.fixedDeltaTime);
            }

        }
        private void HandleMouseInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundMask))
                {
                    SetTargetPosition(hit.point);
                    Debug.DrawLine(ray.origin, hit.point, Color.red, 2f);
                }
            }
        }

        [Client]
        private void SetTargetPosition(Vector3 position)
        {
            targetPosition = position;
            hasTarget = true;

            if (useNavMesh && navMeshAgent != null)
            {
                // 使用NavMesh寻路
                navMeshAgent.enabled = true;
                navMeshAgent.SetDestination(targetPosition);
            }
        }

        private void HandleNavMeshMovement()
        {
            if (!hasTarget || navMeshAgent == null)
                return;

            // 检查是否到达目标
            if (!navMeshAgent.pathPending)
            {
                if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
                {
                    if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f)
                    {
                        // 到达目标
                        hasTarget = false;
                        navMeshAgent.enabled = false;

                        if (animator != null)
                        {
                            animator.SetFloat("Speed", 0f);
                        }
                        return;
                    }
                }
            }

            // 使用NavMeshAgent移动
            if (navMeshAgent.isOnNavMesh && navMeshAgent.isActiveAndEnabled)
            {
                // 获取移动方向和速度
                Vector3 moveDirection = navMeshAgent.desiredVelocity.normalized;
                float speed = navMeshAgent.desiredVelocity.magnitude;

                // 角色转向
                if (moveDirection.magnitude >= 0.1f)
                {
                    float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
                    float angle = Mathf.SmoothDampAngle(
                        transform.eulerAngles.y,
                        targetAngle,
                        ref turnSmoothVelocity,
                        turnSmoothTime
                    );
                    transform.rotation = Quaternion.Euler(0f, angle, 0f);
                }

                // 更新动画
                if (animator != null)
                {
                    animator.SetFloat("Speed", speed / walkSpeed);
                }
            }
        }

        private void HandleDirectMovement()
        {
            if (!hasTarget)
                return;

            Vector3 directionToTarget = targetPosition - transform.position;
            directionToTarget.y = 0;

            float distanceToTarget = directionToTarget.magnitude;

            if (distanceToTarget <= stoppingDistance)
            {
                hasTarget = false;
                if (animator != null)
                {
                    animator.SetFloat("Speed", 0f);
                }
                return;
            }

            Vector3 moveDirection = directionToTarget.normalized;

            if (moveDirection.magnitude >= 0.1f)
            {
                float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
                float angle = Mathf.SmoothDampAngle(
                    transform.eulerAngles.y,
                    targetAngle,
                    ref turnSmoothVelocity,
                    turnSmoothTime
                );
                transform.rotation = Quaternion.Euler(0f, angle, 0f);
            }

            float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
            controller.Move(moveDirection * currentSpeed * Time.deltaTime);

            if (animator != null)
            {
                animator.SetFloat("Speed", moveDirection.magnitude * (currentSpeed / walkSpeed));
            }
        }

        private void CheckGrounded()
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }
        }
        float tmpH = 0;
        float tmpV = 0;
        private void HandleMovement()
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
            if(Mathf.Abs(moveHorizontal)-Mathf.Abs(tmpH)<0)
            {
                if(moveHorizontal<.2f)
                {
                    moveHorizontal = 0; 
                }
            }
            if (Mathf.Abs(moveVertical) - Mathf.Abs(tmpV) < 0)
            {
                if (moveVertical < .2f)
                {
                    moveVertical = 0;
                }
            }
        Vector3 direction = playerCamera.transform.TransformDirection(moveHorizontal, 0f, moveVertical).normalized;
        direction.y = 0;


            if (!(moveHorizontal == 0 && moveVertical == 0))
            {
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

                float angle = Mathf.LerpAngle(transform.eulerAngles.y, targetAngle, .3f);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);


            }
            tmpH = moveHorizontal;
            tmpV = moveVertical;


            transform.Translate(direction * currentSpeed * Time.deltaTime,Space.World);
            
            // 更新动画参数
            if (animator != null)
            {
                animator.SetFloat("Speed", direction.magnitude * (currentSpeed / walkSpeed));
            }

            else if (animator != null)
            {
                animator.SetFloat("Speed", 0f);
            }
        }

        private void HandleJump()
        {
            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                rb.AddForce(jumpSpeed * Vector3.up);

                if (animator != null)
                {
                    animator.SetTrigger("Jump");
                }
            }
        }
        private void ApplyGravity()
        {
            // 正确应用重力到 CharacterController
            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f; // 贴地
            }
            else
            {
                velocity.y += gravity;
            }
        }


        // 用于动画事件调用
        public bool IsGrounded()
        {
            return isGrounded;
        }
        private void Jump()
        {
            //rb.AddForce(Vector2.up * jumpforce, ForceMode2D.Impulse);
        }

        private void OnNameChanged(string oldName, string newName)
        {
            nameText.text = newName;
        }
        [Command]
        private void CmdSetPlayerName(string name)
        {
            palyerName = name;
        }
        [Command]
        private void CmdSetPlayerRole(ulong id)
        {
            if (gmt.frontids.Contains(id))
            {
                checkRole(connectionToClient, "Front");
            }
            else
            {
                checkRole(connectionToClient, "Backend");
            }

        }
        [TargetRpc]
        private void checkRole(NetworkConnectionToClient conn, string role)
        {
            this.role = role;
            if (role == "Front")
            {

                GameLogic.GetModule<UImanager>().ShowUI<FrontUI>("FrontUI", false);
            
                //Camera.main.transform.SetParent(transform, false);
                //Camera.main.transform.localPosition = new Vector3(0, 6, 0);
            }
            else
            {
                Camera.main.orthographicSize = 19;
                GetComponentInChildren<SpriteRenderer>().enabled = false;
                GetComponent<CapsuleCollider2D>().enabled = false;
                for (int i = 0; i < transform.childCount; i++)
                {
                    transform.GetChild(i).gameObject.SetActive(false);
                }

            }

        }
    
}
