using UnityEngine;

public class FallToCheckPoint : MonoBehaviour
{
    [SerializeField] private float _fallHeightDistance = 2;

    private float _previousYPosition;
    private float _lastYPosition;
    private bool _isFalling = false;

    private SpawnSystem _spawnSystem;

    private void Start() => _spawnSystem = ServiceLocator.Instance.GetService<SpawnSystem>();

    private void Update()
    {
        float currentYPosition = transform.position.y;

        if (_isFalling)
        {
            // Check if the player has landed
            if (Physics.Raycast(transform.position, Vector3.down, 0.1f))
            {
                _isFalling = false;

                // Calculate fall damage based on the height fallen
                float _fallHeight = _lastYPosition - currentYPosition;

                if (_fallHeight > _fallHeightDistance) // Only apply fall damage if fallen more than 5 units
                {
                    float _fallDamage = _fallHeight - _fallHeightDistance; // Calculate fall damage
                    _spawnSystem.SpawnAtLastCheckPoint();
                }
            }
        }
        else
        {
            // Check if the player has started falling
            if (currentYPosition < _previousYPosition)
            {
                _isFalling = true;
                _lastYPosition = currentYPosition;
            }
        }

        _previousYPosition = currentYPosition;
    }
}