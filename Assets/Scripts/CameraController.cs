using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CameraController : MonoBehaviour
{
    /// The speed at which to move this camera
    public float cameraSpeed = 10.0f;
    /// An object to use as a pointer
    public GameObject pointer;
    /// The Camera component
    private Camera m_camera;
    /// MeshRenderer component of 'pointer'
    private MeshRenderer m_pointerRenderer;

    // Start is called before the first frame update
    void Start()
    {
        m_camera = GetComponent<Camera>();
        m_pointerRenderer = pointer.GetComponent<MeshRenderer>();
    }

    /// Find the Lemoning in the given set that is closest to 'pos'
    private GameObject FindNearestLemoning(HashSet<GameObject> lemonings, Vector3 pos) {
        if (lemonings.Count == 0) {
            // No lemonings, return null
            return null;
        }
        // default to null
        GameObject current = null;
        float distance = 0.0f;
        foreach (GameObject next in lemonings) {
            // Compare distances to see which is closer
            float nextDistance = Vector3.Distance(pos, next.transform.position);
            if (current == null || nextDistance < distance) {
                distance = nextDistance;
                current = next;
            }
        }
        return current;
    }

    private void ProcessFollowerChain(HashSet<GameObject> lemonings, Vector3 target)
    {
        HashSet<GameObject> leaders = new HashSet<GameObject>();
        // Nearest lemon will follow cursor
        if (lemonings.Count > 0) {
            GameObject lemonObj = FindNearestLemoning(lemonings, target);
            LemoningController lemon = lemonObj.GetComponent<LemoningController>();
            lemon.SetDestination(target);
            lemonings.Remove(lemonObj);
            leaders.Add(lemonObj);
        }
        while (lemonings.Count > 0) {
            // get next nearest lemon
            GameObject lemonObj = FindNearestLemoning(lemonings, target);
            // Find potential leader (one closest to this lemon)
            GameObject leaderObj = FindNearestLemoning(leaders,lemonObj.transform.position);
            // Set lemon to follow
            LemoningController lemon = lemonObj.GetComponent<LemoningController>();
            lemon.SetFollow(leaderObj);
            lemonings.Remove(lemonObj);
            leaders.Add(lemonObj);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // movement (basically copied from old PlayerMovement)
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(horizontal, 0f, vertical);
        if (movement.sqrMagnitude > 1f) {
            movement.Normalize();
        }
        // apply movement
        transform.position += movement * Time.deltaTime * cameraSpeed;
        // Create ray based on mouse position in 3d space
        Vector2 mouseScreenPos = Input.mousePosition;
        Vector3 mouseWorldPos = m_camera.ScreenToWorldPoint(
            new Vector3(mouseScreenPos.x, mouseScreenPos.y, 1000.0f)
        );
        Ray ray = new Ray(transform.position, mouseWorldPos-transform.position);
        // Perform raycast to find a solid object
        RaycastHit raycastHit;
        if (Physics.Raycast(ray, out raycastHit)) {
            // if an object was hit, then show a sphere where the raycast hit
            pointer.transform.position = raycastHit.point;
            m_pointerRenderer.enabled = true;
            if (Input.GetMouseButtonDown(0)) {
                // If player clicked, then find nearest point on navmesh
                NavMeshHit navMeshHit;
                if (NavMesh.SamplePosition(raycastHit.point, out navMeshHit, 3.0f, NavMesh.AllAreas)) {
                    // if there was a nearby point on the navmesh, then make Lemonings follow the path
                    GameObject[] ls = GameObject.FindGameObjectsWithTag("Lemoning");
                    HashSet<GameObject> lemonings = new HashSet<GameObject>(ls);
                    ProcessFollowerChain(lemonings, navMeshHit.position);
                }
            }
        } else {
            m_pointerRenderer.enabled = false;
        }
    }
}
