using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEventListener : MonoBehaviour
{
    public GameEvent _gameEvent;
    public UnityEvent _responce;

    private void OnEnable()
    {
        _gameEvent.RegisterListener(this); 
    }
    private void OnDisable()
    {
        _gameEvent.UnRegisterListener(this);
    }
    public void OnEventRaised()
    {
        _responce.Invoke();
    }
}
