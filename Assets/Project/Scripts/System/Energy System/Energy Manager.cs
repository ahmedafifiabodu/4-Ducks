using UnityEngine;

public class EnergyManager : MonoBehaviour
{
    [SerializeField] private bool _isCombined;

    [SerializeField] private EnergySystem _catEenergySystem;
    [SerializeField] private EnergySystem _ghostEnergySystem;
    [SerializeField] private EnergySystem _combinedEnergySystem;

    internal bool IsCombined => _isCombined;
    internal EnergySystem CatEnergySystem => _catEenergySystem;
    internal EnergySystem GhostEnergySystem => _ghostEnergySystem;
    internal EnergySystem CombinedEnergySystem => _combinedEnergySystem;

    private void Awake() => ServiceLocator.Instance.RegisterService(this, true);

    private void OnEnable() => EnergyCrystal.OnEnergyCrystalCollected.AddListener(GainEnergy);

    private void OnDisable() => EnergyCrystal.OnEnergyCrystalCollected.RemoveListener(GainEnergy);

    private void GainEnergy(float _energyAmount, PlayerType _playerType)
    {
        if (_isCombined)
            _combinedEnergySystem.GainEnergy(_energyAmount);
        else
        {
            if (_playerType.IsPlayerCat)
                _catEenergySystem.GainEnergy(_energyAmount);

            if (_playerType.IsPlayerGhost)
                _ghostEnergySystem.GainEnergy(_energyAmount);
        }
    }

    private void LoseEnergy(float _energyAmount, PlayerType _playerType)
    {
        if (_isCombined)
            _combinedEnergySystem.LoseEnergy(_energyAmount);
        else
        {
            if (_playerType.IsPlayerCat)
                _catEenergySystem.LoseEnergy(_energyAmount);

            if (_playerType.IsPlayerGhost)
                _ghostEnergySystem.LoseEnergy(_energyAmount);
        }
    }

    private void EditCombinedState(bool isCombined) => _isCombined = isCombined;
}