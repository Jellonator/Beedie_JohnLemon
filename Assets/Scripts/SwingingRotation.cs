using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingingRotation : MonoBehaviour
{
    /// Time it takes to complete one full swing
    public float swingTime = 1.0f;
    /// Swing axis
    public Vector3 swingAxis = Vector3.up;
    /// Minimum rotation
    public float minimumRotation = -90.0f;
    /// Maximum rotation
    public float maximumRotation = 90.0f;
    /// Timer used to track rotation
    float m_timer = 0.0f;
    // original rotation
    Quaternion m_originalRotation;

    // Start is called before the first frame update
    void Start()
    {
        m_originalRotation = transform.rotation;
    }

    float EaseInOutSine(float x)
    {
        return -(Mathf.Cos(Mathf.PI * x) - 1.0f) / 2.0f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        m_timer += Time.deltaTime / swingTime;
        float x = EaseInOutSine(Mathf.PingPong(m_timer, 1.0f));
        float rotation = x * maximumRotation + (1.0f - x) * minimumRotation;
        transform.rotation = m_originalRotation;
        transform.RotateAround(transform.position, swingAxis, rotation);
    }
}
