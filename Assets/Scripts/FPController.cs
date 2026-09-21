using System.Collections;
using System.Collections.Generic;
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

    [Header("Rotation")]
    public float rotatespeed = 3f;

    private CharacterController controller;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector3 velocity;
    private float verticalRotation = 0f;
    private Vector2 inputVector;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        originalMoveSpeed = moveSpeed;
    }
    private void Update()
    {
        HandleMovement();
        HandleLook();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
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

    public void OnRotate(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (heldObject != null)
        {
            //heldObject.transform.rotation += context.ReadValue<Vector2>() * rotatespeed;
            transform.rotation *= Quaternion.Euler(inputVector.y, inputVector.x, 0);
        }
    }
}