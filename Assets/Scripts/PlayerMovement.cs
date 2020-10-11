using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    /// Movement vector
    Vector3 m_Movement = Vector3.zero;
    /// Animator component
    Animator m_Animator;
    /// Turning speed
    public float turnSpeed = 20.0f;
    /// Player's rotation
    Quaternion m_Rotation = Quaternion.identity;

    // Start is called before the first frame update
    void Start()
    {
        m_Animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // get horizontal and vertical axis
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        // set movement
        m_Movement.Set(horizontal, 0f, vertical);
        m_Movement.Normalize();
        // determine if there is any movement
        bool hasInput = !Mathf.Approximately(m_Movement.magnitude, 0f);
        // Set iswalking value in animator
        m_Animator.SetBool("IsWalking", hasInput);
        // Determine forward direction
        Vector3 desiredForward = Vector3.RotateTowards(transform.forward, m_Movement, turnSpeed * Time.deltaTime, 0f);
        // Set player's rotation
        m_Rotation = Quaternion.LookRotation(desiredForward);
    }
}
