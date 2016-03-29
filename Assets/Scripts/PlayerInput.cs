using UnityEngine;
using System.Collections;
using InControl;

//public class AztlanPlayerActions : PlayerActionSet
//{
//    public PlayerAction Punch;
//    public PlayerAction Kick;
//    public PlayerAction Block;
//    public PlayerAction Pray;
//    public PlayerAction Jump;
//    public PlayerAction Left;
//    public PlayerAction Right;
//    public PlayerAction Up;
//    public PlayerAction Down;
//    public PlayerOneAxisAction Move;


//    public AztlanPlayerActions()
//    {
//        Punch = CreatePlayerAction( "Punch" );
//        Kick = CreatePlayerAction("Kick");
//        Jump = CreatePlayerAction( "Jump" );
//        Block = CreatePlayerAction( "Block" );
//        Pray = CreatePlayerAction("Pray");
//        Left = CreatePlayerAction( "Move Left" );
//        Right = CreatePlayerAction( "Move Right" );
//        Up = CreatePlayerAction( "Look Up" );
//        Down = CreatePlayerAction( "Crouch" );
//        //Move = CreateOneAxisPlayerAction( Left, Right );
//    }
//}


public class PlayerInput : MonoBehaviour {

    public AztlanPlayerActions playerActions;

    private Player player;
    private PlayerMotor motor;
    private PlayerAttack attack;


    public float actionHorizontalMove = 0f;
    public float actionVerticalMove = 0f;
    public bool actionJump = false;
    public bool actionJumping = false;
    public bool actionPunch = false;
    public bool actionKick = false;

    public bool actionBlock = false;
    public bool actionPray = false;

	// Use this for initialization
	void Start () {
        player = GetComponent<Player>();
        motor = player.motor;
        attack = GetComponent<PlayerAttack>();
	}

    public void SetUpController(InputDevice inputDevice)
    {

        playerActions = new AztlanPlayerActions();
        playerActions.Device = inputDevice;

        playerActions.Left.AddDefaultBinding(Key.LeftArrow);
        playerActions.Left.AddDefaultBinding(Key.A);
        playerActions.Left.AddDefaultBinding(InputControlType.LeftStickLeft);

        playerActions.Right.AddDefaultBinding(Key.RightArrow);
        playerActions.Right.AddDefaultBinding(Key.D);
        playerActions.Right.AddDefaultBinding(InputControlType.LeftStickRight);

        playerActions.Up.AddDefaultBinding(Key.UpArrow);
        playerActions.Up.AddDefaultBinding(Key.W);
        playerActions.Up.AddDefaultBinding(InputControlType.LeftStickUp);

        playerActions.Down.AddDefaultBinding(Key.DownArrow);
        playerActions.Down.AddDefaultBinding(Key.S);
        playerActions.Down.AddDefaultBinding(InputControlType.LeftStickDown);

        playerActions.Jump.AddDefaultBinding(Key.Space);
        playerActions.Jump.AddDefaultBinding(InputControlType.Action1);

        playerActions.Pray.AddDefaultBinding(InputControlType.RightTrigger);

        playerActions.Block.AddDefaultBinding(InputControlType.RightBumper);

        playerActions.Punch.AddDefaultBinding(InputControlType.Action3);
        playerActions.Kick.AddDefaultBinding(InputControlType.Action4);

        playerActions.ListenOptions.MaxAllowedBindings = 3;

    }


    void FixedUpdate()
    {
        if (playerActions.Device == null) return;

        actionHorizontalMove   = playerActions.Right.Value - playerActions.Left.Value;
        actionVerticalMove     = playerActions.Up.Value - playerActions.Down.Value;
        actionJump             = playerActions.Jump.WasPressed;
        actionJumping          = playerActions.Jump.IsPressed;
        actionPunch            = playerActions.Punch.WasPressed;
        actionKick             = playerActions.Kick.WasPressed;

        actionBlock            = playerActions.Block.IsPressed;
        actionPray             = playerActions.Pray.IsPressed;

        //if (actionJump)
        //    motor.StartJump();
    }

	

    //private bool isAnimationState(string state)
    //{
    //    return animator.GetCurrentAnimatorStateInfo(0).IsName(state);
    //}


}
