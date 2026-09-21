using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class ObjectInteract : MonoBehaviour
{
    public GameObject offset; //where the object will be held infront of the player
    public GameObject player;
    private PlayerInput playerInput; //how the player interacts with the surrounding
    private GameObject targetObject;

    public bool isExamining = false; //checks to see that the obejct is being held and examined

    public Canvas objectInteractCanvas; //canvas used to interact and rotate object 
    public Canvas interactCanvas;
    public Canvas HUDCanvas; //main canvas
    public GameObject tableObject; //where the clinder is placed on the table
    private Vector3 lastMousePosition; //checks where the mouse was positioned
    private Transform examinedObject; //used to move the object that is being examined
    private Vector3 mousePosition; //current mouse position
    public bool isHitting; //checks to see that the raycast is hitting the object being examined

    private Rect screenArea = new Rect(UnityEngine.Screen.width / 2 - 600, UnityEngine.Screen.height / 2 - 375, 1200, 750);

    //List of position & rotation of interactable objects
    public Dictionary<Transform, Vector3> originalPositions = new Dictionary<Transform, Vector3>();
    public Dictionary<Transform, Quaternion> originalRotations = new Dictionary<Transform, Quaternion>();

    void Start()
    {
        objectInteractCanvas.enabled = false;
        interactCanvas.enabled = true;
        HUDCanvas.enabled = false;
        targetObject = GameObject.Find("Player");
        playerInput = targetObject.GetComponent<PlayerInput>();
    }

    void Update()
    {
        mousePosition = Mouse.current.position.ReadValue();
        float distance = Vector3.Distance(targetObject.transform.position, tableObject.transform.position);
        //offset.transform.forward = -player.transform.forward;

        if (Keyboard.current.eKey.wasPressedThisFrame) //Rchecks to see that the E key was pressed to interact
        {
            Ray interactRay = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit interactHit;

            if (Physics.Raycast(interactRay, out interactHit))
            {
                if (interactHit.collider.CompareTag("Object"))
                {
                    if (distance < 10f)
                    {
                        isExamining = true;

                        if (isExamining) //stores the object being examined and its original position & rotation
                        {
                            examinedObject = interactHit.transform;
                            originalPositions[examinedObject] = examinedObject.position;
                            originalRotations[examinedObject] = examinedObject.rotation;
                            objectInteractCanvas.enabled = false;
                            interactCanvas.enabled = true;
                            HUDCanvas.enabled = false;
                            Examine(); StartExamination();
                        }
                        else
                        {
                            objectInteractCanvas.enabled = true;
                            interactCanvas.enabled = false;
                            HUDCanvas.enabled = true;
                            NonExamine(); StopExamination();
                        }
                    }
                }
            }
        }
    }

    public void ExitButtonPressed()
    { 
        isExamining = false;
    }

    //When a player starts examining an object, unlocks cursor, makes it visible and stops player from moving
    void StartExamination()
    {
        lastMousePosition = mousePosition;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        playerInput.enabled = false;
    }

    //When the player stops examing an object, it locks the cursor, hides it, and allows the player to move
    void StopExamination()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playerInput.enabled = true;
    }

    //Moves the object picked up (examinedObject) to the offset so the player can rotate the object based on the movement of the mouse
    void Examine()
    {
        if(examinedObject != null)
        {
            if(screenArea.Contains(mousePosition))
            {
                examinedObject.position = Vector3.Lerp(examinedObject.position, offset.transform.position, 0.2f);
                if(Mouse.current.leftButton.isPressed)
                {
                    Vector3 deltaMouse = mousePosition - lastMousePosition;
                    float rotationSpeed = 1.0f;
                    examinedObject.Rotate(deltaMouse.x * rotationSpeed * Vector3.up, Space.World);
                    examinedObject.Rotate(deltaMouse.y * rotationSpeed * Vector3.left, Space.World);
                    lastMousePosition = mousePosition;
                }
            }
        }
    }

    //When the player is no longer examining the object, returns the object to original position and rotation
    void NonExamine()
    {
        if(examinedObject != null)
        {
            if(originalPositions.ContainsKey(examinedObject)) //resets position & rotation of object to original values
            {
                examinedObject.position = Vector3.Lerp(examinedObject.position, originalPositions[examinedObject], 0.2f);
            }
            if(originalRotations.ContainsKey(examinedObject))
            {
                examinedObject.rotation = Quaternion.Slerp(examinedObject.rotation, originalRotations[examinedObject], 0.2f);
            }
        }
    }
}
