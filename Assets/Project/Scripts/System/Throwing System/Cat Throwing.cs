using FMOD.Studio;
using UnityEngine;

public class CatThrowing : ThrowingMechanism
{
    // Serialized fields that can be set in the Unity editor
    [Header("Cat Specific")]
    [SerializeField] private Animator _animator; // Animator for the cat

    private int AttackAnimationId; // ID for the attack animation
    private float animationPlayTransition = 0.001f; // Transition time for the animation

 
    // Called when the object is enabled
    protected override void OnEnable()
    {
        base.OnEnable();

        // Get the ID for the attack animation
        AttackAnimationId = Animator.StringToHash(GameConstant.CatAnimation.Attacking);

        // Set up the input actions
        _inputManager.CatActions.Throw.started += _startThrowAction;
        _inputManager.CatActions.Throw.canceled += _endThrowAction;
    }

    // Called when the object is disabled
    protected override void OnDisable()
    {
        base.OnDisable();

        //CatShootingSFX.stop(STOP_MODE.ALLOWFADEOUT);
        // Remove the input actions
        _inputManager.CatActions.Throw.started -= _startThrowAction;
        _inputManager.CatActions.Throw.canceled -= _endThrowAction;
    }

    // Throw the ball
    protected override void Throw()
    {
        if (_checkingPlayerInput)
            initialVelocity = transform.forward * _currentVelocity;
        else
            initialVelocity = transform.up * _currentVelocity + transform.forward * _currentVelocity;

        base.Throw();

        AudioSystem.PlayerShooting(AudioSystem.FmodSystem.CatShoot , this.gameObject.transform.position);

        // Play the attack animation
        _animator.CrossFade(AttackAnimationId, animationPlayTransition);
    }

    // Draw the trajectory
    protected override void DrawTrajectory(int numP)
    {
        Vector3[] points = new Vector3[numP];
        Vector3 startingPosition = transform.position;
        Vector3 startingVelocity = transform.up * _currentVelocity + transform.forward * _currentVelocity;

        // Calculate the points for the trajectory
        for (int i = 0; i < numP; i++)
        {
            float time = i * timeBetweenPoints;
            points[i] = startingPosition + startingVelocity * time + time * time * Physics.gravity / 2f;
        }

        // Set the points for the line renderer
        trajectoryLineRenderer.positionCount = numP;
        trajectoryLineRenderer.SetPositions(points);
    }
}