using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class EnemyLineChecker : MonoBehaviour
{
    [SerializeField] private SphereCollider _collider;
    private float _fieldOfView = 90f;
    private LayerMask _lineOfSightLayers;
    private Cat _catInTrigger; // Use Object Type Script

    public delegate void GainSightEvent(Cat _cat);

    public delegate void LoseSightEvent(Cat _cat);

    public event GainSightEvent OnGainSight;

    public event LoseSightEvent OnLoseSight;

    internal float FieldOfView { get => _fieldOfView; set => _fieldOfView = value; }
    internal float ColliderRadius { get => _collider.radius; set => _collider.radius = value; }
    internal LayerMask LineOfSightLayers { get => _lineOfSightLayers; set => _lineOfSightLayers = value; }

    private void Awake() => _collider = GetComponent<SphereCollider>();

    private void FixedUpdate()
    {
        if (_catInTrigger != null)
            CheckLineOfSight(_catInTrigger);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Cat cat))
            _catInTrigger = cat;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Cat cat))
        {
            OnLoseSight?.Invoke(cat);
            _catInTrigger = null;
        }
    }

    private bool CheckLineOfSight(Cat cat)
    {
        Vector3 _direction = (cat.transform.position - transform.position).normalized;

        if (Vector3.Dot(transform.forward, _direction) >= Mathf.Cos(_fieldOfView))
            if (Physics.Raycast(transform.position, _direction, out RaycastHit hit, _collider.radius, _lineOfSightLayers))
                if (hit.transform.GetComponent<Cat>() != null)
                {
                    OnGainSight?.Invoke(cat);
                    return true;
                }

        return false;
    }
}