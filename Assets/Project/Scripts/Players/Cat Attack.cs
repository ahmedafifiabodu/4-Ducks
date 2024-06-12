using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class CatAttack : MonoBehaviour
{
    [SerializeField] private Animator _animator; // Animator for the cat

    private InputManager _inputManager;
    private WaitForSeconds _attackDelayWaitForSeconds;

    private int AttackAnimationId; // ID for the attack animation
    private float animationPlayTransition = 0.001f; // Transition time for the animation

    private Vector3 _originalScale; // Original scale of the cat
    private Vector3 _originalPosition; // Original position of the cat

    private bool _isAttacking = false; // Flag to check if the cat is currently attacking
    private float _attackDelay = 1f; // Delay between each attack

    private void Start()
    {
        _inputManager = ServiceLocator.Instance.GetService<InputManager>();
        _inputManager.CatActions.Attack.started += Attack;

        AttackAnimationId = Animator.StringToHash(GameConstant.CatAnimation.Attacking);

        _originalScale = transform.localScale; // Store the original scale

        _attackDelayWaitForSeconds = new WaitForSeconds(_attackDelay);
    }

    private void Attack(InputAction.CallbackContext context)
    {
        if (_isAttacking) return; // If the cat is already attacking, ignore the input

        _originalPosition = transform.position; // Store the original position
        StartCoroutine(AttackRoutine());
    }

    private System.Collections.IEnumerator AttackRoutine()
    {
        _isAttacking = true; // Set the flag to true

        // Play the attack animation
        _animator.CrossFade(AttackAnimationId, animationPlayTransition);

        // Create a sequence for the attack effect
        Sequence attackSequence = DOTween.Sequence();

        // Scale the cat up
        attackSequence.Append(transform.DOScale(_originalScale * 1.1f, 0.1f).SetEase(Ease.InOutSine));

        // Dash the cat forward
        attackSequence.Append(transform.DOMove(transform.position + transform.forward * 1f, 0.2f).SetEase(Ease.InOutSine));

        // Then scale the cat back down and return to the original position
        attackSequence.Append(transform.DOScale(_originalScale, 0.1f).SetEase(Ease.InOutSine));
        attackSequence.Join(transform.DOMove(_originalPosition, 0.2f).SetEase(Ease.InOutSine));

        yield return _attackDelayWaitForSeconds; // Wait for the delay

        _isAttacking = false; // Reset the flag
    }
}