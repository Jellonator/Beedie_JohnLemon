using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlane : MonoBehaviour
{
    public GameEnding gameEnding;

    /// Called when a Collider enters this object's trigger
    void OnTriggerEnter(Collider other)
    {
        // check that collider is a lemoning and has not yet been found
        if (other.tag == "Lemoning") {
            gameEnding.DropPlayer();
            other.gameObject.SetActive(false);
        }
    }
}
