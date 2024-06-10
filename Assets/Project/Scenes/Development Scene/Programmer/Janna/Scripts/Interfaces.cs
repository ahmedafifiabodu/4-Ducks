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
    void Step();
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
