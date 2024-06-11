using UnityEngine;

public class Ghost : MonoBehaviour
{
    private void Awake() => ServiceLocator.Instance.RegisterService(this, false);

    internal Transform GetTransform() => transform;
}