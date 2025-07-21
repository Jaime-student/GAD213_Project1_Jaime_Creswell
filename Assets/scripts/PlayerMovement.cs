using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [Header("Movement")]
    public float moveSpeed; //base speed for the player

    public float groundDrag;

    public Transform TeleportTarget; //the variable for teleporting the position
    public Transform TeleportTarget2; //the variable for teleporting the position
    public Transform TeleportTarget3; //the variable for teleporting the position
    public GameObject ThePlayer; //the variable for teleporting the player

    public float jumpForce; //increases the force at which the player is pushed upwards
    public float jumpCooldown; //increases the time between jumps
    public float airMultiplier; //increases or decreases the speed that the player moves only in the air
    bool readyToJump; //tell the system when the jump cooldown has finished allowing the player to jump again

    [HideInInspector] public float walkSpeed;
    [HideInInspector] public float sprintSpeed;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    public float playerHeight; 
    public LayerMask whatIsGround;
    bool grounded;

    [Header("Crouch")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    public MovementState state;

    public float gravityMultiplier = 0.5f;

    public float jumpForwardForce;

    public enum MovementState
    {
        crouching,
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;

        startYScale = transform.localScale.y;
    }

    private void Update()
    {
        // ground check
        //grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);

        Time.timeScale = moveSpeed;

        transform.Translate(Vector3.forward * Time.deltaTime);

        MyInput();
        SpeedControl();

        // handle drag
        if (grounded)
        {
            rb.linearDamping = groundDrag;
            Debug.Log("Is grounded");
        }
        else
        {
            rb.linearDamping = 0;
        }

        Debug.Log("Current Speed: " + moveSpeed);
    }

    public void OnCollisionStay(Collision collision)
    {
        grounded = true;
    }

    public void OnCollisionExit(Collision collision)
    {
        grounded = false;
    }
    private void FixedUpdate()
    {
        MovePlayer();
        // Apply custom gravity
        if (!grounded)
        {
            rb.AddForce(Physics.gravity * (gravityMultiplier - 1f), ForceMode.Acceleration);
        }
    }

    private void StateHandler()
    {
        // Mode crouching
        if (Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
        }
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        

        // when to jump
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            Debug.Log("Jump has been pressed");
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // start crouch
        if (Input.GetKeyDown(crouchKey))
        {
            Debug.Log("Has crouched");
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        // stop crouch
        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }

    private void MovePlayer()
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // on ground
        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * 20f, ForceMode.Acceleration);
        }
        // in air
        else if (!grounded)
        {
            //rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
            Vector3 airMove = moveDirection.normalized * moveSpeed * airMultiplier;
            rb.AddForce(airMove, ForceMode.Acceleration);
        }


        if (Input.GetKeyDown(KeyCode.H))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        // limit velocity if needed
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        Debug.Log("Has jumped");

        // Reset vertical velocity only
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        // Apply force
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

        // Apply a leap
        Vector3 forwardForce = orientation.forward * jumpForwardForce * 0.2f;
        rb.AddForce(forwardForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;
    }


}

