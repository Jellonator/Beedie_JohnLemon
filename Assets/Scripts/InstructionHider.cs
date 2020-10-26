using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstructionHider : MonoBehaviour
{
    public CanvasGroup instructions;

    /// Called when a Collider enters this object's trigger
    void OnTriggerEnter(Collider other)
    {
        // check that collider is the player object, and that the state is 'playing'
        if (other.tag == "Lemoning") {
            instructions.alpha = 0.0f;
        }
    }
}
