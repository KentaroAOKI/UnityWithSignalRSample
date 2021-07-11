using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.AspNet.SignalR.Client;

public class SignalRManager : MonoBehaviour
{
    private HubConnection _connection;
    private SynchronizationContext _unityThreadContext;
    private List<Vector3> _positionQueue;
    private object _lock;

    void Start()
    {
        // Get the context of the main thread.
        _unityThreadContext = SynchronizationContext.Current;
        InitializeConnection();
        _positionQueue = new List<Vector3>();
        _lock = new object();
    }

    void Update()
    {
        
    }

    private void InitializeConnection()
    {
        // Connect to the SignalR Hub. The following example is a local server.
        _connection = new HubConnection("http://localhost:59251/signalr/hubs");
        var createCubeProxy = _connection.CreateHubProxy("chatHub");
        createCubeProxy.On<string, List<string>>("setEnvironment", SetEnvironment);
        createCubeProxy.On<string, string>("setMessage", SetMessage);
        createCubeProxy.On<string, float, float, float>("setPosition", SetPosition);
        _connection.Start().ContinueWith(x =>
        {
            UnityEngine.Debug.Log(x.Exception?.Message ?? "Connected");
        });
    }

    public void EnqueuePosition(Vector3 position)
    {
        lock(_lock)
        {
            _positionQueue.Add(position);
        }
    }

    public bool DequeuePosition(out Vector3 position)
    {
        bool result = false;
        lock (_lock)
        {
            if(_positionQueue.Count > 0)
            {
                position = _positionQueue[0];
                _positionQueue.RemoveAt(0);
                result = true;
            } else
            {
                position = new Vector3();
            }
        }
        return result;
    }

    private void SetEnvironment(string name, List<string> people)
    {
        // Process on the main thread.
        _unityThreadContext.Post(_ =>
        {
            foreach (string onepeople in people)
                UnityEngine.Debug.Log($"setEnvironment: {name} {onepeople}");
        }, null);
    }
    private void SetMessage(string name, string message)
    {
        // Process on the main thread.
        _unityThreadContext.Post(_ =>
        {
            UnityEngine.Debug.Log($"setMessage: {name} {message}");
        }, null);
    }
    private void SetPosition(string name, float positionX, float positionY, float positionZ)
    {
        // Process on the main thread.
        _unityThreadContext.Post(_ =>
        {
            EnqueuePosition(new Vector3(positionX, positionY, positionZ));
            UnityEngine.Debug.Log($"setPosition: {name} {positionX}, {positionY}, {positionZ}");
        }, null);
    }
}
