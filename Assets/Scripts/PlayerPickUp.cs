using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    [SerializeField] private Transform holdPoint;       // The empty GameObject where items will sit
    [SerializeField] private LayerMask pickupLayer;     // Set this to your "Pickable" layer
    [SerializeField] private float pickupRange = 3f;    // How far the player can reach

    private GameObject heldObject;                      // Tracks the currently held object
    private Rigidbody heldRigidbody;                    // Tracks the Rigidbody of the held object

    public void OnInteract(InputAction.CallbackContext context)
    {
        // Check for player input
        if (context.performed)
        {
            if (heldObject == null)
            {
                TryPickUpObject();
            }
            else
            {
                DropObject();
            }
        }
    }

    private void TryPickUpObject()
    {
        // Cast a ray from the center of the viewport forward
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupRange, pickupLayer))
        {
            // Valid object found, grab references
            heldObject = hit.collider.gameObject;
            heldRigidbody = heldObject.GetComponent<Rigidbody>();

            if (heldRigidbody != null)
            {
                // Disable physics so it moves cleanly with the player
                heldRigidbody.useGravity = false;           // Prevent it from falling
                heldRigidbody.isKinematic = true;          // Prevent external forces from moving it

                // Snap position and parent it to the hold point
                heldObject.transform.position = holdPoint.position;
                heldObject.transform.rotation = holdPoint.rotation;
                heldObject.transform.SetParent(holdPoint);  // Make it move with the player
            }
        }
    }

    private void DropObject()
    {
        if (heldObject != null && heldRigidbody != null)
        {
            
            heldObject.transform.SetParent(null);

            // Re-enable standard environmental physics
            heldRigidbody.useGravity = true;
            heldRigidbody.isKinematic = false;

            // Clear our references
            heldObject = null;
            heldRigidbody = null;
        }
    }
}
