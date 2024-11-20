using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialDialog : MonoBehaviour
{
    [SerializeField] private string soundIntro;

    [SerializeField] private TextMeshProUGUI dialogText; // Assign the TextMeshProUGUI component
    [SerializeField] private Button nextButton; // Assign the "Next" button
    [SerializeField] private Button playButton; // Assign the "Play" button (hidden initially)
    [SerializeField] private string[] dialogLines; // Add your dialog lines here in the Inspector
    [SerializeField] private string nextSceneName; // Name of the next scene

    private int currentLineIndex = 0;

    private void Start()
    {
        AudioManagerGamePlay.Instance.Play(soundIntro);
        playButton.gameObject.SetActive(false); // Hide the "Play" button initially
        UpdateDialog(); // Show the first line of dialog

        // Assign the Next button click listener
        nextButton.onClick.AddListener(OnNextButtonClicked);
        // Assign the Play button click listener
        playButton.onClick.AddListener(OnPlayButtonClicked);
    }

    private void UpdateDialog()
    {
        // Update the dialog text with the current line
        dialogText.text = dialogLines[currentLineIndex];
    }

    private void OnNextButtonClicked()
    {
        currentLineIndex++;

        if (currentLineIndex < dialogLines.Length)
        {
            UpdateDialog(); // Show the next line
        }
        else
        {
            ShowPlayButton(); // Show the "Play" button when all lines are finished
        }
    }

    private void ShowPlayButton()
    {
        nextButton.gameObject.SetActive(false); // Hide the "Next" button
        playButton.gameObject.SetActive(true); // Show the "Play" button
    }

    private void OnPlayButtonClicked()
    {
        SceneManager.LoadScene(nextSceneName); // Load the next scene
    }
}
