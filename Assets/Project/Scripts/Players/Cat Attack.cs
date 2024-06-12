using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class CatAttack : MonoBehaviour
{
    [SerializeField] private int _attackDamage; // Damage dealt by the attack
    [SerializeField] private Animator _animator; // Animator for the cat's animations

    private InputManager _inputManager; // Reference to the input manager
    private WaitForSeconds _attackDelayWaitForSeconds; // WaitForSeconds used for attack delay
    private IDamageable _damageable = null; // Reference to the damageable component of the enemy

    private int AttackAnimationId; // Hash ID for the attack animation
    private float animationPlayTransition = 0.001f; // Transition time for starting the attack animation

    private Vector3 _originalScale; // Original scale of the cat, used for animation effects
    private Vector3 _originalPosition; // Original position of the cat, used for animation effects

    private bool _isAttacking = false; // Flag to check if the cat is currently attacking
    private float _attackDelay = 1f; // Delay between each attack to prevent spamming

    private void Start()
    {
        // Get the input manager from the service locator
        _inputManager = ServiceLocator.Instance.GetService<InputManager>();
        // Subscribe to the attack action in the input manager
        _inputManager.CatActions.Attack.started += Attack;

        // Convert the animation name to a hash for performance
        AttackAnimationId = Animator.StringToHash(GameConstant.CatAnimation.Attacking);

        // Store the original scale of the cat
        _originalScale = transform.localScale;

        // Initialize the WaitForSeconds with the attack delay
        _attackDelayWaitForSeconds = new WaitForSeconds(_attackDelay);
    }

    private void Attack(InputAction.CallbackContext context)
    {
        // If the cat is already attacking, ignore further input
        if (_isAttacking) return;

        // Store the original position of the cat
        _originalPosition = transform.position;
        // Start the attack routine
        StartCoroutine(AttackRoutine());
    }

    private System.Collections.IEnumerator AttackRoutine()
    {
        _isAttacking = true; // Indicate that the cat is attacking

        // Play the attack animation with a very quick transition
        _animator.CrossFade(AttackAnimationId, animationPlayTransition);

        // Create a sequence for the attack effect using DOTween
        Sequence attackSequence = DOTween.Sequence();

        // Scale the cat up slightly for a visual effect
        attackSequence.Append(transform.DOScale(_originalScale * 1.1f, 0.1f).SetEase(Ease.InOutSine));

        // Dash the cat forward slightly for a visual effect
        attackSequence.Append(transform.DOMove(transform.position + transform.forward * 1f, 0.2f).SetEase(Ease.InOutSine));

        // Scale the cat back down and return to the original position
        attackSequence.Append(transform.DOScale(_originalScale, 0.1f).SetEase(Ease.InOutSine));
        attackSequence.Join(transform.DOMove(_originalPosition, 0.2f).SetEase(Ease.InOutSine));

        // Wait for the attack delay before allowing another attack
        yield return _attackDelayWaitForSeconds;

        _isAttacking = false; // Reset the attacking flag
    }

    // Called when the attack animation reaches its designated point
    internal void OnAttackAnimationCompleted() => _damageable?.TakeDamage(_attackDamage);

    private void OnTriggerEnter(Collider other)
    {
        // When entering a collider, check if it has an IDamageable component
        if (other.TryGetComponent<IDamageable>(out var damageable))
            _damageable = damageable; // Store the reference to apply damage later
    }

    private void OnTriggerExit(Collider other)
    {
        // When exiting a collider, clear the stored IDamageable reference
        if (other.TryGetComponent<IDamageable>(out var _))
            _damageable = null;
    }
}