using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    ServiceLocator serviceLocator;
    FMODEvents FmodSystemn;

    [Header("Player Steps")]
    [field: SerializeField] public EventReference PlayerSteps {  get; private set; }

    private void Awake()
    {
        ServiceLocator.Instance.RegisterService(this, true);

        serviceLocator = ServiceLocator.Instance;
        FmodSystemn = serviceLocator.GetService<FMODEvents>();
    }
}
