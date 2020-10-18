using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LemoningController : MonoBehaviour
{
    private NavMeshAgent m_navMeshAgent;

    // Start is called before the first frame update
    void Start()
    {
        m_navMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Activate controller
    public void SetFollow(Vector3 target) {
        m_navMeshAgent.SetDestination(target);
    }
}
