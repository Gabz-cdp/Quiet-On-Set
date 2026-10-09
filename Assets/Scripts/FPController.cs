using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class FPController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float gravity = -9.81f;

    [Header("Look Settings")]
    public Transform cameraTransform;
    public float lookSensitivity = 2f;
    public float verticalLookLimit = 90f;

    [Header("Reveal Setting")]
    public float CameraUpSpeed = 3f;
    private float originalMoveSpeed;
    public GameObject[] hiddenObjects;


    [Header("Crouch Settings")]
    public float crouchHeight = 1f;
    public float standHeight = 2f;
    public float crouchSpeed = 2.5f;

    [Header("Pickup Settings")]
    public float pickupRange = 3f;
    public Transform holdPoint;
    private PickUpObject heldObject;
    public TMP_Text pickupText; //Pickup text
    public GameObject circleHair; //Crosshair trigger to show when an object/door is in range

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

    [Header("Throw Settings")]
    public float throwForce = 10f;
    public float throwUpwardBoost = 1f;

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
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        originalMoveSpeed = moveSpeed;

        //Clears crosshairs that are set to raycast at the start of the game
        circleHair.gameObject.SetActive(false);
        OpenDoorCrossHair.gameObject.SetActive(false);
        CloseDoorCrossHair.gameObject.SetActive(false);

    }
    private void Update()
    {
        HandleMovement();
        HandleLook();

        if (heldObject != null)
        {
            heldObject.MoveToHoldPoint(holdPoint.position);
        }

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
        if (Physics.Raycast(ray, out hit, OpenDoorRange))
        {
            //Play the "DoorOpen" animation if the object is a door
            Animator doorAnimator = hit.collider.GetComponentInParent<Animator>();
            if (doorAnimator != null)
            {
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
        OpenDoorCrossHair.gameObject.SetActive(false); //disables the crosshair for the door*/
        
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, pickupRange))
            {
                //Check if object is a door
                if (hit.collider.CompareTag("Door"))
                {
                    //Play the "DoorOpen" animation if the object is a door
                    Animator doorAnimator = hit.collider.GetComponentInParent<Animator>();
                    //bool IsOpen = doorAnimator.GetBool("IsOpen");
                    
                    if (doorAnimator != null)
                    {
                        doorAnimator.SetTrigger("DoorOpen");
                        OpenDoorText.text = "Open Door [E]";
                        OpenDoorCrossHair.gameObject.SetActive(true);
                        return;
                    }
                    else
                    {
                        doorAnimator.SetTrigger("DoorClose");
                        CloseDoorText.text = "Close Door [E]";
                        CloseDoorCrossHair.gameObject.SetActive(true);
                        return;
                    }

                    /*if (IsOpen)
                    {
                        doorAnimator.SetTrigger("DoorClose");
                        doorAnimator.SetBool("IsOpen", false);
                        CloseDoorText.text = "Close Door [E]";
                        CloseDoorCrossHair.gameObject.SetActive(true);
                        return;
                    }
                    else
                    {
                        doorAnimator.SetTrigger("DoorOpen");
                        doorAnimator.SetBool("IsOpen", true);
                    }*/
                }
            }
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        if(moveSpeed > 0)
        {
            StartBobbing();
        }
        else if(moveSpeed <= 0)
        {
            StopBobbing();
        }
        /*if (context.performed)
        {
            StopBobbing();
        }*/
    }

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
        if (context.performed)
        {
            foreach (GameObject hiddenObject in hiddenObjects) 
            {
                hiddenObject.SetActive(true);
            }
            moveSpeed = CameraUpSpeed;
        }

        if (context.canceled)
        {
            foreach (GameObject hiddenObject in hiddenObjects)
            {
                hiddenObject.SetActive(false);
            }
            moveSpeed = originalMoveSpeed;
        }

        StopBobbing();
    }


    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            controller.height = crouchHeight;
            moveSpeed = crouchSpeed;
        }
        else if (context.canceled)
        {
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
                }
            }
        }
        else
        {
            heldObject.Drop();
            heldObject = null;
        }
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

        /*public void OnRotate(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            if (heldObject != null)
            {
                //heldObject.transform.rotation += context.ReadValue<Vector2>() * rotatespeed;
                transform.rotation *= Quaternion.Euler(inputVector.y, inputVector.x, 0);
            }
        }*/

        void StartBobbing()
    {
        Camera.GetComponent<Animator>().Play("HeadBobbing");
    }

    void StopBobbing()
    {
        Camera.GetComponent<Animator>().Play("New State");
    }
}