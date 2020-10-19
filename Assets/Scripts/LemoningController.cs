using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LemoningController : MonoBehaviour
{
    /// Amount of time to wait before the controller gives up on path finding
    public float giveUpTime = 0.25f;
    /// Stopping distance when moving to target
    public float stoppingDistanceTarget = 0.2f;
    /// Stopping distance when following another controller
    public float stoppingDistanceFollowing = 1.0f;
    /// Obstacle reference
    private NavMeshObstacle m_Obstacle;
    /// Reference to Animator component
    private Animator m_Animator;
    /// Reference to RigidBody component
    private Rigidbody m_Rigidbody;
    /// True if controller is following another controller
    private bool m_isFollowing = false;
    /// target to follow
    private GameObject m_following = null;
    /// Actual path that the character will attempt to follow
    private NavMeshPath m_path;
    /// The index of the corner that the character will try to follow next
    private int m_pathIndex = 0;
    /// True if the player is currently following the path
    private bool m_isPathFinding = false;
    /// Amount of distance to stop for
    private float m_stoppingDistance = 0.0f;
    /// Amount of time that the player has been unable to move for
    private float m_stoppedTime = 0.0f;
    /// Previous position
    private Vector3 m_previousPosition = Vector3.zero;

    void Start()
    {
        // Create empty path
        m_path = new NavMeshPath();
        // get components
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    /// Get the total distance that the player will have to travel to complete its path
    private float GetRemainingPathDistance() {
        if (!m_isPathFinding) {
            // No remaining distance if not following a path
            return 0.0f;
        }
        float sum = 0.0f;
        // Start with controller position
        Vector3 pos = transform.position;
        for (int i = m_pathIndex; i < m_path.corners.Length; i++) {
            // Add distance between previous point and next point
            sum += Vector3.Distance(pos, m_path.corners[i]);
            pos = m_path.corners[i];
        }
        return sum;
    }

    /// Return true if the character is moving, or plans to move.
    /// A moving character is either pathfinding or following another character.
    public bool IsMoving() {
        if (m_isFollowing) {
            return true;
        } else {
            return m_isPathFinding;
        }
    }

    void FixedUpdate()
    {
        if (m_isFollowing) {
            // Try to pathfind to the character that they are following
            CalculatePathTo(m_following.transform.position);
        }
        if (GetRemainingPathDistance() <= m_stoppingDistance) {
            // If player should stop, then try to stop.
            if (m_isFollowing) {
                // only stop if the followed character is no longer moving
                if (!m_following.GetComponent<LemoningController>().IsMoving()) {
                    Stop();
                }
            } else {
                Stop();
            }
        }
        // Increase stop timer
        m_stoppedTime += Time.deltaTime;
        // Reset stop timer if player moved a little bit last frame
        if (Vector3.Distance(m_previousPosition, m_Rigidbody.position) > Time.deltaTime * 0.1f) {
            m_stoppedTime = 0.0f;
        }
        m_previousPosition = m_Rigidbody.position;
        // Stop controller if they have been stopped for longer than one second
        if (m_stoppedTime >= giveUpTime) {
            Stop();
        }
        // Player is walking if they are following a path
        m_Animator.SetBool("IsWalking", m_isPathFinding);
    }

    void OnAnimatorMove()
    {
        if (m_isPathFinding) {
            // Determine the amount of distance to cover.
            // Add a little extra beyond the stopping distance so that the player can cross into
            // the stopping zone.
            float moveSpeed = m_Animator.deltaPosition.magnitude;
            float remaining = moveSpeed;
            remaining = Mathf.Min(remaining, 1e-2f + GetRemainingPathDistance() - m_stoppingDistance);
            // Some extra book-keeping info
            bool did_move = false;
            Vector3 initialPosition = m_Rigidbody.position;
            // Try to move the player to the next corner in the path.
            // If the player succeeds, then reduce 'remaining' and try it again
            while (m_pathIndex < m_path.corners.Length && remaining > 0.0f) {
                did_move = true;
                Vector3 current = m_Rigidbody.position;
                Vector3 next = m_path.corners[m_pathIndex];
                next.y = current.y;
                float distance = Vector3.Distance(current, next);
                if (distance <= remaining) {
                    // if the player can move the full distance to this corner, then just move the player to the corner.
                    m_pathIndex += 1;
                    remaining -= distance;
                    m_Rigidbody.MovePosition(next);
                } else {
                    // interpolate the player to the next corner based on 'remaining'
                    Vector3 amount = (next - current) * (remaining / distance);
                    m_Rigidbody.MovePosition(m_Rigidbody.position + amount);
                    remaining = 0.0f;
                }
            }
            if (did_move) {
                Vector3 diff = m_Rigidbody.position - initialPosition;
                diff.y = 0.0f;
                // Check that player moved at least 5% of the amount of their movement speed.
                float epsilon = 0.05f * moveSpeed;
                if (diff.magnitude > epsilon) {
                    // if the player did move, then rotate the player
                    m_Rigidbody.MoveRotation(Quaternion.LookRotation(diff.normalized));
                }
            } 
        }
    }

    /// Tell the controller to path to the given target
    private void CalculatePathTo(Vector3 target)
    {
        NavMesh.CalculatePath(transform.position, target, NavMesh.AllAreas, m_path);
        m_isPathFinding = true;
        m_pathIndex = 0;
        m_stoppedTime = 0.0f;
    }

    /// Stop path finding completely
    public void Stop()
    {
        m_isPathFinding = false;
        m_isFollowing = false;
    }

    /// Tell controller to go to a destination
    public void SetDestination(Vector3 target)
    {
        CalculatePathTo(target);
        m_isFollowing = false;
        m_stoppingDistance = stoppingDistanceTarget;
    }

    /// Tell controller to follow another lemoning
    public void SetFollow(GameObject target)
    {
        m_following = target;
        m_isFollowing = true;
        m_stoppingDistance = stoppingDistanceFollowing;
        m_stoppedTime = 0.0f;
    }
}
