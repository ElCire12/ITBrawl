using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateManager : MonoBehaviour
{
    private PlayerState currentState;
    private Dictionary<string, PlayerState> states;

    public PlayerInput playerInput;
    private InputAction movementAction;
    private InputAction specialAttackAction;
    private InputAction jumpAction;

    [Header("PlayerState")]
    public string playerstate = "";
    public string previousState = "";

    [Header("Input")]
    public Vector2 movementInput;
    public bool specialAttackPressed;
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

    private Vector2 initialPosition;
    public bool isStunned = false;

    void Awake()
    {
        characterAttacks = GetComponent<CharacterAttack>();
        // Inicializa el PlayerInput y las acciones
        playerInput = GetComponent<PlayerInput>();
        movementAction = playerInput.actions["Move"];
        specialAttackAction = playerInput.actions["SpecialAttack"];
        jumpAction = playerInput.actions["Jump"];
    }

    void OnEnable()
    {
        playerInput.actions.Enable();
        //jumpAction.Enable();
        jumpAction.started += ctx => {
            jumpStarted = true;
        };
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
            { "SpecialAttack", new AttackingState(this)}
        };

        currentState = new IdleState(this);
    }

    void Update()
    {
        if (currentState == null) return;

        #region
        //ONLY IN DEVELOPMENT
        playerstate = currentState.ToString();

        if (Input.GetKeyDown(KeyCode.R))
        {
            transform.position = initialPosition;
            rb.velocity = Vector2.zero;
        }
        animator.SetBool("is_grounded", isGrounded);
        #endregion

        //MAIN PROGRAM
        ReadPlayerInput();

        ReadPlayerContext(); 

        DecideNextState();

        currentState.Update();

        DeleteInputs();
    }

    private void FixedUpdate()
    {
        currentState.FixedUpdate();
    }

    void DecideNextState()
    {
        if (isStunned) return;
        if (isAttacking) return;

        if (!isGrounded)
        {
            ChangeState("Air");
            if (specialAttackPressed && movementInput.y > 0.5f)
            {
                ChangeState("SpecialAttack");
            }
        }

        else if (isGrounded)
        {
            if (specialAttackPressed)
            {
                ChangeState("SpecialAttack");
            }
            else if (jumpStarted)
            {
                ChangeState("Jump");
            }
            else if ( Mathf.Abs(movementInput.x) > 0.3f)
            {
                ChangeState("Walking");
            }
            else if (rb.velocity.magnitude < 2)
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
    }

    void ReadPlayerContext()
    {
        // DETECT IF IS GROUNDED
        // Castea un rayo hacia abajo
        Ray ray = new Ray(transform.position, Vector3.down);
        isGrounded = Physics.Raycast(ray, groundCheckDistance, groundMask);

        // (Opcional) Para verlo en la escena
        Debug.DrawRay(transform.position, Vector3.down * groundCheckDistance, isGrounded ? Color.green : Color.red);
    }

    void DeleteInputs()
    {
        jumpStarted = false;
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
}
