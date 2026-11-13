using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class FPController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float gravity = -9.81f;
    public Animator animator;
    public bool isWalking = false;


    [Header("Look Settings")]
    public Transform cameraTransform;
    public float lookSensitivity = 2f;
    public float verticalLookLimit = 90f;

    [Header("Reveal Setting")]
    public float CameraUpSpeed = 3f;
    public float originalMoveSpeed;
    public GameObject[] hiddenObjects;
    public bool bIsInCamera;
    public bool hasCamera = false;
    

    [Header("Crouch Settings")]
    public float crouchHeight = 1f;
    public float standHeight = 2f;
    public float crouchSpeed = 2.5f;
    public bool bIsCrouch;

    [Header("Pickup Settings")]
    public float pickupRange = 3f;
    public Transform holdPoint;
    private PickUpObject heldObject;
    public TMP_Text pickupText; //Pickup text
    public GameObject circleHair; //Crosshair trigger to show when an object is in range

    [Header("Open Door")]
    public TMP_Text OpenDoorText; //Open door text
    public float OpenDoorRange = 5f;
    public GameObject OpenDoorCrossHair;

    [Header("Close Door")]
    public TMP_Text CloseDoorText;
    public GameObject CloseDoorCrossHair;
    //public float CloseDoorRange = 5f;

    //[Header("Rotation")]
    //public float rotatespeed = 3f;

    [Header("Read Object")]
    public float readRange = 3f;
    public TMP_Text readText; //Pickup text
    public GameObject readCrossHair; //Crosshair trigger to show when an object is in range
    public Canvas UICanvas;
    public GameObject BackgroundPanel;
    public GameObject TessaTimeCard;
    public GameObject RubicsCubeTimeCard;
    public GameObject CameraManTimeCard;
    public GameObject MissingGirlTimeCard;

    [Header("Throw Settings")]
    public float throwForce = 10f;
    public float throwUpwardBoost = 1f;

    [Header("NPC Interaction Settings")]
    public float interactionDistance = 5f;
    public LayerMask npcLayer; // Put your NPCs on a specific layer to avoid hitting walls
    public Transform playerCamera;

    [Header("Scene Interaction Settings")]
    public float interactRange = 4f;


    private CharacterController controller;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector3 velocity;
    private float verticalRotation = 0f;
    private Vector2 inputVector;
    public GameObject Camera;


    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;

        originalMoveSpeed = moveSpeed;

        //Clears crosshairs that are set to raycast at the start of the game
        circleHair.gameObject.SetActive(false);
        OpenDoorCrossHair.gameObject.SetActive(false);
        OpenDoorText.enabled = false;
        CloseDoorCrossHair.gameObject.SetActive(false);
        readCrossHair.gameObject.SetActive(false);
        readText.enabled = false;

    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void Update()
    {
        HandleMovement();

        HandleLook();

        if (heldObject != null)
        {
            heldObject.MoveToHoldPoint(holdPoint.position);
        }

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        
        Vector3 moveDirection = (camForward * moveInput.y + camRight * moveInput.x).normalized;

        if (moveDirection.sqrMagnitude > 0.01f)
        {
            controller.Move(moveDirection * moveSpeed * Time.deltaTime);

            
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }

        //==INTERACTABLE OBJECTS==
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange))
        {
            //Checking for the interactable objects to mark that the crosshair is on it
            PickUpObject pickUp = hit.collider.GetComponent<PickUpObject>();
            if (pickUp != null)
            {
                pickupText.text = "pick up [E]"; //can replace to = pickUp.gameObject.name
                circleHair.gameObject.SetActive(true); //crosshair trigger enabled when raycast is on object/door
                return;
            }

        }



        //==DOOR ANIMATION==
        if (Physics.Raycast(ray, out hit, OpenDoorRange))
        {
            //Play the "DoorOpen" animation if the object is a door
            Animator doorAnimator = hit.collider.GetComponentInParent<Animator>();
            //bool IsOpen = doorAnimator.GetBool("IsOpen");

            if (doorAnimator != null)
            {
                OpenDoorText.enabled = true;
                OpenDoorText.text = "Open Door [E]";
                OpenDoorCrossHair.gameObject.SetActive(true);
                return;
            }
            else if (doorAnimator == null)
            {
                OpenDoorText.enabled = false; //disabling the open door text
                OpenDoorCrossHair.gameObject.SetActive(false); //disabling the open door crosshair
                CloseDoorText.text = "Close Door [E]";
                CloseDoorCrossHair.gameObject.SetActive(true);
                return;
            }
        }
       



        //Clear text if not looking at an object
        pickupText.text = ""; //clears text when raycast isnt hitting the object
        circleHair.gameObject.SetActive(false); //disables the crosshair for the object

        //Clear text if not looking at the door
        CloseDoorText.text = "";
        CloseDoorText.gameObject.SetActive(false);

        // Clear text if not looking at the door
        OpenDoorText.text = ""; //clears text when raycast isnt hitting the door
        OpenDoorCrossHair.gameObject.SetActive(false); //disables the crosshair for the door
        
    }


    public void OnNPCInteract(InputAction.CallbackContext context)
    {
        // 1. If dialogue is already open, use E to advance lines
        if (DialogueManager.Instance.IsDialogueActive)
        {
            DialogueManager.Instance.AdvanceDialogue();
            return;
        }

        // If dialogue is not open, use Raycast to look for an NPC
        // Use camera direction for First-Person, or transform.forward for top-down/3D Third-Person
        Vector3 rayOrigin = playerCamera != null ? playerCamera.position : transform.position;
        Vector3 rayDirection = playerCamera != null ? playerCamera.forward : transform.forward;

        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, rayDirection, out hit, interactionDistance, npcLayer))
        {
            // Check if the object we hit has the NPC component
           // if (hit.collider.TryGetComponent<NPC>(out NPC npc))
           if (hit.collider.CompareTag("NPC"))
            {
                //pickupText.text = "Talk press [E]";

                hit.collider.GetComponent<NPC>().TriggerDialogue();
            }
        }
    }

    public void OnSceneInteract(InputAction.CallbackContext context)
    {
        // Use this object's transform if no camera was manually assigned
        Transform cam = playerCamera != null ? playerCamera : transform;
        Ray ray = new Ray(cam.position, cam.forward);

        // Checks everything in front of the camera within interactRange
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange))
        {
            // Checks the hit object or its parent for the ScenePortal script
            ScenePortal portal = hit.collider.GetComponentInParent<ScenePortal>();

            if (portal != null)
            {
                portal.TriggerSceneChange();
            }
        }
    }

    public void ExecuteInteraction()
    {
        // 1. Advance dialogue if it's already open
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive)
        {
            DialogueManager.Instance.AdvanceDialogue();
            return; // Stop here so it doesn't run the raycast code below!
        }

        // 2. Perform raycast ONLY if dialogue wasn't already open
        if (playerCamera == null) return;

        RaycastHit hit;
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, interactionDistance, npcLayer))
        {
            if (hit.collider.TryGetComponent<NPC>(out NPC npc))
            {
                npc.TriggerDialogue();
            }
        }
    }


    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, OpenDoorRange))
            {
                //Check if object is a door
                if (hit.collider.CompareTag("Door"))
                {
                    //Play the "DoorOpen" animation if the object is a door
                    Animator doorAnimator = hit.collider.GetComponentInParent<Animator>();
                    bool isOpen = doorAnimator.GetBool("isOpen");
                    
                    //Switches between open and close animations using "isOpen" flag
                    if (isOpen)
                    {
                        //Front Office Door
                        doorAnimator.SetTrigger("DoorClose");
                        doorAnimator.SetBool("isOpen", false);

                        //Supply Closey
                        doorAnimator.SetTrigger("CloseDoor");
                        doorAnimator.SetBool("isOpen", false);
                    }
                    else
                    {
                        //Front Office Door
                        doorAnimator.SetTrigger("DoorOpen");
                        doorAnimator.SetBool("isOpen", true);

                        //Supply Closey
                        doorAnimator.SetTrigger("OpenDoor");
                        doorAnimator.SetBool("isOpen", true);
                    }
                }
            }



        }


    }

    /*public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        if ((context.performed) && (!bIsInCamera) && (!bIsCrouch))
        {
            StartBobbing();
        }
        else if (context.canceled)
        {
            StopBobbing();
        }
    }*/

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    public void HandleMovement()
    {
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        controller.Move(move * moveSpeed * Time.deltaTime);
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    public void HandleLook()
    {
        float mouseX = lookInput.x * lookSensitivity;
        float mouseY = lookInput.y * lookSensitivity;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, - verticalLookLimit, verticalLookLimit);
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }


    //Camera and Hidden Objects
    public void OnReveal(InputAction.CallbackContext context)
    {
        if (hasCamera == true)
        {
            if (context.performed)
            {
                bIsInCamera = true;
                foreach (GameObject hiddenObject in hiddenObjects)
                {
                    hiddenObject.SetActive(true);
                }
                moveSpeed = CameraUpSpeed;
            }

            if (context.canceled)
            {
                bIsInCamera = false;
                foreach (GameObject hiddenObject in hiddenObjects)
                {
                    hiddenObject.SetActive(false);
                }
                moveSpeed = originalMoveSpeed;
            }

            StopBobbing();

        }
    }


    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            bIsCrouch = true;
            controller.height = crouchHeight;
            moveSpeed = crouchSpeed;
        }
        else if (context.canceled)
        {
            bIsCrouch = true;
            controller.height = standHeight;
            moveSpeed = originalMoveSpeed;
        }
     
        StopBobbing();
    }

    public void OnPickUp(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (heldObject == null)
        {
            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, pickupRange))
            {
                PickUpObject pickUp = hit.collider.GetComponent<PickUpObject>();

                if (pickUp != null)
                {
                    pickUp.PickUp(holdPoint);
                    heldObject = pickUp;
                    if(pickUp.CompareTag("CameraObject"))
                    {
                        hasCamera = true; 
                    }
                }
            }
        }
        else
        {
            heldObject.Drop();
            heldObject = null;
            hasCamera = false;
        }
    }

    public void OnRead(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
            RaycastHit hit;

            //Casts raycast to hit the game object
            if (Physics.Raycast(ray, out hit, readRange))
            {
                //Check if object is a timecard
                if (hit.collider.CompareTag("Read"))
                {
                    //enables the read crosshairs if the object is readable
                    readText.enabled = true;
                    readText.text = "Read [R]";
                    readCrossHair.gameObject.SetActive(true);

                    //Pause the screen when interacting with readable objects
                    if (context.performed)
                    {
                        Debug.Log("Read Objects Working");

                        //disable the crosshair and crosshair text
                        readText.enabled = false;
                        readCrossHair.gameObject.SetActive(false);

                        //actives the UI canvas and the Background panel
                        UICanvas.enabled = true;
                        BackgroundPanel.gameObject.SetActive(true);

                        //==TIME CARDS==
                        if(CameraManTimeCard)
                        {
                            bool currentState = CameraManTimeCard.activeSelf;
                            CameraManTimeCard.SetActive(!currentState);
                            Debug.Log("Camera Man Time Card Working");
                        }

                        if (MissingGirlTimeCard)
                        {
                            bool currentState = MissingGirlTimeCard.activeSelf;
                            MissingGirlTimeCard.SetActive(!currentState);
                            Debug.Log("Camera Man Time Card Working");
                        }

                        if (CameraManTimeCard)
                        {
                            bool currentState = TessaTimeCard.activeSelf;
                            TessaTimeCard.SetActive(!currentState);
                            Debug.Log("Camera Man Time Card Working");
                        }

                        if (RubicsCubeTimeCard)
                        {
                            bool currentState = RubicsCubeTimeCard.activeSelf;
                            RubicsCubeTimeCard.SetActive(!currentState);
                            Debug.Log("Camera Man Time Card Working");
                        }
                    }
                }
            }
        }
    }

    public void OnClose()
    {
        //Unpause the screen when interacting with readable objects
        CameraManTimeCard.SetActive(false);
        MissingGirlTimeCard.SetActive(false);
        TessaTimeCard.SetActive(false);
        RubicsCubeTimeCard.SetActive(false);
    }

    public void OnThrow(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (heldObject == null) return;
        Vector3 dir = cameraTransform.forward;
        Vector3 impulse = dir * throwForce + Vector3.up *
        throwUpwardBoost;
        heldObject.Throw(impulse);
        heldObject = null;
    }

    void StartBobbing()
    {
        Camera.GetComponent<Animator>().Play("HeadBobbing");
        Debug.Log("Start Bobbing");
    }

    public void StopBobbing()
    {
        Camera.GetComponent<Animator>().Play("New State");
        Debug.Log("Stop Bobbing");
    }
}