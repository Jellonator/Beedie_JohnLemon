using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LemoningWander : MonoBehaviour
{
    /// The minimum amount of distance that a position must be for a lemoning to wander to it.
    public float minimumMoveDistance = 0.5f;
    /// The maximum amount of distance that a Lemoning will attempt to wander to.
    public float maximumMoveDistance = 2.5f;
    /// The number of positions that a lemoning will attempt to move to.
    public int moveAttempts = 3;
    /// The minimum amount of time that a lemoning will spend waiting to wander.
    public float minimumTimer = 1.0f;
    /// The maximum amount of time that a lemoning will spend waiting to wander.
    public float maximumTimer = 5.0f;
    /// Reference to this character's LemoningController
    private LemoningController m_Controller;
    /// Tracks how long this lemoning has been waiting for
    private float m_waitTimer = 0.0f;
    /// How long a lemoning needs to wait before wandering
    private float m_targetWait = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        m_Controller = GetComponent<LemoningController>();
        RandomizeTimer();
        m_waitTimer = 0.0f;
    }

    /// Randomizes the timer to a new value in range [minimumTimer, maximumTimer]
    private void RandomizeTimer()
    {
        m_targetWait = Random.Range(minimumTimer, maximumTimer);
    }

    /// Attempt to move the lemoning to a new location
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
