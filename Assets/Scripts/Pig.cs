using UnityEngine;
using System.Collections;

public class Pig : MonoBehaviour
{
    [SerializeField] private string soundRandomPig;
    [SerializeField] private string soundPigHit;
    private float previousYPosition; // To track the previous Y position of the pig
    [SerializeField] private float fallDistanceThreshold = 2f; // The distance threshold to check for falling
    [SerializeField] private float timeBeforeDestruction = 2f; // Time before the pig disappears after falling
    [SerializeField] private float velocityThreshold = 1f; // The velocity magnitude threshold for pig destruction
    [SerializeField] private float coolDownSound = 2.5f; 

    private Rigidbody pigRigidbody; // The pig's Rigidbody for velocity checks
    private bool isDestroyed = false; // To prevent multiple destruction triggers
    [SerializeField] private int hitPoints = 1; // Number of hits before destruction, exposed in inspector


    void Start()
    {
        previousYPosition = transform.position.y; // Initialize with current Y position at the start
        pigRigidbody = GetComponent<Rigidbody>(); // Get the Rigidbody component of the pig
        StartCoroutine(PlayRandomPigSound());

    }

    void OnCollisionEnter(Collision collision)
    {
        // If the collision is with a bird, destroy the pig
        if (collision.collider.CompareTag("Bird"))
        {
            DecreaseHitPoints();
        }
    }

    void Update()
    {
        // Check if the pig's Y position has decreased by the specified amount
        if (transform.position.y < previousYPosition - fallDistanceThreshold)
        {
            // The pig has fallen the specified distance
            DestroyPig(); // Destroy it after the delay
        }

        // Check if the pig's velocity magnitude exceeds the threshold
        if (pigRigidbody.velocity.magnitude > velocityThreshold)
        {
            DestroyPig(); // Destroy the pig if the velocity exceeds the threshold
        }

        // Update the previous Y position for the next frame
        previousYPosition = transform.position.y;
    }

    private void DecreaseHitPoints()
    {
        // Decrease the pig's hit points by 1 on each hit
        hitPoints--;

        // Play the pig hit sound
        AudioManagerGamePlay.Instance.Play(soundPigHit);

        // If the pig's HP is 0, destroy it
        if (hitPoints <= 0)
        {
            DestroyPig();
        }
    }


    private void DestroyPig()
    {
        if (isDestroyed) return;
        isDestroyed = true;
        AudioManagerGamePlay.Instance.Play(soundPigHit);

        
        // Destroy the pig after the specified delay
        Destroy(gameObject, timeBeforeDestruction);
    }

    private IEnumerator PlayRandomPigSound()
    {
        while (!isDestroyed) // Keep playing sounds until the pig is destroyed
        {
            AudioManagerGamePlay.Instance.Play(soundRandomPig); // Play the random pig sound
            yield return new WaitForSeconds(coolDownSound); // Wait for some seconds before the next sound
        }
    }
}
