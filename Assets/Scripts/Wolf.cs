using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Wolf : MonoBehaviour
{
    [SerializeField] private string soundRandomWolf;
    [SerializeField] private string soundWolfHit;
    [SerializeField] private float timeBeforeDestruction = 2f; // Time before the pig disappears after falling

    private bool isDestroyed = false; // To prevent multiple destruction triggers
    private Animator animator;
    [Tooltip("Referrence to the animator prefab")]
    [SerializeField] private NavMeshAgent navMeshAgent; 

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>(); 
    }


    void OnCollisionEnter(Collision collision)
    {
        // If the collision is with a bird, destroy the pig
        if (collision.collider.CompareTag("Bird"))
        {
            DestroyWolf();
        }
       
    }

    private void DestroyWolf()
    {
        if (isDestroyed) return;
        isDestroyed = true;

        // Stop NavMeshAgent movement
        if (navMeshAgent != null)
        {
            navMeshAgent.isStopped = true;
        }

        animator.SetBool("isDead", true);
        AudioManagerGamePlay.Instance.Play(soundWolfHit);

        
        // Destroy the pig after the specified delay
        Destroy(gameObject, timeBeforeDestruction);
    }

}
