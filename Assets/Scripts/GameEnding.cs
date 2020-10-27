using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    // public GameObject player;
    /// The canvas group to fade in when the player wins
    public CanvasGroup exitBackgroundImageCanvasGroup;
    /// The canvas group to fade in when the player is caught
    public CanvasGroup caughtBackgroundImageCanvasGroup;
    /// Reference to the exit audio source
    public AudioSource exitAudio;
    /// Reference to the caught audio source
    public AudioSource caughtAudio;
    /// Reference to score text
    public Text scoreText;
    /// The current game ending state
    GameEndingState m_State = GameEndingState.Playing;
    /// Timer to keep track of fade time
    float m_Timer = 0f;
    /// Number of Lemonings in the scene
    int m_totalLemonings = 0;
    /// Number of found lemonings
    HashSet<GameObject> m_foundLemonings;
    /// Number of remaining lemonings
    int m_remainingLemonings = 0;

    void Start()
    {
        m_foundLemonings = new HashSet<GameObject>();
        m_totalLemonings = GameObject.FindGameObjectsWithTag("Lemoning").Length;
        m_remainingLemonings = m_totalLemonings;
        UpdateScoreText();
    }

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

    int GetScore()
    {
        return m_foundLemonings.Count;
    }

    void UpdateScoreText()
    {
        if (m_totalLemonings > m_remainingLemonings) {
            scoreText.text = "Score: " + GetScore() + "/" + m_totalLemonings + " (" + m_remainingLemonings + " remaining)";
        } else {
            scoreText.text = "Score: " + GetScore() + "/" + m_totalLemonings;
        }
    }

    /// Called when a Collider enters this object's trigger
    void OnTriggerEnter(Collider other)
    {
        if (m_State == GameEndingState.Playing) {
            // check that collider is a lemoning and has not yet been found
            if (other.tag == "Lemoning" && !m_foundLemonings.Contains(other.gameObject)) {
                m_foundLemonings.Add(other.gameObject);
                UpdateScoreText();
                if (GetScore() >= m_remainingLemonings) {
                    SetEndingState(GameEndingState.Exit);
                }
            }
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

    /// Call this function to indicate that a player has fallen off of the stage.
    public void DropPlayer()
    {
        if (m_State == GameEndingState.Playing) {
            m_remainingLemonings -= 1;
            UpdateScoreText();
            if (m_remainingLemonings <= 0) {
                SetEndingState(GameEndingState.Caught);
            }
        }
    }
}
