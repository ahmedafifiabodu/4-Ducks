using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private void Awake()
    {
        ServiceLocator.Instance.RegisterService(this);
    }

    internal void Move(Vector3 _input)
    {
        Debug.Log("Moving");
    }

    internal void StartRun()
    {
        Debug.Log("Start Running");
    }

    internal void StopRun()
    {
        Debug.Log("Stop Running");
    }

    internal void Jump()
    {
        Debug.Log("Jumping");
    }
}