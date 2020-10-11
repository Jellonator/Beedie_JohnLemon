using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WaypointPatrol : MonoBehaviour
{
    /// Reference to the NavMeshAgent
    public NavMeshAgent navMeshAgent;
    /// A list of Transforms to use as waypoints.
    public Transform[] waypoints;

    void Start()
    {
        navMeshAgent.SetDestination(waypoints[0].position);
    }

    void Update()
    {
        
    }
}
