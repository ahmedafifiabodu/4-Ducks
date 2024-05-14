using UnityEngine;

public class Test : MonoBehaviour
{
    private void Awake()
    {
        ServiceLocator.Instance.RegisterService(this, false);
    }

    private void Start()
    {
        Logging.Log("Hello, World!");
    }
}