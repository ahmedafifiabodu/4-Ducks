using FMODUnity;
using UnityEngine;

public class FMODEvents : MonoBehaviour
{
    [Header("Player Steps")]
    [field: SerializeField] public EventReference PlayerSteps { get; private set; }

    [Header("Cat Shoot")]
    [field: SerializeField] public EventReference CatShoot { get; private set; }

    [Header("Turret Shoot")]
    [field: SerializeField] public EventReference TurretShoot { get; private set; }

    [Header("Music")]
    [field: SerializeField] public EventReference Music { get; private set; }

    private void Awake() => ServiceLocator.Instance.RegisterService(this, true);
}