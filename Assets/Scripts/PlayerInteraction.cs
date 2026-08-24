using UnityEngine;
using TMPro; 

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactionDistance = 3.0f;
    public KeyCode pickupKey = KeyCode.E;

    [Header("UI References")]
    public GameObject descriptionPanel;
    public TextMeshProUGUI descriptionText;

    private Camera playerCamera;

    void Start()
    {
        // Automatically grabs the main camera (attached to the player object)
        playerCamera = Camera.main;
    }

    void Update()
    {
        // Perform a raycast from the center of the viewport
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            // Check if the object has the Item component
            Item item = hit.collider.GetComponent<Item>();

            if (item != null)
            {
                

                if (Input.GetKeyDown(pickupKey))
                {
                    DisplayDescription(item);
                    item.OnPickedUp();
                }
            }
        }
    }

    void DisplayDescription(Item item)
    {
        // Enable the panel 
        descriptionPanel.SetActive(true);
        
        Invoke(nameof(HideDescription), 5.0f);
    }

    void HideDescription()
    {
        descriptionPanel.SetActive(false);
    }
}
