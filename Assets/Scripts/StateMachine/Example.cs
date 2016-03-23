using UnityEngine;
using System;

public class TestMachine : MonoBehaviour
{
	private SubStatemachine sm;
	public bool walking;
	void Start()
	{
		sm = new SubStatemachine();
		sm.Idle.OnUpdate += delegate { Debug.Log("idling");};
		sm.Idle.OnUpdate += DoThis;
		sm.Idle.OnEnter += delegate{Debug.Log("Entering idle");};
		sm.Idle.ChangeTo(sm.Walking).If(delegate{ return walking; }).And(transform.IsAboveGround);
		
		sm.Walking.OnEnter += delegate{ Debug.Log("entering walk");};
		sm.Walking.OnUpdate += delegate{ transform.position += new Vector3(Time.deltaTime, 0, 0); };
		sm.Enter(sm.Idle);
	}
	
	void Update()
	{
		sm.Update();
	}
	
	void DoThis()
	{
		Debug.Log("doing This");
	}
	
	void OnUpdate()
	{

	}
}

public class SubStatemachine : StateMachine
{
	public State Idle = new State();
	public State Walking = new State();
	public State Jumping = new State();
}

public static class Conditions
{
	public static bool IsAboveGround(this Transform t)
    {
	 	return t.position.y > 0;
    }
}