using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KeySystem //Keeps everyhting related to the Key in the Key System
{
    public class KeyRaycast : MonoBehaviour
    {
        [SerializeField] private int rayLength = 3;
        [SerializeField] private LayerMask layerMaskInteract;
        [SerializeField] private string excludeLayerName = null;

        private KeyItemController raycastedObject;
        [SerializeField] private KeyCode openDoorKey = KeyCode.Mouse0;

        [SerializeField] private Image crosshair = null;
        private bool isCrosshairActive;
        private bool doOnce;

        private string interactableTag = "InteractiveObject";

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
                        CrosshairChange(true);
                    }

                    isCrosshairActive = true;
                    doOnce = true;

                    if (Input.GetKeyDown(openDoorKey)) //Try swap to new input system and get to work?
                    {
                        raycastedObject.ObjectInteraction();
                    }
                }
            }
            else;
            {
                if(isCrosshairActive)
                {
                    CrosshairChange(false);
                    doOnce = false;
                }
            }
        }
        void CrosshairChange(bool on)
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
        }
    }
}
