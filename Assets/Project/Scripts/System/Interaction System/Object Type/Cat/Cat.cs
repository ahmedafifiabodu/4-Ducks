using UnityEngine;

// Cat class inherits from MonoBehaviour
public class Cat : MonoBehaviour
{
    // Serialized fields are private variables that can be set in the Unity editor
    [SerializeField] private AttackRadius _attackRadius; // Reference to the AttackRadius component

    [SerializeField] private Animator _animator; // Reference to the Animator component

    // Called when the object is first initialized
    private void Awake() => ServiceLocator.Instance.RegisterService(this, false); // Register this service in the ServiceLocator

    // Method to get the Transform component of the cat
    public Transform GetTransform() => transform;
}