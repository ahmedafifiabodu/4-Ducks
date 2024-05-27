using UnityEngine;

public class CatThrowing : ThrowingMechanism
{
    [Header("Cat Specific")]
    [SerializeField] private Animator _animator;

    private int AttackAnimationId;
    private float animationPlayTransition = 0.001f;

    protected override void OnEnable()
    {
        base.OnEnable();

        AttackAnimationId = Animator.StringToHash(GameConstant.Animation.CatAttacking);

        _inputManager.CatActions.Throw.started += _startThrowAction;
        _inputManager.CatActions.Throw.canceled += _endThrowAction;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        _inputManager.CatActions.Throw.started -= _startThrowAction;
        _inputManager.CatActions.Throw.canceled -= _endThrowAction;
    }

    protected override void Throw()
    {
        if (_checkingPlayerInput)
            initialVelocity = transform.forward * _currentVelocity;
        else
            initialVelocity = transform.up * _currentVelocity + transform.forward * _currentVelocity;

        base.Throw();

        _animator.CrossFade(AttackAnimationId, animationPlayTransition);
    }

    protected override void DrawTrajectory(int numP)
    {
        Vector3[] points = new Vector3[numP];
        Vector3 startingPosition = transform.position;
        //Vector3 startingVelocity = new Vector3(0, _currentVelocity, _currentVelocity);
        Vector3 startingVelocity = transform.up * _currentVelocity + transform.forward * _currentVelocity;

        for (int i = 0; i < numP; i++)
        {
            float time = i * timeBetweenPoints;
            points[i] = startingPosition + startingVelocity * time + time * time * Physics.gravity / 2f;
            Logging.Log($"Point {i}: {points[i]}");
        }

        trajectoryLineRenderer.positionCount = numP;
        trajectoryLineRenderer.SetPositions(points);
    }
}