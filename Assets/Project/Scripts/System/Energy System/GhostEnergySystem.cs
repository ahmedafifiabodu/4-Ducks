using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostEnergySystem : EnergySystem
{
    private void OnEnable() => EnergyCrystal._onEnergyCrystalCollected.AddListener(GainEnergy);

    private void OnDisable() => EnergyCrystal._onEnergyCrystalCollected.RemoveListener(GainEnergy);
}
