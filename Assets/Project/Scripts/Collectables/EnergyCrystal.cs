using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class EnergyCrystal : Crystal
{
    public static UnityEvent<float> OnEnergyCrystalCollected = new UnityEvent<float>();

    [SerializeField] private float _energyAmount;
    public float EnergyAmount => _energyAmount;
    public override void Ability()
    {
        throw new System.NotImplementedException();
    }
    public override void Collect()
    {
        base.Collect();
        OnEnergyCrystalCollected?.Invoke(_energyAmount);
    }
    private void OnTriggerEnter(Collider other)
    {
        Collect();
    }

}
