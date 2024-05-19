using UnityEngine;

public class Cat : MonoBehaviour, IDamageable
{
    [SerializeField] private AttackRadius _attackRadius;
    [SerializeField] private Animator _animator;
    [SerializeField] private int _health = 100;

    private Coroutine _lookCoroutine;
    private const string ATTACK_TRIGGER = "Attack";

    private void Awake()
    {
        ServiceLocator.Instance.RegisterService(this, true);
        _attackRadius.OnAttack += OnAttack;
    }

    private void OnAttack(IDamageable _target)
    {
        _animator.SetTrigger(ATTACK_TRIGGER);

        if (_lookCoroutine != null)
            StopCoroutine(_lookCoroutine);

        _lookCoroutine = StartCoroutine(LookAt(_target.GetTransform()));
    }

    private System.Collections.IEnumerator LookAt(Transform target)
    {
        Quaternion _lookRotation = Quaternion.LookRotation(target.position - transform.position);
        float _time = 0;

        while (_time < 1)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, _time);
            _time += Time.deltaTime * 2;
            yield return null;
        }

        transform.rotation = _lookRotation;
    }

    public void TakeDamage(int damage)
    {
        _health -= damage;
        if (_health <= 0)
            gameObject.SetActive(false);
    }

    public Transform GetTransform() => transform;
}