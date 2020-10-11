﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    /// Turning speed
    public float turnSpeed = 20.0f;
    /// Movement vector
    Vector3 m_Movement = Vector3.zero;
    /// Animator component
    Animator m_Animator;
    /// Player's rotation
    Quaternion m_Rotation = Quaternion.identity;
    /// Player's Rigidbody component
    Rigidbody m_Rigidbody;
    /// Player's footstep audiosource
    AudioSource m_AudioFootsteps;

    // Start is called before the first frame update
    void Start()
    {
        // get components
        m_AudioFootsteps = GetComponent<AudioSource>();
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // get horizontal and vertical axis
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        // set movement
        m_Movement.Set(horizontal, 0f, vertical);
        if (m_Movement.sqrMagnitude > 1f) {
            m_Movement.Normalize();
        }
        // determine if there is any movement
        bool hasInput = !Mathf.Approximately(m_Movement.magnitude, 0f);
        // Set iswalking value in animator
        m_Animator.SetBool("IsWalking", hasInput);
        // Determine forward direction
        Vector3 desiredForward = Vector3.RotateTowards(transform.forward, m_Movement, turnSpeed * Time.deltaTime, 0f);
        // Set player's rotation
        m_Rotation = Quaternion.LookRotation(desiredForward);
        // play footsteps if player is walking
        if (hasInput) {
            if (!m_AudioFootsteps.isPlaying) {
                m_AudioFootsteps.Play();
            }
        } else {
            m_AudioFootsteps.Stop();
        }
    }

    /// Called to move the animation
    void OnAnimatorMove()
    {
        // move position
        m_Rigidbody.MovePosition(m_Rigidbody.position + m_Movement * m_Animator.deltaPosition.magnitude);
        // set rotation
        m_Rigidbody.MoveRotation(m_Rotation);
    }
}
