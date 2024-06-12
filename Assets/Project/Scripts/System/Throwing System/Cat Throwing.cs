using UnityEngine;

// CatThrowing class inherits from ThrowingMechanism
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
        InputManager.CatActions.Throw.started += StartThrowAction;
        InputManager.CatActions.Throw.canceled += EndThrowAction;
    }

    // Called when the object is disabled
    protected override void OnDisable()
    {
        base.OnDisable();

        // Remove the input actions
        InputManager.CatActions.Throw.started -= StartThrowAction;
        InputManager.CatActions.Throw.canceled -= EndThrowAction;
    }

    // Throw the ball
    protected override void Throw()
    {
        if (CheckingPlayerInput)
            InitialVelocity = transform.forward * CurrentVelocity;
        else
            InitialVelocity = transform.up * CurrentVelocity + transform.forward * CurrentVelocity;

        base.Throw();

        // Play the shooting sound
        AudioSystem.PlayerShooting(AudioSystem.FmodSystem.CatShoot, this.gameObject.transform.position);

        // Play the attack animation
        _animator.CrossFade(AttackAnimationId, animationPlayTransition);
    }

    // Draw the trajectory
    protected override void DrawTrajectory(int numP)
    {
        Vector3[] points = new Vector3[numP];
        Vector3 startingPosition = transform.position;
        Vector3 startingVelocity = transform.up * CurrentVelocity + transform.forward * CurrentVelocity;

        // Calculate the points for the trajectory
        for (int i = 0; i < numP; i++)
        {
            float time = i * TimeBetweenPoints;
            points[i] = startingPosition + startingVelocity * time + time * time * Physics.gravity / 2f;
        }

        // Set the points for the line renderer
        TrajectoryLineRenderer.positionCount = numP;
        TrajectoryLineRenderer.SetPositions(points);
    }
}