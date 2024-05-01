using System;
using System.Collections.Generic;

public class ServiceLocator
{
    private readonly IDictionary<object, object> services = new Dictionary<object, object>();

    private static ServiceLocator _instance;

    public static ServiceLocator Instance
    {
        get
        {
            _instance ??= new ServiceLocator();
            return _instance;
        }
    }

    public T GetService<T>()
    {
        if (services.TryGetValue(typeof(T), out var service))
            return (T)service;
        else
            throw new ApplicationException("The requested service is not registered");
    }

    public void RegisterService<T>(T service)
    {
        if (service is UnityEngine.Component component && component.transform.root == component.transform)
        {
            component.transform.parent = null;
            UnityEngine.Object.DontDestroyOnLoad(component.gameObject);
        }
        else if (service is UnityEngine.GameObject gameObject && gameObject.transform.root == gameObject.transform)
        {
            gameObject.transform.parent = null;
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
        }

        services[typeof(T)] = service;
    }
}