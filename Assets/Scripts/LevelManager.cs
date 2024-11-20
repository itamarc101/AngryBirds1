using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private string nextSceneName; // Name of the next scene

    [SerializeField] private string winSound; // Sound to play when the level is completed
    [SerializeField] private float winDelay = 2f; // Delay before transitioning to the next scene

    private int remainingEnemies; // Number of enemies left
    private bool levelCompleted = false;

    private void Start()
    {
        // Initialize remaining enemies based on initial targets
        UpdateRemainingEnemies();
    }

    private void Update()
    {
        // Check if the remaining enemies are all destroyed
        UpdateRemainingEnemies();

        // If no remaining enemies, trigger level completion
        if (remainingEnemies <= 0 && !levelCompleted)
        {
            levelCompleted = true;
            StartCoroutine(HandleWinCondition());
        }
    }

    // Call this method whenever an enemy is destroyed
    public void OnEnemyDestroyed()
    {
        UpdateRemainingEnemies();
    }

    // Update the count of remaining enemies
    private void UpdateRemainingEnemies()
    {
        remainingEnemies = GameObject.FindGameObjectsWithTag("Pig").Length + GameObject.FindGameObjectsWithTag("Wolf").Length;
    }

    private System.Collections.IEnumerator HandleWinCondition()
    {
        // Play the win sound
        AudioManagerGamePlay.Instance.Play(winSound);

        // Wait for the delay
        yield return new WaitForSeconds(winDelay);

        SceneManager.LoadScene(nextSceneName);  
    }
}
