using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.InputSystem;

public class ReadableObjects : MonoBehaviour
{
    public GameObject[] UIElements;
    public GameObject[] UIControls;
    public GameObject container;

    public void OnRead(InputAction.CallbackContext context)
    {   
        //Pause the screen when interacting with readable objects
        if (context.performed)
        {
            container.SetActive(true);
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void OnClose()
    {
        //Unpause the screen when interacting with readable objects
        container.SetActive(false);
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
