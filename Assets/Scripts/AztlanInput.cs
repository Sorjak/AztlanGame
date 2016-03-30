using UnityEngine;
#if !UNITY_4_6 && !UNITY_5_1 && !UNITY_5_2
using UnityEngine.SceneManagement;
#endif
using System.Collections;
using System.IO;
using InControl;

using PlatformerPro;

public class AztlanPlayerActions : PlayerActionSet
{
    public PlayerAction Punch;
    public PlayerAction Kick;
    public PlayerAction Block;
    public PlayerAction Pray;
    public PlayerAction Jump;
    public PlayerAction Left;
    public PlayerAction Right;
    public PlayerAction Up;
    public PlayerAction Down;
    public PlayerAction Pause;
    public PlayerOneAxisAction MoveHorizontal;
    public PlayerOneAxisAction MoveVertical;


    public AztlanPlayerActions()
    {
        Punch = CreatePlayerAction("Punch");
        Kick = CreatePlayerAction("Kick");
        Jump = CreatePlayerAction("Jump");
        Block = CreatePlayerAction("Block");
        Pray = CreatePlayerAction("Pray");
        Left = CreatePlayerAction("Move Left");
        Right = CreatePlayerAction("Move Right");
        Up = CreatePlayerAction("Look Up");
        Down = CreatePlayerAction("Crouch");
        Pause = CreatePlayerAction("Pause");
        MoveHorizontal = CreateOneAxisPlayerAction( Left, Right );
        MoveVertical = CreateOneAxisPlayerAction(Up, Down);
    }
}

public class AztlanInput : PlatformerPro.Input
{
    public AztlanPlayerActions playerActions;

    /// <summary>
    /// Stores the last state of the vertical axis (digital).
    /// </summary>
    protected int lastDigitalVerticalAxisState;

    /// <summary>
    /// Stores the last state of the horizontal axis (digital).
    /// </summary>
    protected int lastDigitalHorizontalAxisState;

