using DataKeeper.Attributes;
using DataKeeper.Generic;
using UnityEngine;

namespace DataKeeper.FSM
{
    /// <summary>
    /// Example implementation for a character controller with transition system
    /// </summary>
    public class CharacterFSM : MonoBehaviour
    {
        public Reactive<CharacterState> StateReactive = new Reactive<CharacterState>(); 
        
        private StateMachine<CharacterState, CharacterFSM> stateMachine;
        public Rigidbody2D rb; // Assuming 2D character

        public float walkSpeed = 5f;
        public float runSpeed = 10f;
        public float jumpForce = 10f;

        public enum CharacterState
        {
            Idle,
            Walking,
            Running,
            Jumping,
            Falling
        }

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            SetupStateMachine();
        }

        private void SetupStateMachine()
        {
            stateMachine = new StateMachine<CharacterState, CharacterFSM>(this);

            // Add states
            stateMachine.AddState(CharacterState.Idle, new IdleState());
            stateMachine.AddState(CharacterState.Walking, new WalkingState());
            stateMachine.AddState(CharacterState.Running, new RunningState());
            stateMachine.AddState(CharacterState.Jumping, new JumpingState());
            stateMachine.AddState(CharacterState.Falling, new FallingState());

            // Add transitions
            // Idle -> Walking
            stateMachine.AddTransition(
                CharacterState.Idle,
                CharacterState.Walking,
                () => Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.1f && !Input.GetKey(KeyCode.LeftShift))
                .OnTransition(() => Debug.Log("Transitioning: Idle -> Walking"));

            // Walking -> Running
            stateMachine.AddTransition(
                CharacterState.Walking,
                CharacterState.Running,
                () => Input.GetKey(KeyCode.LeftShift))
                .OnTransition(() => Debug.Log("Transitioning: Walking -> Running"));

            // Running -> Walking
            stateMachine.AddTransition(
                CharacterState.Running,
                CharacterState.Walking,
                () => !Input.GetKey(KeyCode.LeftShift))
                .OnTransition(() => Debug.Log("Transitioning: Running -> Walking"));

            // Moving -> Idle
            stateMachine.AddTransition(
                CharacterState.Walking,
                CharacterState.Idle,
                () => Mathf.Abs(Input.GetAxisRaw("Horizontal")) < 0.1f)
                .OnTransition(() => Debug.Log("Transitioning: Walking -> Idle"));

            // Jump transition from any state
            stateMachine.AddAnyStateTransition(
                CharacterState.Jumping,
                () => Input.GetKeyDown(KeyCode.Space) && IsGrounded())
                .Cooldown(5)
                .OnTransition(() => Debug.Log("Transitioning to Jump"));

            // Falling transition from any state
            stateMachine.AddAnyStateTransition(
                CharacterState.Falling,
                () => !IsGrounded() && rb.velocity.y < -0.1f && stateMachine.CurrentStateType != CharacterState.Jumping)
                .OnTransition(() => Debug.Log("Transitioning to Falling"));

            stateMachine.SetInitialState(CharacterState.Idle);
            StateReactive.Value = stateMachine.CurrentStateType;
            StateReactive.AddListener(stateMachine.ChangeState);
        }
        
        [Button]
        private void Jumping()
        {
            StateReactive.Value = CharacterState.Jumping;
        }

        public bool IsGrounded()
        {
            // Implement your ground check logic here
            return Physics2D.Raycast(transform.position, Vector2.down, 0.1f);
        }

        private void Update()
        {
            stateMachine.Update();
        }

        private void FixedUpdate()
        {
            stateMachine.FixedUpdate();
        }
    }

    public class IdleState : State<CharacterFSM.CharacterState, CharacterFSM>
    {
        public override void OnEnter()
        {
            Debug.Log("Entering Idle State");
        }
    }

    public class WalkingState : State<CharacterFSM.CharacterState, CharacterFSM>
    {
        public override void OnUpdate()
        {
            float moveInput = Input.GetAxisRaw("Horizontal");
            Vector2 movement = new Vector2(moveInput * stateMachine.Target.walkSpeed, stateMachine.Target.rb.velocity.y);
            stateMachine.Target.rb.velocity = movement;
        }
    }

    public class RunningState : State<CharacterFSM.CharacterState, CharacterFSM>
    {
        public override void OnUpdate()
        {
            float moveInput = Input.GetAxisRaw("Horizontal");
            Vector2 movement = new Vector2(moveInput * stateMachine.Target.runSpeed, stateMachine.Target.rb.velocity.y);
            stateMachine.Target.rb.velocity = movement;
        }
    }

    public class JumpingState : State<CharacterFSM.CharacterState, CharacterFSM>
    {

        public override void OnEnter()
        {
            Debug.Log("Entering Jumping State");
            stateMachine.Target.rb.AddForce(Vector2.up * stateMachine.Target.jumpForce, ForceMode2D.Impulse);
        }

        public override void OnUpdate()
        {
            // Handle horizontal movement while jumping
            float moveInput = Input.GetAxisRaw("Horizontal");
            Vector2 movement = new Vector2(moveInput * stateMachine.Target.walkSpeed, stateMachine.Target.rb.velocity.y);
            stateMachine.Target.rb.velocity = new Vector2(movement.x, stateMachine.Target.rb.velocity.y);

            // Transition to falling if velocity is negative
            if (stateMachine.Target.rb.velocity.y < -0.1f)
            {
                stateMachine.ChangeState(CharacterFSM.CharacterState.Falling);
            }
        }
    }

    public class FallingState : State<CharacterFSM.CharacterState, CharacterFSM>
    {
        public override void OnEnter()
        {
            Debug.Log("Entering Falling State");
        }

        public override void OnUpdate()
        {
            // Handle horizontal movement while falling
            float moveInput = Input.GetAxisRaw("Horizontal");
            Vector2 movement = new Vector2(moveInput * stateMachine.Target.walkSpeed, stateMachine.Target.rb.velocity.y);
            stateMachine.Target.rb.velocity = new Vector2(movement.x, stateMachine.Target.rb.velocity.y);

            // Check for landing
            if (stateMachine.Target.IsGrounded())
            {
                if (Mathf.Abs(moveInput) > 0.1f)
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                        stateMachine.ChangeState(CharacterFSM.CharacterState.Running);
                    else
                        stateMachine.ChangeState(CharacterFSM.CharacterState.Walking);
                }
                else
                {
                    stateMachine.ChangeState(CharacterFSM.CharacterState.Idle);
                }
            }
        }
    }
}