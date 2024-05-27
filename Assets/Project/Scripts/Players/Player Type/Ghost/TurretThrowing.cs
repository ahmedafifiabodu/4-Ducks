using UnityEngine;

public class TurretThrowing : ThrowingMechanism
{
    protected override void OnEnable()
    {
        base.OnEnable();

        _inputManager.PossessTurretActions.Fire.started += _startThrowAction;
        _inputManager.PossessTurretActions.Fire.canceled += _endThrowAction;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        _inputManager.PossessTurretActions.Fire.started -= _startThrowAction;
        _inputManager.PossessTurretActions.Fire.canceled -= _endThrowAction;
    }

    protected override void Throw()
    {
        initialVelocity = transform.forward * _currentVelocity;
        base.Throw();
    }

    protected override void DrawTrajectory(int numP)
    {
        Vector3[] points = new Vector3[numP];
        Vector3 startingPosition = transform.position;
        //Vector3 startingVelocity = new Vector3(0, _currentVelocity, _currentVelocity);
        Vector3 startingVelocity = transform.forward * _currentVelocity;

        for (int i = 0; i < numP; i++)
        {
            float time = i * timeBetweenPoints;
            points[i] = startingPosition + startingVelocity * time;
            Logging.Log($"Turret Point {i}: {points[i]}");
        }

        trajectoryLineRenderer.positionCount = numP;
        trajectoryLineRenderer.SetPositions(points);
    }
}