using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LemoningController : MonoBehaviour
{
    // Reference to NavMeshAgent component
    private NavMeshAgent m_navMeshAgent;
    // Reference to Animator component
    private Animator m_Animator;
    // Reference to RigidBody component
    private Rigidbody m_Rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        // get components
        m_navMeshAgent = GetComponent<NavMeshAgent>();
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
        // disable navmeshagent moving character
        m_navMeshAgent.updatePosition = false;
        m_navMeshAgent.updateRotation = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (m_navMeshAgent.remainingDistance <= m_navMeshAgent.stoppingDistance) {
            // stop talking when close to target
            m_Animator.SetBool("IsWalking", false);
        }
    }

    void OnAnimatorMove()
    {
        if (!m_navMeshAgent.isStopped) {
            // get direction to move in
            Vector3 direction = m_navMeshAgent.nextPosition - transform.position;
            direction.y = 0.0f;
            direction.Normalize();
            // move
            m_Rigidbody.MovePosition(m_Rigidbody.position + direction * m_Animator.deltaPosition.magnitude);
            // rotate
            m_Rigidbody.MoveRotation(Quaternion.LookRotation(direction, Vector3.up));
        }
    }

    // Tell controller to follow a target
    public void SetFollow(Vector3 target)
    {
        m_navMeshAgent.SetDestination(target);
        m_Animator.SetBool("IsWalking", true);
    }
}
