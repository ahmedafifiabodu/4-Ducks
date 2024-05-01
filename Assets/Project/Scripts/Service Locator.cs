using System;
using System.Collections.Generic;
using System.Diagnostics;

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
        Type serviceType = typeof(T);

        if (services.ContainsKey(serviceType))
        {
            UnityEngine.Debug.LogWarning($"Service of type {serviceType.Name} is already registered.");

            if (service is UnityEngine.Component existingComponent)
            {
                UnityEngine.Object.Destroy(existingComponent.gameObject);
            }
            else if (service is UnityEngine.GameObject existingGameObject)
            {
                UnityEngine.Object.Destroy(existingGameObject);
            }
            return;
        }

        if (service is UnityEngine.Component newComponent)
        {
            if (newComponent.transform.parent != null)
            {
                newComponent.transform.parent = null;
                UnityEngine.Object.DontDestroyOnLoad(newComponent.gameObject);
            }
        }
        else if (service is UnityEngine.GameObject newGameObject)
        {
            if (newGameObject.transform.parent != null)
            {
                newGameObject.transform.parent = null;
                UnityEngine.Object.DontDestroyOnLoad(newGameObject);
            }
        }

        services[serviceType] = service;
    }
}