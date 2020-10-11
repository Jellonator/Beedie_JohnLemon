using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEnding : MonoBehaviour
{
    /// Amount of time that it takes for the screen to fade out.
    public float fadeDuration = 1f;
    /// Reference to the player
    public GameObject player;
    /// True if the player has exited
    bool m_IsPlayerAtExit = false;

    /// Called when a Collider enters this object's trigger
    void OnTriggerEnter(Collider other)
    {
        // check that collider is the player object
        if (other.gameObject == player)
        {
            m_IsPlayerAtExit = true;
        }
    }

    /// Called every frame
    void Update()
    {
        if (m_IsPlayerAtExit) {
            
        }
    }
}
