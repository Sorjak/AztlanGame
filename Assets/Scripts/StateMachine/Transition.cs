using System.Collections.Generic;
using System;
using UnityEngine;

public class Transition
{
    public Action OnTransition = delegate {};

	private List<Condition> conditions = new List<Condition>();
	private State target;
	private Func<bool> condition = delegate{ return false; };
	public State Target
	{
		get
		{
			return target;
		}
		set{}
	}
	public Transition ( State state, Func<bool> test )
	{
		target = state;
		Condition c = new Condition(test);
		conditions.Add(c);
	}
	
	public Transition ( State state )
	{
		target = state;
	}
	
	public Condition If ( Func<bool> test )
	{
		Condition c = new Condition(test);
		conditions.Add(c);
		return c;
	}
	public bool Evaluate()
	{
		// ORS
		for (int i = 0; i < conditions.Count; i++)
		{
			// ANDS
			//Debug.Log("testing");
			bool valid = true;
			foreach (Func<bool> f in conditions[i].tests.GetInvocationList())
			{
				if(f() != true)
				{
					valid = false;
					break;
				}
				//Debug.Log("successful test");
			}
			if(valid)
			{
				return true;
			}
		}
		return false;
	}

    public State GetTarget()
    {
        return target;
    }

}