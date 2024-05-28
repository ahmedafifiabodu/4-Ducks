using UnityEngine;
using UnityEngine.Events;

public class EnergySystem : MonoBehaviour
{
    [SerializeField] private float _maxEnergy;
    [SerializeField] private float _minEnergyRange;

    internal UnityEvent OnEnergyChanged;
    internal UnityEvent OnMaxEnergy;
    internal UnityEvent OnNoEnergy;

    internal UnityEvent OnGainEnergy;
    internal UnityEvent OnLoseEnergy;
    //public UnityEvent OnLowEnergy;  //if needed.

    private float _energy = 0;
    internal float Energt => _energy;
    internal float EnergyPrecentage => (_energy / _maxEnergy);

    public void GainEnergy(float energyAmount)
    {
        _energy += energyAmount;

        if (_energy > _maxEnergy)
        {
            _energy = _maxEnergy;
            OnMaxEnergy?.Invoke();
        }

        OnEnergyChanged?.Invoke();
        OnGainEnergy?.Invoke();
    }

    public void LoseEnergy(float energyAmount)
    {
        _energy -= energyAmount;

        if (_energy <= 0)
        {
            _energy = 0;
            OnNoEnergy?.Invoke();
        }

        // To apply effect on low energy can be used later? Note: Still needs alittle edit to be used.

        //if (_energy <= _minEnergyRange)
        //{
        //    OnLowEnergy?.Invoke();
        //}

        OnEnergyChanged?.Invoke();
        OnLoseEnergy?.Invoke();
    }
}