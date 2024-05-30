using UnityEngine;

public class Cat : MonoBehaviour, IDamageable
{
    [SerializeField] private AttackRadius _attackRadius;
    [SerializeField] private Animator _animator;
    [SerializeField] private int _health = 100;

    private void Awake() => ServiceLocator.Instance.RegisterService(this, true);

    public void TakeDamage(int damage)
    {
        _health -= damage;
        if (_health <= 0)
            gameObject.SetActive(false);
    }

    public Transform GetTransform() => transform;
}