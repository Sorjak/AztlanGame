using System.Collections.Generic;
using System;

public class State
{
    public string name;

	public Action OnUpdate = delegate{};
	public Action OnEnter = delegate{};
	public Action OnExit = delegate{};
	private List<Transition> transitions = new List<Transition>();

    private bool isComplete;
    public bool IsComplete { get { return isComplete; } set { isComplete = value; } }

    public State() { }

    public State(string input_name)
    {
        name = input_name;
    }

	public void Update()
	{
		OnUpdate();
	}
	
	public void AddTransition( State state, Func<bool> condition )
	{
		Transition transition = new Transition( state, condition );
		transitions.Add(transition);
	}
	
	public Transition ChangeTo( State state )
	{
		Transition transition = new Transition( state);
		transitions.Add(transition);
		return transition;
	}

    public Transition GetTransition()
    {
        for (var i = 0; i < transitions.Count; i++)
        {
            if (transitions[i].Evaluate())
            {
                return transitions[i];
            }
        }
        return null;
    }
	
	public void Enter()
	{
        isComplete = false;
		OnEnter();
	}
	
	public void Exit()
	{
		OnExit();
	}

    public void OnTransitionTo(State state, Action action)
    {
        foreach (Transition transition in transitions)
        {
            if (transition.GetTarget() == state)
            {
                transition.OnTransition += action; 
            }
        }
    }

    public override string ToString()
    {
        return name;
    }
}