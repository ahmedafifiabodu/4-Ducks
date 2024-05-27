using System.Collections;
using UnityEngine;

/*public class ThrowingBall : ThrowingMechanism
{
    [Header("Trajectory Line Renderer")]
    [SerializeField] private LineRenderer trajectoryLineRenderer;

    [SerializeField] private int numPoints = 10;
    [SerializeField] private float timeBetweenPoints = 0.1f; // Time between points
    [SerializeField] private float pointIncreaseInterval = 0.2f;

    [SerializeField] private Animator _animator;
    [SerializeField] private float animationPlayTransition = 0.001f;
    private int AttackAnimationId;

    private void Awake()
    {
        //_animator = GetComponent<Animator>();
        AttackAnimationId = Animator.StringToHash(GameConstant.Animation.CatAttacking);
    }

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

        if (IsCat)
            _animator.CrossFade(AttackAnimationId, animationPlayTransition);

        trajectoryLineRenderer.enabled = false;
        numPoints = 10;
    }

    private void DrawTrajectory(int numP)
    {
        Vector3[] points = new Vector3[numP];
        Vector3 startingPosition = transform.position;
        //Vector3 startingVelocity = new Vector3(0, _currentVelocity, _currentVelocity);
        Vector3 startingVelocity = transform.up * _currentVelocity + transform.forward * _currentVelocity;

        for (int i = 0; i < numP; i++)
        {
            float time = i * timeBetweenPoints;
            points[i] = startingPosition + startingVelocity * time + time * time * Physics.gravity / 2f;
        }

        trajectoryLineRenderer.positionCount = numP;
        trajectoryLineRenderer.SetPositions(points);
    }
}*/