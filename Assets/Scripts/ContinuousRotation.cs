using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinuousRotation : MonoBehaviour
{
    /// Axis around which this spinner rotates
    public Vector3 axis = Vector3.up;
    /// Speed at which this spinner rotates
    public float speed = 45.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.RotateAround(transform.position, axis, speed * Time.deltaTime);
    }
}
