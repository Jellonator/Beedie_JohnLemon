using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEnding : MonoBehaviour
{
    /// Amount of time that it takes for the screen to fade out.
    public float fadeDuration = 1f;
    /// Reference to the player
    public GameObject player;

    /// Called when a Collider enters this object's trigger
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {

        }
    }
}
