using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct PlayerCollision
{
    public Collider2D collider;
    public Vector2 normal;
}

public enum PlayerTriggers
{
    NONE = 0,
    HEAD_FRONT = 1 << 0,
    HEAD_BACK = 1 << 1,
    CHEST_FRONT = 1 << 2,
    CHEST_BACK = 1 << 3,
    LEGS_FRONT = 1 << 4,
    LEGS_BACK = 1 << 5
}

public class Player : MonoBehaviour {

    public class PlayerStateMachine : StateMachine
    {
        public State Moving = new State();
        public State Blocking = new State();
        public State Praying = new State();
    }

    public PlayerStateMachine sm;

    public List<PlayerCollision> collisions;

    public PlayerMotor motor;
    public PlayerInput input;
    public PlayerAttack attack;

    public PlayerTriggers triggerState;
    public Rigidbody2D rigidbody;
    public Animator animator;

    public bool praying;
    public float prayerWeight = .001f;
    public float currentPrayer = 0f;

    public bool blocking;

    private Vector3 startPos;

	// Use this for initialization
	void Awake () {
        startPos = transform.position;
        triggerState = 0;

        rigidbody = rigidbody ?? GetComponent<Rigidbody2D>();
        animator = animator ?? GetComponent<Animator>();
        input = input ?? GetComponent<PlayerInput>();

        attack = GetComponent<PlayerAttack>();
        motor = new PlayerMotor(this);

        collisions = new List<PlayerCollision>();
        InitStateMachine();
	}

    private void InitStateMachine()
    {
        sm = new PlayerStateMachine();

        sm.Moving.OnUpdate += () => { if (motor != null) motor.ProcessMotion(); };

        sm.Enter(sm.Moving);
    }

    void FixedUpdate()
    {
        triggerState = PlayerTriggers.NONE;
        collisions.Clear();
    }

    void OnCollisionStay2D(Collision2D col)
    {
        int currLayer = col.collider.gameObject.layer;
        if (currLayer == LayerMask.NameToLayer("Environment"))
        {
            PlayerCollision pc = new PlayerCollision();
            pc.collider = col.collider;
            pc.normal = col.contacts[0].normal;

            collisions.Add(pc);
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (transform.position.y < -10f)
        {
            transform.position = startPos;
        }

        if (blocking)
        {
            Debug.Log("blocking");
        }

        //Debug.Log("Motor State: " + motor.getCurrentState.ToString());
        sm.Update();
	}

    void LateUpdate()
    {
        blocking = false;
    }

    public void SetBlock()
    {
        blocking = true;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, rigidbody.velocity);
    }
}
