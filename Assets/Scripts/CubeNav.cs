using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CubeNav : MonoBehaviour
{
    private NavMeshAgent _agent;
    private SignalRManager _signalrManager;
    public GameObject signalrManagerObject;
    // Start is called before the first frame update
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _signalrManager = signalrManagerObject.GetComponent<SignalRManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(_signalrManager != null)
        {
            Vector3 position;
            if (_signalrManager.DequeuePosition(out position))
            {
                _agent.SetDestination(position);
                Debug.Log(position);
            }
        }
        
    }
}
