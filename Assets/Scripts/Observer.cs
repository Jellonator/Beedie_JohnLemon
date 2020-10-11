using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Observer : MonoBehaviour
{
    /// Reference to the player's transform
    public Transform player;
    /// Reference to the game ending class
    public GameEnding gameEnding;
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

    void FixedUpdate()
    {
        if (m_IsPlayerInRange) {
            Vector3 direction = (player.position + Vector3.up) - transform.position;
            Ray ray = new Ray(transform.position, direction);
            RaycastHit raycastHit;
            if (Physics.Raycast(ray, out raycastHit)) {
                if (raycastHit.collider.transform == player) {
                    gameEnding.CatchPlayer();
                }
            }
        }
    }
}
