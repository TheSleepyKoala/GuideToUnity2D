using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Animations;

namespace HumbleGCO.CollidersTutorial
{
    //Modified for this Tutorial - See original code here https://github.com/atmosgames/SuperSimple2DKit
    [RequireComponent(typeof(Rigidbody2D))]
    public class Snail : PhysicsObject
    {
        [Header ("Reference")]
        [SerializeField] private Animator animator;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private GameObject graphic;

        [Header("Properties")]
        private float fallForgivenessCounter; //Counts how long the player has fallen off a ledge
        [SerializeField] private float fallForgiveness = .2f; //How long the player can fall from a ledge and still jump
        [System.NonSerialized] public RaycastHit2D ground; 
        [SerializeField] private bool noCursor;
        [SerializeField] private bool jumping;
        private float launch; //The float added to x and y moveSpeed. This is set with hurtLaunchPower, and is always brought back to zero
        [SerializeField] private float launchRecovery; //How slow should recovering from the launch be? (Higher the number, the longer the launch will last)
        public float maxSpeed = 7; //Max move speed
        public float jumpPower = 17;
        private Vector3 origLocalScale;
        
        private Controls controls;
        private InputAction moveAction;

        // Singleton instantiation
        private static Snail instance;
        public static Snail Instance
        {
            get
            {
                if (instance == null) instance = GameObject.FindObjectOfType<Snail>();
                return instance;
            }
        }

        private void Awake() => controls = new Controls();

        private void OnEnable()
        {
            controls.Enable();
            controls.Player.Jump.performed += _ => Jump(1f);
        }
        
        private void OnDisable()
        {
            controls.Disable();
            controls.Player.Jump.performed -= _ => Jump(1f);
        }

        private void Start()
        {
            if(noCursor)
                Cursor.visible = false;
        }

        private void Update() => ComputeVelocity();

        protected void ComputeVelocity()
        {
            //Player movement & attack
            Vector2 move = Vector2.zero;
            ground = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), -Vector2.up);

            //Lerp launch back to zero at all times
            launch += (0 - launch) * Time.deltaTime * launchRecovery;

            move.x = controls.Player.Direction.ReadValue<Vector2>().x + launch;

            //Flip the graphic's localScale
            if (move.x > 0.01f)
                spriteRenderer.flipX = true;
            else if (move.x < -0.01f)
                spriteRenderer.flipX = false;     
                
            //Allow the player to jump even if they have just fallen off an edge ("fall forgiveness")
            if (!grounded)
            {
                if (fallForgivenessCounter < fallForgiveness && !jumping)
                {
                    fallForgivenessCounter += Time.deltaTime;
                }
                else
                {
                    animator.SetBool("grounded", false);
                }
            }
            else
            {
                fallForgivenessCounter = 0;
                animator.SetBool("grounded", true);
            }

            animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);
            animator.SetFloat("velocityY", velocity.y);
            animator.SetInteger("moveDirection", (int)controls.Player.Direction.ReadValue<Vector2>().x);
            targetVelocity = move * maxSpeed;
        }

        public void Jump(float jumpMultiplier)
        {
            if (animator.GetBool("grounded") == true)
            {
                if (velocity.y != jumpPower)
                {
                    velocity.y = jumpPower * jumpMultiplier; //The jumpMultiplier allows us to use the Jump function to also launch the player from bounce platforms
                    jumping = true;
                }
            }
        }

        public void Cook()
        {
            animator.SetTrigger("cook");
            Debug.Log("You cook the snail!");
        }
    }
}
