using UnityEngine;

public class NPC : MonoBehaviour
{
    [TextArea(3, 5)]
    public string[] dialogueLines = new string[]
    {
        "Hhh... hello?",
        "I have been here so long i can't...",
        "I can't talk outside of the TV..."
    };

    public void TriggerDialogue()
    {
        // Pass the whole conversation array to the manager
        DialogueManager.Instance.StartDialogue(dialogueLines);
    }
}

