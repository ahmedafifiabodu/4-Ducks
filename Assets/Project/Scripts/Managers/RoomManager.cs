using Unity.Cinemachine;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [SerializeField] private CinemachineBlenderSettings _cameraBlendSettings;

    ServiceLocator serviceLocator;

    private void Start()
    {
        ServiceLocator.Instance.GetService<CameraInstance>().ChangeCustomBlend(_cameraBlendSettings);


    }
}
