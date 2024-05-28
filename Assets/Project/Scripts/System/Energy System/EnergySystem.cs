using UnityEngine;
using UnityEngine.Events;

public class EnergySystem : MonoBehaviour
{
    public UnityEvent OnEnergyChanged;
    public UnityEvent OnMaxEnergy;
    public UnityEvent OnNoEnergy;

    public UnityEvent OnGainEnergy;
    public UnityEvent OnLoseEnergy;
//    public UnityEvent OnLowEnergy;  //if needed.

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
        EnergyCrystal.OnEnergyCrystalCollected.AddListener(GainEnergy);
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
    private void OnDisable()
    {
        EnergyCrystal.OnEnergyCrystalCollected.RemoveListener(GainEnergy);
    }
}
