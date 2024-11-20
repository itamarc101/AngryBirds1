using UnityEngine;

public class MainCamera : MonoBehaviour
{
    [SerializeField] private GameObject Slingshot; // Reference to the slingshot
    private Vector3 _startPosition;
    private GameObject currentBird; // Holds reference to the currently active bird

    [SerializeField] private float followSpeed = 2.5f; // Speed at which the camera follows
    [SerializeField] private float adjustHight = 10f; // Adjust Z based on bird
    void Start()
    {
        _startPosition = transform.position;
    }

    void Update()
    {
        // Dynamically find the bird if none is set or the current bird becomes inactive
        if (currentBird == null)
        {
            currentBird = FindActiveBird();
        }

        // Determine the target position to follow
        Vector3 targetPosition = currentBird != null
            ? new Vector3(
                transform.position.x, // Keep X constant
                transform.position.y, // Keep Y constant
                Mathf.Max(currentBird.transform.position.z - adjustHight, _startPosition.z)) // Adjust Z based on bird
            : _startPosition; // Default to start position if no bird is found

        // Smoothly move the camera towards the target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSpeed);
    }

    // Dynamically finds the currently active bird in the scene.
    // returns The active bird GameObject, or null if none is found
    private GameObject FindActiveBird()
    {
        GameObject[] birds = GameObject.FindGameObjectsWithTag("Bird");

        foreach (var bird in birds)
        {
            if (bird != null && bird.activeSelf)
            {
                return bird; // Return the first active bird found
            }
        }

        return null; // No active bird found
    }
}
