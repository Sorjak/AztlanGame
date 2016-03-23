using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StateMachine {
	
	public List<State> states;
    private State currentState;
	//public List<State> states;
	// Use this for initialization
	public StateMachine()
	{
		
	}
	
	public void Update()
	{
		if( currentState == null ) return;
		currentState.Update();
        Transition transition = currentState.GetTransition();
        if ( transition != null )
		{
            State next = transition.GetTarget();
            transition.OnTransition();
			Enter(next);
		}
	}
	
	public void Enter( State state )
	{
		if(currentState != null)
		{
			currentState.Exit();
		}
		state.Enter();
		currentState = state;
	}

    public State CurrentState()
    {
        return currentState;
    }
}
