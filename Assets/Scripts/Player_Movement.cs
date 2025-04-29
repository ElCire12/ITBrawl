//using System.Xml.Serialization;
//using UnityEngine;
//using UnityEngine.InputSystem;
//using static Player;

//public class Player : MonoBehaviour
//{
//    public enum PlayerStates
//    {
//        Idle,
//        Walking,
//        Jumping,
//        AirAttacking,
//    }

//    public PlayerStates currentState = PlayerStates.Idle;
//    public Vector2 moveInput;

//    public Rigidbody rigidbody;
//    public PlayerInput playerInput_Object;
//    public float jump_force;
//    public float movement_speed;
//    public Vector2 playerInput;
//    public Animator animator;
//    public Transform playerVisuals;
//    PlayerInputActions playerInputActions;

//    [SerializeField]
//    bool isGrounded;

//    private void Awake()
//    {
//        playerInputActions = new PlayerInputActions();
//    }
//    void Start()
//    {

//    }

//    private void OnEnable()
//    {
//        //playerInputActions.PlayerControl.Jump.started += ctx => Jump();
//        playerInputActions.PlayerControl.Enable(); 
//    }

//    private void OnDisable()
//    {
//        playerInputActions.PlayerControl.Disable();
//    }

//    void Update()
//    {
//        moveInput = playerInputActions.PlayerControl.Move.ReadValue<Vector2>();
//        CheckChangeState();
        

//    }

//    public void CheckChangeState()
//    {
//        if (moveInput.x > 0 && isGrounded)
//        {
//            ChangeState(PlayerStates.Walking);
//        }
//        else if (isGrounded)
//        {
//            ChangeState(PlayerStates.Idle);
//        }
//    }

//    public void ChangeState(PlayerStates newState)
//    {
//        if (currentState != newState) {
//            currentState = newState;
//        }
        
//    }


//    private void OnCollisionEnter(Collision collision)
//    {
//        if (collision.gameObject.tag == "ground")
//        {
//            isGrounded = true;
//        }
//    }

//    private void OnCollisionExit(Collision collision)
//    {
//        if (collision.gameObject.tag == "ground")
//        {
//            isGrounded = false;
//        }
//    }

//    //rigidbody.AddForce(Vector3.up* jump_force, ForceMode.Impulse);
//}
