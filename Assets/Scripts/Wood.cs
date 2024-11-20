using UnityEngine;

public class Wood : MonoBehaviour
{
    [SerializeField] private string soundWoodHit;

    [SerializeField] private GameObject WoodShatter;
    [SerializeField] private float timeBeforeDestruct = 2f;
    private bool hasFallen = false;
    private float initialYPosition;  // Store the initial Y position

    void Start()
    {
        // Store the initial Y position of the wood
        initialYPosition = transform.position.y;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the collision is with the bird (or projectile)
        if (collision.collider.CompareTag("Bird"))
        {
            AudioManagerGamePlay.Instance.Play(soundWoodHit); // Play the random pig sound
        }

        // If the collision velocity is enough to break the wood
        if (collision.relativeVelocity.magnitude > 5f)
        {
            DestroyWoodAndProjectile(collision);
        }

    }

     void Update()
    {
        // Only check for falling if it hasn't been marked as fallen
        if (!hasFallen)
        {
            // Check if the wood has fallen below its initial position
            if (transform.position.y < initialYPosition - 0.1f)  // Threshold to prevent immediate fall detection
            {
                hasFallen = true;
                Destroy(gameObject, timeBeforeDestruct);
            }
        }
    }

    private void DestroyWoodAndProjectile(Collision collision)
    {
        // Spawn the wood shatter effect
        GameObject shatter = Instantiate(WoodShatter, transform.position, Quaternion.identity);

        // Destroy the wood after the effect
        Destroy(shatter, 2f);
        Destroy(gameObject);

        // Destroy the projectile if it hits the wood
        if (collision.collider.CompareTag("Bird"))
        {
            Destroy(collision.gameObject, 2f); // Destroy the projectile after 2 seconds
        }
    }
}
