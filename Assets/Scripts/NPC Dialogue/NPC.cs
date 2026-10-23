using UnityEngine;

public class NPC : MonoBehaviour
{
    [TextArea(1, 20)]
    public string[] dialogueLines = new string[]
    {
        "Hhh... hello?",
        "I have been here so long i can't...",
        "I can't talk outside of the TV...",
        "I don't have long.. she sees everything",
        "Tessa has gone rougue" 
    };

    public void TriggerDialogue()
    {
        // Pass the whole conversation array to the manager
        DialogueManager.Instance.StartDialogue(dialogueLines);
    }
}

