using UnityEngine;
using TMPro;

public class Dialogue : MonoBehaviour
{
    [SerializeField] private GameObject InteractPrompt;
    [SerializeField] private GameObject DialogueBox;
    [SerializeField] private TextMeshProUGUI DialogueText;
    [SerializeField] private GameObject NectLinePromptl; 

    private bool isInteracting =false; 
    private bool canInteract = false;


    private void Update()
    {
        InteractPrompt.SetActive(canInteract);
    }


    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            canInteract = true;
            InteractPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            canInteract = false;
            InteractPrompt.SetActive(false);
        }
    }
}
