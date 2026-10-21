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

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        dialoguePanel.SetActive(false);
    }

    public void StartDialogue(string[] lines)
    {
        currentLines = lines;
        currentLineIndex = 0;
        IsDialogueActive = true;
        dialoguePanel.SetActive(true);

        DisplayCurrentLine();
    }

    public void AdvanceDialogue()
    {
        currentLineIndex++;

        // If we ran out of lines, close the UI
        if (currentLineIndex >= currentLines.Length)
        {
            CloseDialogue();
        }
        else
        {
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
        dialoguePanel.SetActive(false);
    }
}

