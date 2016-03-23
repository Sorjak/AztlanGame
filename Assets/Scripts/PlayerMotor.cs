using UnityEngine;
using System.Collections;


[System.Serializable]
public class PlayerMotor {

    public class PlayerMotorStateMachine : StateMachine
    {
        public State Idle = new State("Idle");

        public State Running = new State("Running");
        public State Sliding = new State("Sliding");

        public State Jumping = new State("Jumping");
        public State Falling = new State("Falling");
        public State Landing = new State("Landing");

        public State MovingToHang = new State("MovingToHang");
        public State Hanging = new State("Hanging");
        public State Climbing = new State("Climbing");


    }

    public PlayerMotorStateMachine sm;

    private Player player;

    [System.Serializable]
    public class Acceleration
    {
        public float horizontal = 12f;
        public float stop = 50f;
        public float air = 8f;
    }

    public Acceleration acc;

    //public float maxSpeed = 25f;
    public float runSpeedMultiplier = 10f;
    public float airSpeedMultiplier = 7f;

    public float jumpSpeed = 5f;
    public float jumpMinDuration = .1f;
    public float jumpMaxDuration = .4f;
    public float landDuration = .2f;
    public float climbDuration = .5f;
    public float hangDelay = .3f;

    public float faceOffset = .3f;

    public Vector2 realVelocity;
    public Vector2 attachPosition;

    private bool  facingRight = false;

    private bool candoublejump;
    private float jumpTimer;
    private float landingTimer;
    private float hangDelayTimer;

    public bool on_ground = false;
    public bool on_ledge = false;

    public bool climbing = false;
    public bool sliding = false;

    public bool startJump = false;

    public bool isOnGround { get { return on_ground; } }
    public bool isOnLedge { get { return on_ledge; } }
    public bool isClimbing { get { return climbing; } }

    public State getCurrentState { get { return sm.CurrentState(); } }

    // Use this for initialization
    public PlayerMotor(Player playerIn)
    {
        player = playerIn;
        jumpTimer = Time.realtimeSinceStartup;

        acc = new Acceleration();

        InitStateMachine();
    }

    #region Public Methods

    public void ProcessMotion()
    {
        // reset flags

        on_ground = false;
        on_ledge = false;

        //climbing = false;
        sliding = false;

        if (player.triggerState > 0)
            HandleTriggers();

        sm.Update();
        
    }



    #endregion

    #region State Methods