    // Use this for initialization
    void Start()
    {
        var numDevices = InputManager.Devices.Count;

        for (int i = 0; i < numDevices; i++)
        {
            InputDevice device = InputManager.Devices[i];
            if (device != null)
            {
                SetUpController(device);
            }

        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void LastUpdate()
    {
        if (PauseButton == ButtonState.DOWN)
        {
            TimeManager.Instance.TogglePause(false);
        }

        lastDigitalHorizontalAxisState = HorizontalAxisDigital;
        lastDigitalVerticalAxisState = VerticalAxisDigital;
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

    public override float HorizontalAxis
    {
        get
        {
            //float axisValue = playerActions.Right.Value - playerActions.Left.Value;
            float axisValue = playerActions.MoveHorizontal.Value;
            if (axisValue > 0 || axisValue < 0) 
                return  axisValue;
            else
                return 0.0f;
        }
    }

    public override int HorizontalAxisDigital
    {
        get { return Mathf.Abs(HorizontalAxis) > .25f ? (int)HorizontalAxis : 0; }
    }

    public override ButtonState HorizontalAxisState
    {
        get
        {
            if (lastDigitalHorizontalAxisState <= 0 && HorizontalAxisDigital == 1) return ButtonState.DOWN;
            if (lastDigitalHorizontalAxisState >= 0 && HorizontalAxisDigital == -1) return ButtonState.DOWN;
            if (lastDigitalHorizontalAxisState != 0 && lastDigitalHorizontalAxisState == HorizontalAxisDigital) return ButtonState.HELD;
            if (lastDigitalHorizontalAxisState != 0 && HorizontalAxisDigital == 0) return ButtonState.UP;
            return ButtonState.NONE;
        }
    }

    public override float VerticalAxis
    {
        get
        {
            //float axisValue = playerActions.Up.Value - playerActions.Down.Value;
            float axisValue = -playerActions.MoveVertical.Value;
            if (axisValue > 0 || axisValue < 0)
                return axisValue;
            else
                return 0.0f;
        }
    }

    public override int VerticalAxisDigital
    {
        get { return Mathf.Abs(VerticalAxis) > .25f ? (int)VerticalAxis : 0; }
    }

    public override ButtonState VerticalAxisState
    {
        get
        {
            if (lastDigitalVerticalAxisState <= 0 && VerticalAxisDigital == 1) return ButtonState.DOWN;
            if (lastDigitalVerticalAxisState >= 0 && VerticalAxisDigital == -1) return ButtonState.DOWN;
            if (lastDigitalVerticalAxisState != 0 && lastDigitalVerticalAxisState == VerticalAxisDigital) return ButtonState.HELD;
            if (lastDigitalVerticalAxisState != 0 && VerticalAxisDigital == 0) return ButtonState.UP;
            return ButtonState.NONE;
        }
    }

    public override float AltHorizontalAxis
    {
        get { throw new System.NotImplementedException(); }
    }

    public override int AltHorizontalAxisDigital
    {
        get { throw new System.NotImplementedException(); }
    }

    public override ButtonState AltHorizontalAxisState
    {
        get { throw new System.NotImplementedException(); }
    }

    public override float AltVerticalAxis
    {
        get { throw new System.NotImplementedException(); }
    }

    public override int AltVerticalAxisDigital
    {
        get { throw new System.NotImplementedException(); }
    }

    public override ButtonState AltVerticalAxisState
    {
        get { throw new System.NotImplementedException(); }
    }

    public override ButtonState JumpButton
    {
        get { return GetStateForKey(playerActions.Jump); }
    }

    public override ButtonState RunButton
    {
        get { return ButtonState.NONE; }
    }

    public override ButtonState PauseButton
    {
        get { return GetStateForKey(playerActions.Pause); }
    }

    public override ButtonState ActionButton
    {
        get { return GetStateForKey(playerActions.Punch); }
    }

    public override bool AnyKey
    {
        get { return UnityEngine.Input.anyKeyDown; }
    }

    public override ButtonState GetActionButtonState(int buttonIndex)
    {
        return ActionButton;
    }

    public override bool SetAxis(KeyType type, string axis, bool reverseAxis)
    {
        throw new System.NotImplementedException();
    }

    public override bool SetKey(KeyType type, KeyCode keyCode)
    {
        throw new System.NotImplementedException();
    }

    public override bool SetKey(KeyType type, KeyCode keyCode, int keyNumber)
    {
        throw new System.NotImplementedException();
    }

    public override KeyCode GetKeyForType(KeyType type, int keyNumber)
    {
        //if (type == KeyType.UP) return playerActions.Up.Bindings[0].;
        //if (type == KeyType.DOWN) return down;
        //if (type == KeyType.LEFT) return left;
        //if (type == KeyType.RIGHT) return right;
        //if (type == KeyType.JUMP) return jump;
        //if (type == KeyType.RUN) return run;
        //if (type == KeyType.PAUSE) return pause;
        //if (type == KeyType.ACTION && keyNumber >= 0 && keyNumber < actionButtons.Length) return actionButtons[keyNumber];
        return KeyCode.None;
    }

    public override string GetAxisForType(KeyType type)
    {
        if (type == KeyType.VERTICAL_AXIS) return playerActions.MoveHorizontal.ToString();
        if (type == KeyType.HORIZONTAL_AXIS) return playerActions.MoveVertical.ToString();
        return "None";
    }

    public override bool SaveInputData()
    {
        throw new System.NotImplementedException();
    }

    public override bool LoadInputData(string dataName)
    {
        throw new System.NotImplementedException();
    }

    public override void LoadInputData(StandardInputData data)
    {
        throw new System.NotImplementedException();
    }

    virtual protected ButtonState GetStateForKey(PlayerAction key)
    {
        if (key.WasPressed) return ButtonState.DOWN;
        if (key.WasReleased) return ButtonState.UP;
        if (key.IsPressed) return ButtonState.HELD;
        return ButtonState.NONE;
    }
}
