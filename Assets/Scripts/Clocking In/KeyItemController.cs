using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KeySystem //Keeps everyhting related to the Key in the Key System
{
    public class KeyItemController : MonoBehaviour
    {
        [SerializeField] private bool FrontOfficeDoor = false;
        [SerializeField] private bool KeyCard = false;

        [SerializeField] private KeyInventory _keyInventory = null;

        private KeyDoorController doorObject;

        private void Start()
        {
            if (FrontOfficeDoor)
            {
                doorObject = GetComponent<KeyDoorController>();
            }
        }

        //To either interact with  the keycard
        public void ObjectInteraction()
        {
            if(FrontOfficeDoor)
            {
                doorObject.PlayAnimation();
            }
            else if(KeyCard)
            {
                _keyInventory.hasKeyCard = true;
                gameObject.SetActive(false);
            }
        }
    }
}
