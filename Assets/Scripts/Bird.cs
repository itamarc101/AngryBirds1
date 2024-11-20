using UnityEngine;

public class Bird : MonoBehaviour
{
    [SerializeField] private GameObject Feathers; // Feathers object

    [SerializeField] private float disappearTime = 4f; // Time after which the bird disappears if not hitting anything
    [SerializeField] private float coolDown = 2f; // Time after feathers disappear
    private float timer = 0f;
    private bool isLaunched = false; // Tracks whether the bird has been launched
 

    void Update()
    {
        // Increment the timer only if the bird has been launched
        if (isLaunched)
        {
            timer += Time.deltaTime;

            // If the timer exceeds the disappear time, destroy the bird
            if (timer >= disappearTime)
            {
                Destroy(gameObject);
            }
        }
    }
    public void Launch()
    {
        // Called by the slingshot to indicate the bird has been launched
        isLaunched = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        // If the collision isnt with ground, show feathers 
        if (!collision.collider.CompareTag("Ground")) {
            GameObject feathers = Instantiate(Feathers, transform.position, Quaternion.identity);
            Destroy(feathers, coolDown);

        }
    }

}
