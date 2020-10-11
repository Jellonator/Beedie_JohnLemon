using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Observer : MonoBehaviour
{
    /// Reference to the player's transform
    public Transform player;
    /// True if the player has been detected
    bool m_IsPlayerInRange = false;

    /// Called when collider enters trigger
    void OnTriggerEnter(Collider other)
    {
        if (other.transform == player) {
            m_IsPlayerInRange = true;
        }
    }

    /// Called when a collider leaves the trigger
    void OnTriggerExit(Collider other) {
        if (other.transform == player) {
            m_IsPlayerInRange = false;
        }
    }
}
