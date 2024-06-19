using Unity.Cinemachine;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [SerializeField] private CinemachineBlenderSettings _cameraBlendSettings;

    private CameraInstance _cameraInstance;
    private ServiceLocator _serviceLocator;
    private SceneManagement _sceneManagement;
    private InputManager _inputManager;

    private AudioSystemFMOD _audioSystem;

    private void Start()
    {
        _serviceLocator = ServiceLocator.Instance;

        _cameraInstance = _serviceLocator.GetService<CameraInstance>();
        _sceneManagement = _serviceLocator.GetService<SceneManagement>();
        _inputManager = _serviceLocator.GetService<InputManager>();
        _audioSystem = _serviceLocator.GetService<AudioSystemFMOD>();

        _cameraInstance.ChangeCustomBlend(_cameraBlendSettings);


        if (_sceneManagement != null)
        {
            int currentLevel = _sceneManagement.CurrentLevel;

            _audioSystem.MasterBus.stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);

            switch (currentLevel)
            {
                case 1:
                    _inputManager.GhostActions.Ascend.Disable();
                    _inputManager.CatActions.Throw.Disable();
                    break;

                case 2:
                    _inputManager.GhostActions.Ascend.Disable();
                    _inputManager.CatActions.Throw.Disable();
                    _audioSystem.InitializeMusic(_audioSystem.FmodSystem.Puzzle);
                    break;

                case 3:
                    _inputManager.GhostActions.Ascend.Enable();
                    _inputManager.CatActions.Throw.Enable();
                    _audioSystem.InitializeMusic(_audioSystem.FmodSystem.Arena);

                    break;

                case 4:
                    _inputManager.GhostActions.Ascend.Enable();
                    _inputManager.CatActions.Throw.Enable();
                    _audioSystem.InitializeMusic(_audioSystem.FmodSystem.Puzzle);
                    break;
            }
        }
    }
}