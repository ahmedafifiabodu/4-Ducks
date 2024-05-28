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

    private void Heal(float _healAmount, PlayerType _playerType)
    {
        if (_isCombined)
            _combinedHealthSystem.Heal(_healAmount);
        else
        {
            if (_playerType.IsPlayerCat)
                _catHealthSystem.Heal(_healAmount);
        }
    }

    private void TakeDamage(float _damageAmount, PlayerType _playerType)
    {
        if (_isCombined)
            _combinedHealthSystem.TakeDamage(_damageAmount);
        else
        {
            if (_playerType.IsPlayerCat)
                _catHealthSystem.TakeDamage(_damageAmount);
        }
    }
}