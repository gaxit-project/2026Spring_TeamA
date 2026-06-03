using UnityEngine;
using UnityEngine.AI;

public class EnemyStopper : MonoBehaviour
{
    private NavMeshAgent agent;
    private EnemyView view;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponentInParent<NavMeshAgent>();
        view = GetComponentInParent<EnemyView>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Wall") || other.CompareTag("WindowZone"))
        {
            agent.isStopped = true;
            agent.ResetPath();
        }

        if(other.CompareTag("WindowZone"))
        {
            view.SetHearing(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Wall") || other.CompareTag("WindowZone"))
        {
             agent.isStopped = false; 
        }
    }
}
