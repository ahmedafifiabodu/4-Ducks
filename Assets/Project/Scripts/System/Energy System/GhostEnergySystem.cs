using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostEnergySystem : EnergySystem
{
    [SerializeField] private float _energyDecreaseRate; 
    private Coroutine _energyDecreaseCoroutine;
    private void OnEnable() => EnergyCrystal._onEnergyCrystalCollected.AddListener(GainEnergy);

    private void OnDisable() => EnergyCrystal._onEnergyCrystalCollected.RemoveListener(GainEnergy);

    public void StartEnergyDecrease()
    {
        if (_energyDecreaseCoroutine == null)
        {
            _energyDecreaseCoroutine = StartCoroutine(DecreaseEnergyOverTime());
        }
    }
    public void StopEnergyDecrease()
    {
        if (_energyDecreaseCoroutine != null)
        {
            StopCoroutine(_energyDecreaseCoroutine);
            _energyDecreaseCoroutine = null;
        }
    }
    private IEnumerator DecreaseEnergyOverTime()
    {
        while (_energy > 0)
        {
            _energy -= _energyDecreaseRate * Time.deltaTime;

            if (_energy <= 0)
            {
                _energy = 0;
                _onNoEnergy?.Invoke();
            }

            OnEnergyChanged?.Invoke();
            yield return null;
        }
    }
}
