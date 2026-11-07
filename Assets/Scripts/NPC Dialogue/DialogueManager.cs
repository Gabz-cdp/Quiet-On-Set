using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueTextUI;

    private string[] currentLines;
    private int currentLineIndex = 0;

    // Public property so the player script knows if a conversation is happening
    public bool IsDialogueActive { get; private set; }

    public int frameOpened;


    [Header("Timing")]
    public float inputCooldown = 1f; // Minimum time required between line changes
    public float nextAllowedInputTime;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        //dialoguePanel.SetActive(false);
    }

    public void StartDialogue(string[] lines)
    {
        currentLines = lines;
        currentLineIndex = 0;
        IsDialogueActive = true;
        //dialoguePanel.SetActive(true);

        frameOpened = Time.frameCount;

        DisplayCurrentLine();
    }

    public void AdvanceDialogue()
    {
        // If the player presses E before the cooldown is over, do nothing
        if (Time.time < nextAllowedInputTime) return;

        currentLineIndex++;

        if (currentLineIndex >= currentLines.Length)
        {
            CloseDialogue();
        }
        else
        {
            // Reset the timer for the next line
            nextAllowedInputTime = Time.time + inputCooldown;
            DisplayCurrentLine();
        }
    }

    private void DisplayCurrentLine()
    {
        dialogueTextUI.text = currentLines[currentLineIndex];
    }

    public void CloseDialogue()
    {
        IsDialogueActive = false;
        //dialoguePanel.SetActive(false);
    }
}

