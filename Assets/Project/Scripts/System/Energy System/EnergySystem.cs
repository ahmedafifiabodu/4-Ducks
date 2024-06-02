using UnityEngine;
using UnityEngine.Events;

public class EnergySystem : MonoBehaviour
{
    [SerializeField] private float _maxEnergy;
    [SerializeField] private float _minEnergyRange;
    [SerializeField] private float _startEnergy;

    [SerializeField] private UnityEvent _onEnergyChanged;
    [SerializeField] private UnityEvent _onMaxEnergy;
    [SerializeField] private UnityEvent _onNoEnergy;
    [SerializeField] private UnityEvent _onGainEnergy;
    [SerializeField] private UnityEvent _onLoseEnergy;
    //[SerializeField] private UnityEvent _onLowEnergy;  //if needed.


    private float _energy = 0;
    internal float EnergyPrecentage => (_energy / _maxEnergy);
    internal UnityEvent OnEnergyChanged => _onEnergyChanged;

    private void Start()
    {
        _energy = _startEnergy;
    }
    public void GainEnergy(float energyAmount)
    {
        _energy += energyAmount;

        if (_energy > _maxEnergy)
        {
            _energy = _maxEnergy;
            _onMaxEnergy?.Invoke();
        }

        OnEnergyChanged?.Invoke();
        _onGainEnergy?.Invoke();
    }

    public void LoseEnergy(float energyAmount)
    {
        _energy -= energyAmount;

        if (_energy <= 0)
        {
            _energy = 0;
            _onNoEnergy?.Invoke();
        }

        // To apply effect on low energy can be used later? Note: Still needs alittle edit to be used.

        //if (_energy <= _minEnergyRange)
        //{
        //    OnLowEnergy?.Invoke();
        //}

        OnEnergyChanged?.Invoke();
        _onLoseEnergy?.Invoke();
    }
}