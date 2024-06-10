using System.Collections;
using UnityEngine;
public interface IMove
{
    void Move(Vector2 input);
}

public interface IJump
{
    void Jump();
    void ApplyGravity();
    void LimitVelocity();
}

public interface IStep
{
    void Step(Vector3 moveDirection);
    bool ShouldStep(Vector3 moveDirection);
}

public interface IDash
{
    void Dash();
    IEnumerator PerformDash();
    void SetWallTrigger(Collider wallCollider, bool isTrigger);
}

public interface IAscend
{
    void Ascend();
    void ApplyGravity();
}
