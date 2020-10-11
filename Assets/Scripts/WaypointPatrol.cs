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
    /// The current waypoint index
    int m_CurrentWaypointIndex = 0;

    void Start()
    {
        if (waypoints.Length > 0) {
            navMeshAgent.SetDestination(waypoints[0].position);
        }
    }

    void FixedUpdate()
    {
        if (waypoints.Length > 1) {
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance) {
                m_CurrentWaypointIndex = (m_CurrentWaypointIndex + 1) % waypoints.Length;
                navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
            }
        }
    }
}
