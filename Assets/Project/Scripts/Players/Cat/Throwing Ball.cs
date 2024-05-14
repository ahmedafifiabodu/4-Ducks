using System.Collections;
using UnityEngine;

public class ThrowingBall : ThrowingMechanism
{
    [SerializeField] private LineRenderer trajectoryLineRenderer;
    [SerializeField] private int numPoints = 10;
    [SerializeField] private float timeBetweenPoints = 0.1f; // Time between points
    [SerializeField] private float pointIncreaseInterval = 0.2f;

    protected override IEnumerator StartThrow()
    {
        StartCoroutine(base.StartThrow());

        trajectoryLineRenderer.enabled = true;
        trajectoryLineRenderer.positionCount = numPoints;

        float timer = 0f;

        while (true)
        {
            _currentVelocity += _velocityMultiplier * Time.deltaTime;
            timer += Time.deltaTime;

            if (timer >= pointIncreaseInterval && numPoints < 50 && _currentVelocity >= 4.5)
            {
                numPoints++;
                timer = 0f;
            }

            DrawTrajectory(numPoints);
            yield return null;
        }
    }

    protected override void Throw()
    {
        base.Throw();

        trajectoryLineRenderer.enabled = false;
        numPoints = 10;
    }

    private void DrawTrajectory(int numP)
    {
        Vector3[] points = new Vector3[numP];
        Vector3 startingPosition = transform.position;
        Vector3 startingVelocity = new(0, _currentVelocity, _currentVelocity);

        for (int i = 0; i < numP; i++)
        {
            float time = i * timeBetweenPoints;
            points[i] = startingPosition + startingVelocity * time + time * time * Physics.gravity / 2f;
        }

        trajectoryLineRenderer.positionCount = numP;
        trajectoryLineRenderer.SetPositions(points);
    }
}