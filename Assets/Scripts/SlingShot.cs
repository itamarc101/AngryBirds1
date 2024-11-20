using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SlingShot : MonoBehaviour
{
    [SerializeField] private string soundTheme;
    [SerializeField] private string soundStretch;
    [SerializeField] private string soundRelease;
    [SerializeField] private string soundClickRed;
    [SerializeField] private string soundClickSparrow;
    [SerializeField] private string soundRedFly;
    [SerializeField] private string soundSparrowFly;


    [SerializeField] private Transform ProjectileRed;   // Reference for the Red projectile
    [SerializeField] private Transform ProjectileSparrow; // Reference for the Sparrow projectile
    [SerializeField] private Transform DrawFrom;
    [SerializeField] private Transform DrawTo;
    [SerializeField] private float adjustHeight = 0.5f;
    [SerializeField] private float shotForce = 15f;
    [SerializeField] private float trajectoryDisp = 15f;
    [SerializeField] private float strengthForce = 2f;
    

    [SerializeField] private int trajectoryResolution = 30; // Number of points in the trajectory

    private Transform currentProjectile;
    private Rigidbody projectileRigidBody;
    private Vector3 aimDirection;
    
    private bool isDragging = false;
    private bool isRed = false;
    private bool isSparrow = false;

    public LineRenderer trajectoryLine; // Line renderer for displaying the trajectory
    public SlingShotStrings slingshotString;

    [SerializeField] private float trajectoryWidth = 0.05f;

    private void PlayThemeSound()
    {
        AudioManagerGamePlay.Instance.Play(soundTheme);
    }
    void Start()
    {
        PlayThemeSound();
        
        // Initialize the trajectory line if not already set in the inspector
        if (trajectoryLine == null)
        {
            trajectoryLine = gameObject.AddComponent<LineRenderer>();
            trajectoryLine.positionCount = trajectoryResolution;
            trajectoryLine.startWidth = trajectoryWidth;
            trajectoryLine.endWidth = trajectoryWidth;
        }
        trajectoryLine.enabled = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Check if clicking on the current projectile
            if (currentProjectile != null && IsClickingOnProjectile())
            {
                AudioManagerGamePlay.Instance.Play(soundStretch);
                StartDragging();
            }
        }

        if (isDragging)
        {
            DragProjectile();
            DisplayTrajectory(trajectoryDisp); // Update trajectory display while dragging
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (isDragging)
            {
                AudioManagerGamePlay.Instance.Play(soundRelease);
                ReleaseAndShoot(shotForce); 
                trajectoryLine.enabled = false; // Disable trajectory after launch
            }
        }
    }

    public void SelectRedProjectile()
    {
        isRed = true;
        AudioManagerGamePlay.Instance.Play(soundClickRed);
        SpawnProjectile(ProjectileRed);
    }

    public void SelectSparrowProjectile()
    {
        isSparrow = true;
        AudioManagerGamePlay.Instance.Play(soundClickSparrow);
        SpawnProjectile(ProjectileSparrow);
    }

    private void SpawnProjectile(Transform projectilePrefab)
    {
        // Destroy the current projectile if one exists
        if (currentProjectile != null)
        {
            Destroy(currentProjectile.gameObject);
        }

        // Instantiate the selected projectile at the DrawFrom position
        currentProjectile = Instantiate(projectilePrefab, DrawFrom.position, Quaternion.identity, transform);
        projectileRigidBody = currentProjectile.GetComponent<Rigidbody>();
        projectileRigidBody.isKinematic = true; // Prevent physics until release
        slingshotString.pointCenter = currentProjectile.transform;
    }

    private bool IsClickingOnProjectile()
    {
        // Raycast from the mouse position to check if it hits the projectile
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform == currentProjectile)
            {
                return true; // Clicked on the current projectile
            }
        }
        return false; // Not clicking on the projectile
    }

    private void StartDragging()
    {
        isDragging = true;
        trajectoryLine.enabled = true; // Enable trajectory display while dragging
    }

    private void DragProjectile()
    {
        Vector3 mousePosition = Input.mousePosition; // Current position of the mouse
        // Using drawFrom z axis to get the world space
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Camera.main.WorldToScreenPoint(DrawFrom.position).z));

        Vector3 dragVector = worldPosition - DrawFrom.position;  // Drag vector - how far the projectile will be pulled back
        
        // Distance along the x,y plane
        dragVector.z = Mathf.Clamp(dragVector.y, -Vector3.Distance(DrawFrom.position, DrawTo.position), 0f);

        currentProjectile.position = DrawFrom.position + dragVector; //Updates the projectile position based on the calculated vector
        // Aiming in the opposite direction of instantiation
        aimDirection = -dragVector.normalized;
        slingshotString.Update(); //Reset the string in the slingshot 
    }

    private void ReleaseAndShoot(float shotForce)
    {
        isDragging = false;
        currentProjectile.parent = null; // Releasing the projectile from the slingshot
        projectileRigidBody.isKinematic = false; // Use physics

        Bird bird = currentProjectile.GetComponent<Bird>();
        if (bird != null)
        {
            bird.Launch();
        }


        aimDirection.z = Mathf.Abs(aimDirection.z);
        aimDirection.y -= adjustHeight;

        float drawStrength = Vector3.Distance(DrawFrom.position, currentProjectile.position) / Vector3.Distance(DrawFrom.position, DrawTo.position);
        projectileRigidBody.AddForce(aimDirection * (shotForce * strengthForce) * drawStrength, ForceMode.Impulse);

        slingshotString.pointCenter = DrawFrom;
        slingshotString.Update();
        currentProjectile = null;

        if (isRed)  AudioManagerGamePlay.Instance.Play(soundRedFly);
        if (isSparrow)  AudioManagerGamePlay.Instance.Play(soundSparrowFly);

        ResetProjectileFlags();

    }

    private void DisplayTrajectory(float shotForce)
    {
        // Calculate the initial velocity of the projectile using the shot force and its mass.
        Vector3 velocity = aimDirection * (shotForce / projectileRigidBody.mass);
        Vector3 position = currentProjectile.position;   // Start the trajectory at the current projectile's position.
        Vector3 nextPosition; // Placeholder for the next predicted position.
        float overlap; // Small buffer distance to ensure raycast overlap

        trajectoryLine.positionCount = trajectoryResolution;     // Set the number of points in the trajectory line to match the resolution.

        // Render the first point of the trajectory
        trajectoryLine.SetPosition(0, position);

        // Iterate through each point to calculate the trajectory's path
        for (int i = 1; i < trajectoryResolution; i++)
        {
            // Calculate the new velocity considering drag
            velocity = CalculateNewVelocity(velocity, projectileRigidBody.drag, 0.1f);

            // Estimate the next position based on velocity and time step
            nextPosition = position + velocity * 0.1f;

            // Determine the overlap distance for raycasting
            overlap = Vector3.Distance(position, nextPosition) * 0.9f;

            // Check for collisions
            if (Physics.Raycast(position, velocity.normalized, out RaycastHit hit, overlap))
            {
                trajectoryLine.positionCount = i + 1; // Adjust the line's point count to stop here
                trajectoryLine.SetPosition(i, hit.point); // Add the hit point to the trajectory
                return; // Stop drawing the trajectory
            }

            // If no collision, continue rendering the trajectory
            position = nextPosition;
            trajectoryLine.SetPosition(i, position);
        }
    }

    // Calculate the updated velocity after applying drag and gravity effects
    private Vector3 CalculateNewVelocity(Vector3 velocity, float drag, float timeStep)
    {
        // Apply drag and gravity to the velocity
        // Simulate the drag by reducing velocity to the drag coefficient.
        Vector3 dragEffect = velocity * (1 - drag * timeStep); // Simulate drag
        // Apply the effect of gravity,calcualted by the time step.
        Vector3 gravityEffect = Physics.gravity * timeStep;   // Simulate gravity
        // Receiving the new velocity
        return dragEffect + gravityEffect;
    }


    // Method to reset the flags after projectile release
    private void ResetProjectileFlags()
    {
        isRed = false; // Reset Red projectile flag
        isSparrow = false; // Reset Sparrow projectile flag
    }
}
