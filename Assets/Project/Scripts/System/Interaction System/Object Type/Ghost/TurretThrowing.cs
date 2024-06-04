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
        initialVelocity = _currentVelocity * 4 * transform.forward;

        AudioSystem.PlayerShooting(AudioSystem.FmodSystem.TurretShoot, this.gameObject.transform.position);

        base.Throw();
    }

    protected override void DrawTrajectory(int numP)
    {
        Vector3[] points = new Vector3[numP];
        Vector3 startingPosition = transform.position;
        Vector3 startingVelocity = transform.forward * _currentVelocity;

        for (int i = 0; i < numP; i++)
        {
            float time = i * timeBetweenPoints;
            points[i] = startingPosition + startingVelocity * time;
        }

        trajectoryLineRenderer.positionCount = numP;
        trajectoryLineRenderer.SetPositions(points);
    }
}