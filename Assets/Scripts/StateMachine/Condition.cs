using System;
public class Condition
{
	public Condition ( Func<bool> test )
	{
		tests = test;
	}
	public Func<bool> tests = delegate{ return false; };
	
	public Condition And( Func<bool> test)
	{
		tests += test;
		return this;
	}
	
}