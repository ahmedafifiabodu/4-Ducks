using UnityEngine;
using UnityEngine.Events;

public class EnergySystem : MonoBehaviour
{
    public UnityEvent OnEnergyChanged;
    public UnityEvent OnMaxEnergy;
    public UnityEvent OnLowEnergy;

    [SerializeField] private float _maxEnergy;
    [SerializeField] private float _minEnergyRange;

    private float _energy;
    public float Energt => _energy;
    public float EnergyPrecentage => (_energy / _maxEnergy);

    private void Awake()
    {
        _energy = 0;
    }
    
    private void OnEnable()
    {
        EnergyCrystal.OnCrystalCollected.AddListener(GainEnergy);
    }
    public void GainEnergy(float energyAmount)
    {
        _energy += energyAmount;
        if (_energy > _maxEnergy)
        {
            _energy = _maxEnergy;
            OnMaxEnergy?.Invoke();
        }
        Logging.Log("EnergyCollected");
        OnEnergyChanged?.Invoke();
    }
    public void LoseEnergy(float energyAmount)
    {
        _energy -= energyAmount;
        if (_energy <= _minEnergyRange)
        {
            OnLowEnergy?.Invoke();
        }
        OnEnergyChanged?.Invoke();
    }
    private void OnDisable()
    {
        EnergyCrystal.OnCrystalCollected.RemoveListener
            (GainEnergy);

    }
}
