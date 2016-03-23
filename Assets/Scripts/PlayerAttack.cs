using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour {

    public class PlayerAttackStateMachine : StateMachine
    {
        public State Idle = new State("Not Attacking");

        public State lightPunch = new State("Light Punch");
        public State strongPunch = new State("Strong Punch");

        public State lightKick = new State("Light Kick");
        public State strongKick = new State("Strong Kick");
    }

    public PlayerAttackStateMachine sm;

    public Player player;

    public AttackHitBox strongPunch;
    public AttackHitBox highKick;
    public AttackHitBox airLowKick;
    public AttackHitBox airPunch;

    //private Animator animator;


    private AttackHitBox currentAttack { get; set; }
    public bool attacking { get { return currentAttack != null; } }

    void Start()
    {
        player = GetComponent<Player>();
        //animator = GetComponent<Animator>();
        sm = new PlayerAttackStateMachine();

        sm.Idle.OnUpdate += () => { };

        sm.Idle.ChangeTo(sm.lightPunch).If(() => player.input.actionPunch && Mathf.Abs(player.input.actionHorizontalMove) == 0);

        sm.lightPunch.OnEnter += () => player.animator.Play("strong_punch");

        sm.Enter(sm.Idle);
    }

    void Update() {
        Debug.Log("Attack State: " + sm.CurrentState().ToString());
        sm.Update();
    }

    public void RemoveAttack()
    {
        Debug.Log("Removing attack");
        Destroy(currentAttack.gameObject);
        currentAttack = null;
        sm.Enter(sm.Idle);
    }

    public void AddAttack(AttackHitBox prefab)
    {
        Debug.Log("Adding attack");
        currentAttack = Instantiate(prefab) as AttackHitBox;
        currentAttack.transform.SetParent(this.transform, false);
    }

    public void StrongPunch()
    {
        AddAttack(strongPunch);
    }

    public void HighKick()
    {
        AddAttack(highKick);
    }

    public void AirLowKick()
    {
        AddAttack(airLowKick);
    }

    public void AirPunch()
    {
        AddAttack(airPunch);
    }

}
