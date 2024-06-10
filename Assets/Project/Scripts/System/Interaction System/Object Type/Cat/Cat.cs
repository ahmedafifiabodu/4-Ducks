using UnityEngine;

public class Cat : MonoBehaviour
{
    [SerializeField] private AttackRadius _attackRadius;
    [SerializeField] private Animator _animator;

    private void Awake() => ServiceLocator.Instance.RegisterService(this, false);

    public Transform GetTransform() => transform;
}