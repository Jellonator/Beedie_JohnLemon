using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

enum GameEndingState {
    Playing,
    Exit,
    Caught
}

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
    /// Reference to the exit audio source
    public AudioSource exitAudio;
    /// Reference to the caught audio source
    public AudioSource caughtAudio;
    /// The current game ending state
    GameEndingState m_State = GameEndingState.Playing;
    /// Timer to keep track of fade time
    float m_Timer = 0f;

    /// Set the game ending state
    void SetEndingState(GameEndingState state)
    {
        m_State = state;
        // stop existing audio (if applicaple)
        caughtAudio.Stop();
        exitAudio.Stop();
        // play new audio depending on state
        if (state == GameEndingState.Caught) {
            caughtAudio.Play();
        } else if (state == GameEndingState.Exit) {
            exitAudio.Play();
        }
    }

    /// Called when a Collider enters this object's trigger
    void OnTriggerEnter(Collider other)
    {
        // check that collider is the player object, and that the state is 'playing'
        if (other.gameObject == player && m_State == GameEndingState.Playing)
        {
            SetEndingState(GameEndingState.Exit);
        }
    }

    /// Called every frame
    void FixedUpdate()
    {
        if (m_State == GameEndingState.Exit) {
            // if player beat stage, end level and close game
            EndLevel(exitBackgroundImageCanvasGroup, false);
        } else if (m_State == GameEndingState.Caught) {
            // if player is caught, end level and restart level
            EndLevel(caughtBackgroundImageCanvasGroup, true);
        }
    }

    /// Level ending function. Call every frame when the level should end.
    /// CanvasGroup is the canvasgroup to make visible
    /// doRestart indicates if the level should restart or not.
    void EndLevel(CanvasGroup canvasGroup, bool doRestart)
    {
        // Increase timer
        m_Timer += Time.deltaTime;
        // Set the canvasgroup's alpha based on the timer.
        canvasGroup.alpha = Mathf.Clamp(m_Timer / fadeDuration, 0f, 1f);
        if (m_Timer > fadeDuration + displayImageDuration) {
            // If timer is at end:
            if (doRestart) {
                // restart level
                SceneManager.LoadScene(gameObject.scene.buildIndex);
            } else {
                // quit game
                Application.Quit();
            }
        }
    }

    /// Call this function to indicate that the player has been caught and
    /// that the level should be reset as a result.
    public void CatchPlayer()
    {
        if (m_State == GameEndingState.Playing) {
            // only set if the state is set to 'playing'
            SetEndingState(GameEndingState.Caught);
        }
    }
}
