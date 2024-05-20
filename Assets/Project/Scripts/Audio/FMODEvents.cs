using FMODUnity;
using UnityEngine;

public class FMODEvents : MonoBehaviour
{
    [Header("Player Steps")]
    [field: SerializeField] public EventReference PlayerSteps { get; private set; }

    [Header("Music")]
    [field: SerializeField] public EventReference Music { get; private set; }

    private void Awake() => ServiceLocator.Instance.RegisterService(this, true);
}