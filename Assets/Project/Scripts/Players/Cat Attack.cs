using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

// This class handles the attack logic for the cat character, including animation and applying damage.
public class CatAttack : MonoBehaviour
{
    [SerializeField] private int _attackDamage; // Damage dealt by the attack
    [SerializeField] private Animator _animator; // Animator for the cat's animations
    [SerializeField] private float _attackDelay = 1f; // Delay between each attack to prevent spamming
    [SerializeField] private GameObject _hitVFXPrefab; // VFX to play when the attack hits

    private InputManager _inputManager; // Reference to the input manager
    private ObjectPool objectPool; // Reference to the ObjectPool
    private WaitForSeconds _attackDelayWaitForSeconds; // WaitForSeconds used for attack delay
    private IDamageable _damageable = null; // Reference to the damageable component of the enemy

    private int AttackAnimationId; // Hash ID for the attack animation
    private float animationPlayTransition = 0.001f; // Transition time for starting the attack animation

    private Vector3 _originalScale; // Original scale of the cat, used for animation effects
    private Vector3 _originalPosition; // Original position of the cat, used for animation effects

    private bool _isAttacking = false; // Flag to check if the cat is currently attacking

    private void Start()
    {
        // Initialize components and subscribe to input actions
        _inputManager = ServiceLocator.Instance.GetService<InputManager>();
        _inputManager.CatActions.Attack.started += Attack;

        // Get the ObjectPool from the ServiceLocator
        objectPool = ServiceLocator.Instance.GetService<ObjectPool>();

        // Convert the animation name to a hash ID for efficient animation parameter handling
        AttackAnimationId = Animator.StringToHash(GameConstant.CatAnimation.Attacking);

        // Cache the original scale of the cat
        _originalScale = transform.localScale;

        // Initialize the WaitForSeconds object for attack delay
        _attackDelayWaitForSeconds = new WaitForSeconds(_attackDelay);
    }

    private void Attack(InputAction.CallbackContext context)
    {
        // Prevents attack action if already attacking
        if (_isAttacking) return;

        _originalPosition = transform.position;
        StartCoroutine(AttackRoutine());
    }

    private System.Collections.IEnumerator AttackRoutine()
    {
        _isAttacking = true;
        _animator.CrossFade(AttackAnimationId, animationPlayTransition); // Play attack animation

        // Create a sequence for visual effects during the attack
        Sequence attackSequence = DOTween.Sequence();
        attackSequence.Append(transform.DOScale(_originalScale * 1.1f, 0.1f).SetEase(Ease.InOutSine));
        attackSequence.Append(transform.DOMove(transform.position + transform.forward * 1f, 0.2f).SetEase(Ease.InOutSine));
        attackSequence.Append(transform.DOScale(_originalScale, 0.1f).SetEase(Ease.InOutSine));
        attackSequence.Join(transform.DOMove(_originalPosition, 0.2f).SetEase(Ease.InOutSine));

        yield return _attackDelayWaitForSeconds; // Wait for the attack delay

        _isAttacking = false; // Reset the attacking flag
    }

    // This method is called when the attack animation reaches its designated point to apply damage and play VFX
    internal void OnAttackAnimationCompleted()
    {
        if (_damageable != null)
        {
            _damageable.TakeDamage(_attackDamage);
            if (_hitVFXPrefab != null)
            {
                Logging.Log("HERE");

                // Get a pooled object from the ObjectPool
                GameObject hitVFX = objectPool.GetPooledObject(_hitVFXPrefab);

                // Calculate the position for the VFX to appear in front of the enemy, based on the cat's attack direction
                Vector3 hitPosition = _damageable.GetTransform().position + (transform.up * 1.5f); // Adjust the 0.5f value as needed to position the VFX correctly

                hitVFX.transform.SetPositionAndRotation(hitPosition, Quaternion.identity);

                // Activate the VFX
                hitVFX.SetActive(true);

                // Use DOTween to animate the VFX. Assuming you want to fade it out, then deactivate it.
                // Adjust the duration and values according to your needs.
                hitVFX.GetComponent<Renderer>().material.DOFade(0, 1f).OnComplete(() =>
                {
                    hitVFX.SetActive(false);
                    hitVFX.GetComponent<Renderer>().material.DOFade(1, 0); // Reset the fade so it's visible next time
                });
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Detects damageable enemies when they enter the attack range
        if (other.TryGetComponent<IDamageable>(out var damageable))
            _damageable = damageable;
    }

    private void OnTriggerExit(Collider other)
    {
        // Clears the reference to the damageable enemy when it exits the attack range
        if (other.TryGetComponent<IDamageable>(out var _))
            _damageable = null;
    }
}