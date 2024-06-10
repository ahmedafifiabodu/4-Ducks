using UnityEngine;

public class Ghost : MonoBehaviour
{
    private void Awake() => ServiceLocator.Instance.RegisterService(this, true);
    public Transform GetTransform() => transform;
}