using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace KeySystem //Keeps everyhting related to the Key in the Key System
{
    public class KeyRaycast : MonoBehaviour
    {
        [SerializeField] private int rayLength = 3;
        [SerializeField] private LayerMask layerMaskInteract;
        [SerializeField] private string excludeLayerName = null;

        private KeyItemController raycastedObject;
        //[SerializeField] private KeyCode openDoorKey = KeyCode.Mouse0;

        [SerializeField] private Image crosshair = null;
        private bool isCrosshairActive;
        private bool doOnce;

        public GameObject OpenDoorCrossHair;


        private string interactableTag = "InteractiveObject";

        private void Awake()
        {
            OpenDoorCrossHair.gameObject.SetActive(false); //disable the open door crosshair
        }

        private void Update()
        {
            RaycastHit hit;
            Vector3 fwd = transform.TransformDirection(Vector3.forward);

            int mask = 1 << LayerMask.NameToLayer(excludeLayerName) | layerMaskInteract.value;

            if (Physics.Raycast(transform.position, fwd, out hit, rayLength, mask))
            {
                if (hit.collider.CompareTag(interactableTag))
                {
                    if (!doOnce)
                    {
                        raycastedObject = hit.collider.gameObject.GetComponent<KeyItemController>();
                        OpenDoorCrossHair.gameObject.SetActive(true);
                        //CrosshairChange(true);
                    }

                    isCrosshairActive = true;
                    doOnce = true;

                    //The key on the keyboard to interact with the keycard and door
                    if (Keyboard.current.eKey.wasPressedThisFrame) //(Keyboard.current[KeyCode.R].wasPressedThisFrame) //(Input.GetKeyDown(openDoorKey)) 
                    {
                        raycastedObject.ObjectInteraction();
                    }
                }
            }
            else;
            {
                if(isCrosshairActive)
                {
                    OpenDoorCrossHair.gameObject.SetActive(false);
                    //CrosshairChange(false);
                    doOnce = false;
                }
            }
        }
        /*void CrosshairChange(bool on)
        {
            if(on && !doOnce)
            {
                crosshair.color = Color.green;
            }
            else
            {
                crosshair.color = Color.white;
                isCrosshairActive = false;
            }
        }*/
    }
}
