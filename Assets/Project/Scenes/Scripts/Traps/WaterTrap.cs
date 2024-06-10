using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterTrap : Interactable
{
    protected override void Interact(ObjectType _objectType)
    {
        base.Interact(_objectType);
        if(_objectType.gameObject.TryGetComponent<HealthSystem>(out HealthSystem Death))
            Death.Die();
    }
}
