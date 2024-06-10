using UnityEngine;

public class TurretThrowing : ThrowingMechanism
{
    protected override void OnEnable()
    {
        base.OnEnable();

        InputManager.PossessTurretActions.Fire.started += StartThrowAction;
        InputManager.PossessTurretActions.Fire.canceled += EndThrowAction;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        InputManager.PossessTurretActions.Fire.started -= StartThrowAction;
        InputManager.PossessTurretActions.Fire.canceled -= EndThrowAction;
    }

    protected override void Throw()
    {
        InitialVelocity = CurrentVelocity * 4 * transform.forward;

        AudioSystem.PlayerShooting(AudioSystem.FmodSystem.TurretShoot, this.gameObject.transform.position);

        base.Throw();
    }

    protected override void DrawTrajectory(int numP)
    {
        Vector3[] points = new Vector3[numP];
        Vector3 startingPosition = transform.position;
        Vector3 startingVelocity = transform.forward * CurrentVelocity;

        for (int i = 0; i < numP; i++)
        {
            float time = i * TimeBetweenPoints;
            points[i] = startingPosition + startingVelocity * time;
        }

        TrajectoryLineRenderer.positionCount = numP;
        TrajectoryLineRenderer.SetPositions(points);
    }
}