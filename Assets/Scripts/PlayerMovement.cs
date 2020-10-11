using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    /// Movement vector
    Vector3 m_Movement = Vector3.zero;
    /// Animator component
    Animator m_Animator;

    // Start is called before the first frame update
    void Start()
    {
        
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
    }
}