    private void InitStateMachine()
    {
        sm = new PlayerMotorStateMachine();

        sm.Enter(sm.Idle);

        sm.Idle.OnEnter += () => { player.rigidbody.velocity = Vector2.zero; };
        sm.Idle.OnUpdate += () => { player.animator.Play("idle"); };
        sm.Idle.OnExit += () => { };

        //idle transitions
        sm.Idle.ChangeTo(sm.Running).If(() => Mathf.Abs(player.input.actionHorizontalMove) > 0);
        sm.Idle.ChangeTo(sm.Falling).If(() => !on_ground);
        sm.Idle.ChangeTo(sm.Jumping).If(() => player.input.actionJump);
        sm.Idle.ChangeTo(sm.Sliding).If(() => sliding);

        sm.Running.OnEnter += () => { };
        sm.Running.OnUpdate += Run;
        sm.Running.OnExit += () => { };

        //run transitions
        sm.Running.ChangeTo(sm.Idle).If(() => Mathf.Abs(player.input.actionHorizontalMove) <= 0);
        sm.Running.ChangeTo(sm.Falling).If(() => !on_ground);
        sm.Running.ChangeTo(sm.Jumping).If(() => player.input.actionJump);

        sm.Sliding.OnUpdate += Slide;

        //slide transitions
        sm.Sliding.ChangeTo(sm.Falling).If(() => !sliding && !on_ground);
        sm.Sliding.ChangeTo(sm.Idle).If(() => !sliding && on_ground);

        sm.Jumping.OnEnter += EnterJump;
        sm.Jumping.OnUpdate += Jump;
        sm.Jumping.OnExit += () => { };

        //jump transition
        sm.Jumping.ChangeTo(sm.Falling).If(() =>
        {
            return (!player.input.actionJumping && jumpTimer >= jumpMinDuration) ||
                    (player.input.actionJumping && jumpTimer >= jumpMaxDuration);
        });

        sm.Falling.OnEnter += () => { };
        sm.Falling.OnUpdate += Fall;
        sm.Falling.OnExit += () => { };

        //fall transitions
        sm.Falling.ChangeTo(sm.Landing).If(() => on_ground);
        sm.Falling.ChangeTo(sm.Sliding).If(() => sliding);
        sm.Falling.ChangeTo(sm.MovingToHang).If(() => on_ledge && hangDelayTimer >= hangDelay);

        sm.Landing.OnEnter += () => { player.animator.Play("Land"); landingTimer = 0; };
        sm.Landing.OnUpdate += Land;
        sm.Landing.OnExit += () => { };

        //land transitions
        sm.Landing.ChangeTo(sm.Running).If(() => Mathf.Abs(player.input.actionHorizontalMove) > 0);
        sm.Landing.ChangeTo(sm.Jumping).If(() => player.input.actionJump);
        sm.Landing.ChangeTo(sm.Idle).If(() => landingTimer > landDuration);

        sm.MovingToHang.OnEnter += FindAttachPoint;
        sm.MovingToHang.OnUpdate += MoveToAttachPoint;
        sm.MovingToHang.OnExit += () => { };

        sm.MovingToHang.ChangeTo(sm.Hanging).If(() => Vector2.Distance(player.transform.position, attachPosition) < .001f);

        sm.Hanging.OnEnter += () => { player.rigidbody.isKinematic = false; };
        sm.Hanging.OnUpdate += HangLedge;
        sm.Hanging.OnExit += () => { hangDelayTimer = 0; };

        //hanging transitions
        sm.Hanging.ChangeTo(sm.Falling).If(
            () => {
                bool check = false;
                if (facingRight)
                    check |= player.input.actionHorizontalMove < 0;
                else
                    check |= player.input.actionHorizontalMove > 0;

                return check || player.input.actionVerticalMove < 0; 
            }
        );

        sm.Hanging.ChangeTo(sm.Climbing).If(() => player.input.actionJump);

        sm.Climbing.OnEnter += ClimbLedge;

        sm.Climbing.ChangeTo(sm.Idle).If(() => climbing == false); 
    }

    private void Slide()
    {
        player.animator.Play("idle");

        Vector2 goalVelocity = SetGoalVelocity() * airSpeedMultiplier;
        Vector2 moveVector = new Vector2
        {
            x = 0,
            y = player.rigidbody.velocity.y + (Physics2D.gravity * Time.deltaTime).y
        };

        if (goalVelocity.sqrMagnitude > 0)
        {
            float horizMoveAmount = Mathf.Lerp(player.rigidbody.velocity.x, goalVelocity.x, acc.air * Time.deltaTime);

            moveVector.x = Mathf.Min(horizMoveAmount, goalVelocity.x);
        }
        else
        {
            float direction = player.collisions.Count > 0 ? Mathf.Sign(player.collisions[0].normal.x) : 0f;

            moveVector.x = player.rigidbody.velocity.x + (direction * 10f * Time.deltaTime);
        }

        player.rigidbody.velocity = moveVector;

    }

    private void Run()
    {
        player.animator.Play("Run");

        Vector2 goalVelocity = SetGoalVelocity() * runSpeedMultiplier;


        bool slowing = goalVelocity.sqrMagnitude < realVelocity.sqrMagnitude;
        float acceleration = slowing ? acc.stop : acc.horizontal;

        float moveAmount = Mathf.Lerp(player.rigidbody.velocity.x, goalVelocity.x, acc.horizontal * Time.deltaTime);

        Vector2 moveVector = Vector2.right * moveAmount;

        player.rigidbody.velocity = moveVector;

        realVelocity = player.rigidbody.velocity;
        
    }

    private void EnterJump()
    {
        jumpTimer = 0;
        player.rigidbody.velocity.Set(player.rigidbody.velocity.x, 0);
    }

    private void Jump()
    {
        player.animator.Play("Fall");
        jumpTimer += Time.deltaTime;

        Vector2 goalVelocity = SetGoalVelocity() * airSpeedMultiplier;

        float horizMoveAmount = Mathf.Lerp(player.rigidbody.velocity.x, goalVelocity.x, acc.air * Time.deltaTime);

        Vector2 moveVector = new Vector2
        {
            x = Mathf.Min(horizMoveAmount, goalVelocity.x),
            y = (Vector2.up * jumpSpeed).y + (Physics2D.gravity * Time.deltaTime).y
        };

        player.rigidbody.velocity = moveVector;

    }

