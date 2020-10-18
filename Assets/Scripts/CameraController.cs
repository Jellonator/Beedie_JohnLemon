﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CameraController : MonoBehaviour
{
    public float cameraSpeed = 10.0f;

    public GameObject pointer;

    private Camera m_camera;

    private MeshRenderer m_pointerRenderer;

    private bool m_isPointing = false;

    // Start is called before the first frame update
    void Start()
    {
        m_camera = GetComponent<Camera>();
        m_pointerRenderer = pointer.GetComponent<MeshRenderer>();
    }

    private GameObject FindNearestLemoning(HashSet<GameObject> lemonings, Vector3 pos) {
        if (lemonings.Count == 0) {
            return null;
        }
        GameObject current = null;
        float distance = 0.0f;
        foreach (GameObject next in lemonings) {
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
        GameObject lemonObj = FindNearestLemoning(lemonings, target);
        if (lemonObj != null) {
            LemoningController lemon = lemonObj.GetComponent<LemoningController>();
            lemon.SetDestination(target);
            lemonings.Remove(lemonObj);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // movement (basically copied from old PlayerMovement)
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(horizontal, 0f, vertical);
        if (movement.sqrMagnitude > 1f) {
            movement.Normalize();
        }
        transform.position += movement * Time.deltaTime * cameraSpeed;
        // Raycast to find solid
        Vector2 mouseScreenPos = Input.mousePosition;
        Vector3 mouseWorldPos = m_camera.ScreenToWorldPoint(
            new Vector3(mouseScreenPos.x, mouseScreenPos.y, 1000.0f)
        );
        // Vector3 direction = transform.forward;
        Ray ray = new Ray(transform.position, mouseWorldPos-transform.position);
        RaycastHit raycastHit;
        if (Physics.Raycast(ray, out raycastHit)) {
            pointer.transform.position = raycastHit.point;
            Vector3 fakeUp = Vector3.Cross(raycastHit.normal, transform.right);
            pointer.transform.LookAt(pointer.transform.position + raycastHit.normal, fakeUp);
            m_pointerRenderer.enabled = true;
            m_isPointing = true;
            if (Input.GetMouseButtonDown(0)) {
                NavMeshHit navMeshHit;
                if (NavMesh.SamplePosition(raycastHit.point, out navMeshHit, 3.0f, NavMesh.AllAreas)) {
                    GameObject[] ls = GameObject.FindGameObjectsWithTag("Lemoning");
                    HashSet<GameObject> lemonings = new HashSet<GameObject>(ls);
                    ProcessFollowerChain(lemonings, navMeshHit.position);
                }
            }
        } else {
            m_pointerRenderer.enabled = false;
            m_isPointing = false;
        }
    }
}
