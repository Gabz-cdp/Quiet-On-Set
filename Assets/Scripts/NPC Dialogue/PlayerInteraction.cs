using UnityEngine;
using UnityEngine.InputSystem;

/*public class PlayerInteraction : MonoBehaviour
{
    [Header("Raycast Settings")]
    public float interactionDistance = 3f;
    public LayerMask npcLayer; // Put your NPCs on a specific layer to avoid hitting walls
    public Transform playerCamera; // Drag your main camera here if first-person, or use transform if 3D third-person

    private PlayerInputActions inputActions;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Player.Interact.performed += OnInteract;
        inputActions.Player.Interact.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Interact.performed -= OnInteract;
        inputActions.Player.Interact.Disable();
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        // 1. If dialogue is already open, use E to advance lines
        if (DialogueManager.Instance.IsDialogueActive)
        {
            DialogueManager.Instance.AdvanceDialogue();
            return;
        }

        // 2. If dialogue is not open, use Raycast to look for an NPC
        // Use camera direction for First-Person, or transform.forward for top-down/3D Third-Person
        Vector3 rayOrigin = playerCamera != null ? playerCamera.position : transform.position;
        Vector3 rayDirection = playerCamera != null ? playerCamera.forward : transform.forward;

        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, rayDirection, out hit, interactionDistance, npcLayer))
        {
            // Check if the object we hit has the NPC component
            if (hit.collider.TryGetComponent<NPC>(out NPC npc))
            {
                npc.TriggerDialogue();
            }
        }
    }

  /*  // Optional: Visualise the raycast in the Scene view for debugging
    private void OnDrawGizmosSelected()
    {
        Vector3 rayOrigin = playerCamera != null ? playerCamera.position : transform.position;
        Vector3 rayDirection = playerCamera != null ? playerCamera.forward : transform.forward;
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(rayOrigin, rayOrigin + rayDirection * interactionDistance);
    }*/
