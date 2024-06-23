using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class PresentationCamera : MonoBehaviour
{
    [SerializeField] private UnityEvent _camEvent;
    private CinemachineCamera _camera;
    public UnityEvent CamEvent => _camEvent;

    public CinemachineCamera Camera => _camera;

    private void Awake()
    {
        _camera = GetComponent<CinemachineCamera>();
    }
}
