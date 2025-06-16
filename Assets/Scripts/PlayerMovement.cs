using UnityEngine;

public class PlayerMovement : MonoBehaviour 
{
    [Header("Movement")]
    public float moveSpeed = 50f;
    public float sprintSpeed = 90f;
    public float crouchSpeed = 3f;
    [SerializeField]  public float airMul = 0.00001f;

    [Header("Crouch Settings")]
    public float playerHeight = 1f;
    public float crouchHeight = 1f;
    public float crouchTransitionSpeed = 8f;

    [Header("Audio")]
    public AudioClip footstepClip;
    private float stepTimer = 0f;
    public float walkStepInterval = 0.5f;
    public float sprintStepInterval = 0.2f;
    private float stepInterval = 0.0f;

    private float targetHeight;
    public float currentHeight;
    private Vector3 targetCenter;
    private Vector3 currentCenter;

    [SerializeField] Transform orientation;

    float horizontalMovement;
    float verticalMovement;

    bool isGrounded;
    public float groundDistance = 0.1f;
    [SerializeField] LayerMask groundMask;
    [SerializeField] LayerMask pickupMask;

    Vector3 moveDirection;
    Vector3 slopeMoveDirection;

    Rigidbody rb;

    [Header("KeyBinds")]
    [SerializeField] KeyCode jumpKey = KeyCode.Space;
    [SerializeField] KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] KeyCode crouchKey = KeyCode.LeftControl;
    [SerializeField] KeyCode bugKey = KeyCode.P;

    [Header("Jumping")]
    public float jumpForce = 6f;

    [Header("Drag")]
    public float groundDrag = 6f;
    public float airDrag = 2f;

    RaycastHit slopeHit;

    public float yGround = 0.7f;

    public float currentSpeed;

    private CapsuleCollider coll;

    public MoveCamera cameraControl;

    bool isCrouching = false;

    private AudioSource audioSource;

    private bool onSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight / 2 + 0.5f))
        {
            if(slopeHit.normal != Vector3.up)
            {
                return true;
            }
            else { return false; }
        }
        return false;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        currentSpeed = moveSpeed;
        coll = GetComponent<CapsuleCollider>();
        audioSource = GetComponentInChildren<AudioSource>();
        

        currentHeight = playerHeight;
        targetHeight = playerHeight;

        coll.height = playerHeight;
        coll.center = Vector3.up * (playerHeight / 2f);
        currentCenter = coll.center;
        targetCenter = currentCenter;
        stepInterval = walkStepInterval;



    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(transform.position - new Vector3(0, yGround, 0), groundDistance, groundMask) || Physics.CheckSphere(transform.position - new Vector3(0, yGround, 0), groundDistance, pickupMask);

        myInput();
        controlDrag();

        if (Input.GetKeyDown(jumpKey) && isGrounded) 
        {
            jump();
        }

        if (Input.GetKeyDown(bugKey))
        {
            transform.position = new Vector3(65, 0.4f, 70);
        }

        if(Input.GetKey(sprintKey) && !isCrouching)
        {
            currentSpeed = sprintSpeed;
            stepInterval = sprintStepInterval;
        }
        else if(Input.GetKey(sprintKey) && isCrouching)
        {
            currentSpeed = moveSpeed;
            stepInterval = walkStepInterval;
        }

        else
        {
            if (!isCrouching)
            { currentSpeed = moveSpeed;
              stepInterval = walkStepInterval;
            }
        }



        slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);

        if (Input.GetKeyDown(crouchKey))
        {
            isCrouching = true;
            targetHeight = crouchHeight;
            currentSpeed = crouchSpeed;
            targetCenter = Vector3.up * (crouchHeight / 2f);

        }
        else if (Input.GetKeyUp(crouchKey))
        {
            // Проверка: нет ли препятствия над головой
            if (!Physics.SphereCast(transform.position, coll.radius, Vector3.up, out RaycastHit hit, playerHeight))
            {
                isCrouching = false;
                targetHeight = playerHeight;
                currentSpeed = moveSpeed;
                targetCenter = Vector3.up * (playerHeight / 2f);
                
            }
            // иначе остаёмся в приседе
        }

        currentHeight = Mathf.Lerp(currentHeight, targetHeight, Time.deltaTime * crouchTransitionSpeed);
        coll.height = currentHeight;
        currentCenter = Vector3.Lerp(currentCenter, targetCenter, Time.deltaTime * crouchTransitionSpeed);
        coll.center = currentCenter;

        
        cameraControl.crouch(currentHeight);
    }

    void myInput()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");

        moveDirection = orientation.forward * verticalMovement + orientation.right * horizontalMovement;

    }
    void jump()
    {
        if (isGrounded)
        {
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

        }
        
    }


    void controlDrag() 
    {
        if(isGrounded)
        {
            rb.linearDamping = groundDrag;
        }
        else
        {
            rb.linearDamping = airDrag;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    void MovePlayer()
    {

        if (isGrounded && !onSlope())
        {
            rb.AddForce(moveDirection.normalized * currentSpeed, ForceMode.Acceleration);
            walkSound();
        }

        else if(isGrounded && onSlope())
        {
            rb.AddForce(slopeMoveDirection.normalized * currentSpeed, ForceMode.Acceleration);
            walkSound();
        }

        else if(!isGrounded)
        {
            rb.AddForce(moveDirection.normalized * currentSpeed * airMul, ForceMode.Acceleration);
        }
        else
        {
            stepTimer = 0f;
        }
        
    }

    void walkSound()
    {
        if (footstepClip == null || audioSource == null) return;

        if (moveDirection.magnitude > 0.1 && isGrounded)
        {
            stepTimer += Time.deltaTime;
            if (stepTimer >= stepInterval)
            {
                audioSource.PlayOneShot(footstepClip, 0.65f);
                stepTimer = 0f;
            }
        }
        
    }
}
