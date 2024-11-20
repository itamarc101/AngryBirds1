using UnityEngine;

public class BouncingObject : MonoBehaviour
{
    [SerializeField] private float bounceForce = 5f; // The force that makes the object bounce up
    [SerializeField] private float bounceInterval = 1f; // The time interval between each bounce
    private Rigidbody rb; 
    private bool isBouncing = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Start the bouncing process
        if (rb != null)
        {
            isBouncing = true;
            InvokeRepeating("Bounce", 0f, bounceInterval); // Start bouncing immedietly (0f) and bounce interval
        }
        else
        {
            Debug.LogWarning("Rigidbody not found on " + gameObject.name);
        }
    }

    void Bounce()
    {
        if (isBouncing)
        {
            // Apply an upward force (simulate the jump/bounce)
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); // Reset the vertical velocity before applying the bounce
            rb.AddForce(Vector3.up * bounceForce, ForceMode.Impulse); // Apply the force to make the object bounce
        }
    }

    void OnDisable()
    {
        // Stop the bouncing when the object is disabled
        CancelInvoke("Bounce");
    }
}
