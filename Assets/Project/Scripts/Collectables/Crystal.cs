using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Crystal : MonoBehaviour, ICollectable
{
    public static UnityEvent<float> OnCrystalCollected = new UnityEvent<float>();

    public abstract void Ability();
    public virtual void Collect()
    {
        OnCrystalCollected?.Invoke(GetEnergyAmount());
        gameObject.SetActive(false);
    }
    protected abstract float GetEnergyAmount();
}
