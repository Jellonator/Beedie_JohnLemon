using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEnding : MonoBehaviour
{
    /// Amount of time that it takes for the screen to fade out.
    public float fadeDuration = 1f;
    /// Amount of time that the image should be displayed
    public float displayImageDuration = 1f;
    /// Reference to the player
    public GameObject player;
    /// The canvas group to fade in when the player wins
    public CanvasGroup exitBackgroundImageCanvasGroup;
    /// The canvas group to fade in when the player is caught
    public CanvasGroup caughtBackgroundImageCanvasGroup;
    /// True if the player has exited
    bool m_IsPlayerAtExit = false;
    /// True if the player has been caught
    bool m_IsPlayerCaught = false;
    /// Timer to keep track of fade time
    float m_Timer = 0f;

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
            EndLevel(exitBackgroundImageCanvasGroup);
        } else if (m_IsPlayerCaught) {
            EndLevel(caughtBackgroundImageCanvasGroup);
        }
    }

    void EndLevel(CanvasGroup canvasGroup)
    {
        m_Timer += Time.deltaTime;
        canvasGroup.alpha = Mathf.Clamp(m_Timer / fadeDuration, 0f, 1f);
        if (m_Timer > fadeDuration + displayImageDuration) {
            Application.Quit();
        }
    }
}
