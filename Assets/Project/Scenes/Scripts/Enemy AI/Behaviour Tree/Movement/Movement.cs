using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public abstract class Movement : Action
{
    protected abstract bool SetDestination(Vector3 destination);

    protected abstract void UpdateRotation(bool update);

    protected abstract bool HasPath();

    protected abstract Vector3 Velocity();

    protected abstract bool HasArrived();

    protected abstract void Stop();
}