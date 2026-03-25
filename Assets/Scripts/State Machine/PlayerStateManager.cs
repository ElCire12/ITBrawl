using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.UI.Image;
using UnityEngine.Timeline;
using System.Collections;

public class PlayerStateManager : MonoBehaviour
{
    private PlayerState currentState;
    private Dictionary<string, PlayerState> states;

    public PlayerInput playerInput;
    private InputAction movementAction;
    private InputAction specialAttackAction;
    private InputAction basicAttackAction;
    private InputAction jumpAction;

    public PlayerInfo playerInfo;

    public TextMeshProUGUI playerNumberText;

    [Header("PlayerState")]
    public string playerstate = "";
    public string previousState = "";
    public GameObject playerWinPrefab;

    [Header("Input")]
    public Vector2 movementInput;
    public bool specialAttackPressed;
    public bool basicAttackPressed; 
    public bool jumpStarted; 

    [Header("Movement")]
    public float flipActionPoint; 
    public bool isGrounded;
    public float acceleration;
    public float deceleration;
    public float maxSpeed;

    [Header("Ground Detection")]
    public float groundCheckDistance = 0.2f;
    public LayerMask groundMask;

    [Header("Air Movement")]
    public int maxAirJumps;
    public float jumpForce = 10f;
    public float airAcceleration;
    public float airMaxSpeed = 5f;
    public float extraFallGravity = 20f;

    [Header("Attacks")]
    public bool isAttacking = false;
    public CharacterAttack characterAttacks; 

    [Header("Animator")]
    public Animator animator;

    public Rigidbody rb;
    public Transform visuals;

    [Header("Sounds")]
    public AudioClip jumpSound;
    public AudioClip[] hittedSounds;
    public AudioClip[] jumpSounds;

    public bool canAttack = true; 
    public bool canJump = true;


    private Vector2 initialPosition;
    public bool isStunned = false;
    PlayerLive playerLiveScript;

    void Awake()
    {
        characterAttacks = GetComponent<CharacterAttack>();
        // Inicializa el PlayerInput y las acciones
        playerInput = GetComponent<PlayerInput>();
        movementAction = playerInput.actions["Move"];
        specialAttackAction = playerInput.actions["SpecialAttack"];
        basicAttackAction = playerInput.actions["BasicAttack"];
        jumpAction = playerInput.actions["Jump"];
        playerLiveScript = gameObject.GetComponent<PlayerLive>();
    }

    void OnEnable()
    {
        playerInput.actions.Enable();
        //jumpAction.Enable();
        //jumpAction.started += ctx => {
        //    jumpStarted = true;
        //};
    }

    private void OnDisable()
    {
        playerInput.actions.Disable();
    }

    void Start()
    {
        Application.targetFrameRate = 60;
        initialPosition = transform.position;

        states = new Dictionary<string, PlayerState> {
            { "Idle", new IdleState(this) },
            { "Walking", new WalkingState(this) },
            { "Jump", new JumpState(this)},
            { "Air", new AirState(this)},
            { "SpecialAttackingState", new SpecialAttackingState(this)},
            {"BasicAttackingState", new  BasicAttackingState(this)}
        };

        currentState = new IdleState(this);
    }

    void Update()
    {
        if (currentState == null) return;

        playerstate = currentState.ToString();

        animator.SetBool("is_grounded", isGrounded);

        //MAIN PROGRAM
        ReadPlayerInput();

        ReadPlayerContext(); 

        DecideNextState();

        currentState.Update();

        DeleteInputs();
    }

    private void FixedUpdate()
    {
        if (transform.position.y < -30f)
        {
            playerLiveScript.TakeDamage(75, 0);
            rb.velocity = Vector2.zero;
            rb.position = initialPosition;
        }
        currentState.FixedUpdate();
    }

    void DecideNextState()
    {
        if (isAttacking) return;
        if (isStunned) return;

        if (!isGrounded)
        {
            ChangeState("Air");

            if (specialAttackPressed && movementInput.y > 0.5f && previousState != "SpecialAttackingState" && canAttack)
            { 
                ChangeState("SpecialAttackingState");
            }
        }

        else if (isGrounded)
        {
            if (specialAttackPressed && canAttack)
            {
                ChangeState("SpecialAttackingState");
            }
            else if (basicAttackPressed && canAttack)
            {
                ChangeState("BasicAttackingState");
            }
            else if (jumpStarted && canJump)
            {
                ChangeState("Jump");
            }
            else if (Mathf.Abs(movementInput.x) > 0.35f)
            {
                ChangeState("Walking");
            }
            else
            {
                ChangeState("Idle");
            }
        }
    }

    public void ChangeState(string newStateName)
    {
        if (currentState != null && states[newStateName] == currentState) return;

        previousState = currentState.ToString();

        currentState?.Exit();
        currentState = states[newStateName];
        currentState.Enter();
    }

    void ReadPlayerInput()
    {
        movementInput = movementAction.ReadValue<Vector2>();
        specialAttackPressed = specialAttackAction.IsPressed();
        basicAttackPressed = basicAttackAction.IsPressed();

        jumpStarted = jumpAction.WasPressedThisFrame(); 
    }

    void ReadPlayerContext()
    {
        // DETECT IF IS GROUNDED
        float raysSpacing = 0.5f;
        bool centerRay  = Physics.Raycast(new Ray(transform.position, Vector3.down), groundCheckDistance, groundMask);
        bool frontRay = Physics.Raycast(new Ray(new Vector3 (transform.position.x + raysSpacing, transform.position.y, transform.position.z), Vector3.down), groundCheckDistance, groundMask);
        bool backRay = Physics.Raycast(new Ray(new Vector3(transform.position.x - raysSpacing, transform.position.y, transform.position.z), Vector3.down), groundCheckDistance, groundMask);
        isGrounded = centerRay || frontRay || backRay;

        Debug.DrawRay(transform.position, Vector3.down * groundCheckDistance, centerRay ? Color.green : Color.red);
        Debug.DrawRay(new Vector3(transform.position.x + raysSpacing, transform.position.y, transform.position.z), Vector3.down * groundCheckDistance, frontRay ? Color.green : Color.red);
        Debug.DrawRay(new Vector3(transform.position.x - raysSpacing, transform.position.y, transform.position.z), Vector3.down * groundCheckDistance, backRay ? Color.green : Color.red);
    }

    void DeleteInputs()
    {
        //jumpStarted = false;
    }

    public int GetActualPlayerDirection()
    {
        float playerRotation = visuals.transform.rotation.eulerAngles.y;
        if (playerRotation == 90)
        {
            return 1;
        }
        else if (playerRotation == 270)
        {
            return -1;
        }
        else
        {
            Debug.LogError("No se esta detectando bien la dirección del jugador");
            return 0;
        }
    }

    public void ApplyStun(float duration)
    {
        if (duration == 0) return;

        currentState?.Exit();
        currentState = new StunnedState(this, duration);
        currentState.Enter();
    }
}
