using UnityEngine;
using BehaviorDesigner.Runtime;

[System.Serializable]
public class NewVariable
{

}

[System.Serializable]
public class SharedNewVariable : SharedVariable<NewVariable>
{
	public override string ToString() { return mValue == null ? "null" : mValue.ToString(); }
	public static implicit operator SharedNewVariable(NewVariable value) { return new SharedNewVariable { mValue = value }; }
}