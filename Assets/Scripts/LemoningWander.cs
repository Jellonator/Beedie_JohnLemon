using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LemoningWander : MonoBehaviour
{
    public float minimumMoveDistance = 0.5f;

    public float maximumMoveDistance = 2.5f;

    public int moveAttempts = 3;

    public float minimumTimer = 1.0f;

    public float maximumTimer = 5.0f;

    private LemoningController m_Controller;

    private float m_waitTimer = 0.0f;

    private float m_targetWait = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        m_Controller = GetComponent<LemoningController>();
        RandomizeTimer();
        m_waitTimer = 0.0f;
    }

    private void RandomizeTimer()
    {
        m_targetWait = Random.Range(minimumTimer, maximumTimer);
    }

    private void AttemptMove()
    {
        for (int i = 0; i < moveAttempts; i++) {
            // First, pick a point nearby to the player
            Vector2 circle = Random.insideUnitCircle * maximumMoveDistance;
            Vector3 target = transform.position + new Vector3(circle.x, 0.0f, circle.y);
            // Next, find point on Navmesh
            NavMeshHit navMeshHit;
            if (NavMesh.SamplePosition(target, out navMeshHit, 3.0f, NavMesh.AllAreas)) {
                // If point is far enough from player, use it
                if (Vector3.Distance(navMeshHit.position, transform.position) >= minimumMoveDistance) {
                    m_Controller.SetDestination(navMeshHit.position);
                    return;
                }
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (m_Controller.IsMoving()) {
            // if character is moving, reset their timer
            m_waitTimer = 0.0f;
        } else {
            m_waitTimer += Time.deltaTime;
            if (m_waitTimer >= m_targetWait) {
                // randomize and reset timer
                RandomizeTimer();
                m_waitTimer = 0.0f;
                AttemptMove();
            }
        }
    }
}
