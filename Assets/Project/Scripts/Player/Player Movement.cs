using UnityEngine;

public class PlayerMovement : MonoBehaviour, IDataPersistence
{
    private int _move = 0;
    private int _run = 0;
    private int _jump = 0;

    private void Awake()
    {
        ServiceLocator.Instance.RegisterService(this);
    }

    internal void Move(Vector3 _input)
    {
        _move++;
        Debug.Log("Moving");
    }

    internal void StartRun()
    {
        _run++;
        Debug.Log("Start Running");
    }

    internal void StopRun()
    {
        Debug.Log("Stop Running");
    }

    internal void Jump()
    {
        _jump++;
        Debug.Log("Jumping");
    }

    public void LoadGame(GameData _gameData)
    {
        _move = _gameData._move;
        _run = _gameData._run;
        _jump = _gameData._jump;

        transform.position = _gameData._playerPosition;
    }

    public void SaveGame(GameData _gameData)
    {
        _gameData._move = _move;
        _gameData._run = _run;
        _gameData._jump = _jump;

        _gameData._playerPosition = transform.position;
    }
}