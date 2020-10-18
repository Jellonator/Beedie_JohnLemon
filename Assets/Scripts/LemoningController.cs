using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LemoningController : MonoBehaviour
{
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
    private int m_path_index = 0;
    /// True if the player is currently following the path
    private bool m_path_following = false;
    /// Amount of distance to stop for
    private float m_stopping_distance = 0.0f;

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
        if (!m_path_following) {
            return 0.0f;
        }
        float sum = 0.0f;
        Vector3 pos = transform.position;
        for (int i = m_path_index; i < m_path.corners.Length; i++) {
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
            return m_path_following;
        }
    }

    void FixedUpdate()
    {
        if (m_isFollowing) {
            // Try to pathfind to the character that they are following
            NavMesh.CalculatePath(transform.position, m_following.transform.position, NavMesh.AllAreas, m_path);
            m_path_index = 0;
            m_path_following = true;
        }
        if (GetRemainingPathDistance() <= m_stopping_distance) {
            // If player should stop, then try to stop.
            if (m_isFollowing) {
                // only stop if the followed character is no longer moving
                if (!m_following.GetComponent<LemoningController>().IsMoving()) {
                    m_isFollowing = false;
                    m_path_following = false;
                }
            } else {
                m_path_following = false;
            }
        }
        // Player is walking if they are following a path and there is still some distance to cover
        m_Animator.SetBool("IsWalking", m_path_following && GetRemainingPathDistance() > m_stopping_distance);
    }

    void OnAnimatorMove()
    {
        if (m_path_following) {
            // Determine the amount of distance to cover.
            // Add a little extra beyond the stopping distance so that the player can cross into
            // the stopping zone.
            float remaining = m_Animator.deltaPosition.magnitude;
            remaining = Mathf.Min(remaining, 1e-2f + GetRemainingPathDistance() - m_stopping_distance);
            bool did_move = false;
            Vector3 initialPosition = m_Rigidbody.position;
            // Try to move the player to the next corner in the path.
            // If the player succeeds, then reduce 'remaining' and try it again
            while (m_path_index < m_path.corners.Length && remaining > 0.0f) {
                did_move = true;
                Vector3 current = m_Rigidbody.position;
                Vector3 next = m_path.corners[m_path_index];
                float distance = Vector3.Distance(current, next);
                if (distance <= remaining) {
                    // if the player can move the full distance to this corner, then just move the player to the corner.
                    m_path_index += 1;
                    remaining -= distance;
                    m_Rigidbody.MovePosition(next);
                } else {
                    // interpolate the player to the next corner based on 'remaining'
                    m_Rigidbody.MovePosition(m_Rigidbody.position + (next - current) * (remaining / distance));
                    remaining = 0.0f;
                }
            }
            if (did_move) {
                Vector3 diff = m_Rigidbody.position - initialPosition;
                if (diff.magnitude > 1e-2) {
                    // if the player did move, then rotate the player
                    m_Rigidbody.MoveRotation(Quaternion.LookRotation(diff.normalized, Vector3.up));
                }
            } 
        }
    }

    /// Tell controller to go to a destination
    public void SetDestination(Vector3 target)
    {
        m_path_index = 0;
        NavMesh.CalculatePath(transform.position, target, NavMesh.AllAreas, m_path);
        m_isFollowing = false;
        m_stopping_distance = 0.2f;
        m_path_following = true;
    }

    /// Tell controller to follow another lemoning
    public void SetFollow(GameObject target)
    {
        m_following = target;
        m_isFollowing = true;
        m_stopping_distance = 1.0f;
    }
}
