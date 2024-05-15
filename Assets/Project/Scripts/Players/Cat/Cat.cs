using UnityEngine;

public class Cat : MonoBehaviour
{
    private void Awake() => ServiceLocator.Instance.RegisterService(this, true);
}