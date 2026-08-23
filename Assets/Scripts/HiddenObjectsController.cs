/*
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class HiddenObjectsController : MonoBehaviour
{
    [SerializeField] GameObject hiddenObjectsPrefab;

    public void OnCameraUp(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ToggleObjects();
            //moveSpeed = 3f;
        }
    }

    void ToggleObjects()
    {
        bool currentState = hiddenObjectsPrefab.activeSelf;
        hiddenObjectsPrefab.SetActive(!currentState);
    }
}
*/