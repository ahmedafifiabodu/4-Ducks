using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [SerializeField] private HealthSystem _catHealthSystem;
    [SerializeField] private HealthSystem _combinedHealthSystem;
    [SerializeField] private bool _isCombined;

    //public HealthSystem CatHealthSystem => _catHealthSystem;
    //public HealthSystem CombinedHealthSystem => _combinedHealthSystem;
    //public bool IsCombined => _isCombined;

    private void Awake() => ServiceLocator.Instance.RegisterService(this, true);

    private void OnEnable() => HealthCrystal.OnHealthCrystalCollected.AddListener(Heal);

    private void OnDisable() => HealthCrystal.OnHealthCrystalCollected.RemoveListener(Heal);

    public void Heal(float _healAmount, ObjectType _playerType)
    {
        if (_isCombined)
            _combinedHealthSystem.Heal(_healAmount);
        else
        {
            if (_playerType.IsCat)
                _catHealthSystem.Heal(_healAmount);
        }
    }

    public void TakeDamage(float _damageAmount, ObjectType _playerType)
    {
        if (_isCombined)
            _combinedHealthSystem.TakeDamage(_damageAmount);
        else
        {
            if (_playerType.IsCat)
                _catHealthSystem.TakeDamage(_damageAmount);
        }
    }
    public void InstantKill(ObjectType _playerType)
    {
        if (_isCombined)
            _combinedHealthSystem.TakeDamage(_combinedHealthSystem.HealthMax);
        else
        {
            if (_playerType.IsCat)
                _catHealthSystem.TakeDamage(_catHealthSystem.HealthMax);
        }
    }
}