    private void Fall()
    {
        player.animator.Play("Fall");
        hangDelayTimer += Time.deltaTime;
        Vector2 goalVelocity = SetGoalVelocity() * airSpeedMultiplier;

        float horizMoveAmount = Mathf.Lerp(player.rigidbody.velocity.x, goalVelocity.x, acc.air * Time.deltaTime);

        Vector2 moveVector = new Vector2
        {
            x = Mathf.Min(horizMoveAmount, goalVelocity.x),
            y = player.rigidbody.velocity.y + (Physics2D.gravity * Time.deltaTime).y
        };

        player.rigidbody.velocity = moveVector;
    }

    private void Land()
    {
        landingTimer += Time.deltaTime;

        player.rigidbody.velocity = Vector2.zero;
    }


    private void FindAttachPoint()
    {
        RaycastHit2D hitInfo = Physics2D.BoxCast(player.transform.position, new Vector2(.1f, .5f), 0f, -player.transform.right, .5f, 1 << 8);

        if (hitInfo.collider != null)
        {
            Collider2D col = hitInfo.collider;
            float newY = col.bounds.center.y;
            float newX = (hitInfo.point + hitInfo.normal * (player.GetComponent<Collider2D>().bounds.extents.x)).x;
            attachPosition = new Vector2(newX, newY);
            
        }
    }

    private void MoveToAttachPoint()
    {
        player.rigidbody.isKinematic = true;
        player.animator.Play("grab_ledge");
        player.rigidbody.MovePosition(attachPosition);
    }
    

    public void HangLedge()
    {
        player.animator.Play("hang_ledge");
        player.rigidbody.MovePosition(attachPosition);
    }

    public void ClimbLedge()
    {
        on_ledge = false;
        player.animator.Play("climb_ledge");
        climbing = true;

        RaycastHit2D hitInfo = Physics2D.BoxCast(player.transform.position, new Vector2(.1f, 1f), 0f, -player.transform.right, .5f, 1 << 8);

        if (hitInfo.collider != null) 
        {
            Collider2D col = hitInfo.collider;
            float top = col.transform.position.y + col.bounds.extents.y;

            Collider2D playerCol = player.GetComponent<Collider2D>();
            float playerXComponent = playerCol.bounds.extents.x;
            float playerYComponent = 0.58f;

            Vector2 newpos = hitInfo.point - hitInfo.normal * playerXComponent * 2f;
            newpos.y = top + playerYComponent;

            player.StartCoroutine(smoothClimb(newpos, climbDuration));
        }
    }

    public void DropLedge()
    {
        Debug.Log("dropping");
        on_ledge = false;
    }

    #endregion

    #region Utility Methods

    void HandleTriggers()
    {
        switch (player.triggerState)
        {
            case PlayerTriggers.CHEST_FRONT | PlayerTriggers.HEAD_FRONT:
                on_ledge = true;
                break;
            case PlayerTriggers.CHEST_FRONT | PlayerTriggers.HEAD_FRONT | PlayerTriggers.LEGS_FRONT:
                on_ledge = true;
                break;
            case PlayerTriggers.LEGS_FRONT | PlayerTriggers.LEGS_BACK:
                on_ground = true;
                break;
            case PlayerTriggers.LEGS_FRONT:
                sliding = true;
                break;
            case PlayerTriggers.LEGS_BACK:
                sliding = true;
                break;
        }
    }

    private Vector2 SetGoalVelocity()
    {
        float rawInput = player.input.actionHorizontalMove;

        if (rawInput > 0 && !facingRight)
            facingRight = !facingRight;
        else if (rawInput < 0 && facingRight)
            facingRight = !facingRight;

        player.transform.eulerAngles = new Vector3(0, facingRight ? 180 : 0, 0);

        return new Vector2(rawInput, 0f);
    }


    private IEnumerator smoothClimb(Vector2 to, float time)
    {
        float elapsedTime = 0f;

        while (elapsedTime < time)
        {
            Vector2 lerpedPos = Vector2.Lerp(player.transform.position, to, (elapsedTime / time));
            //_rigidbody2D.MovePosition(lerpedPos);
            player.transform.position = lerpedPos;
            elapsedTime += Time.deltaTime;

            Debug.DrawLine(player.transform.position, to);
            yield return new WaitForEndOfFrame();
        }

        player.transform.position = to;
        climbing = false;
        on_ground = true;
    }

    #endregion
}
