using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class EnergyCrystal : Crystal
{
    [SerializeField] private float _energyAmount;
    public float EnergyAmount => _energyAmount;
    public override void Ability()
    {
        throw new System.NotImplementedException();
    }
    protected override float GetEnergyAmount()
    {
       return _energyAmount;
    }
    private void OnTriggerEnter(Collider other)
    {
        Collect();
    }

}
