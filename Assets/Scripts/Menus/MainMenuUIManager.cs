using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MainMenuUIManager : MonoBehaviour
{
    public Camera mainCamera; //assign in inspector
    public float rotationSpeed = 1f;
    private bool isRotating = false;
    public GameObject[] UIElements;

    public void LoadScene(string SceneName)
    {
        SceneManager.LoadScene(SceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }


    public void RotateCameraLeftBy90Degrees()
    {
        if (!isRotating)
        {
            StartCoroutine(RotateCameraCoroutine(90f));
        }
    }

    public void RotateCameraBack()
    {
        if (!isRotating)
        {
            StartCoroutine(RotateCameraCoroutine(270f));
        }
    }

    private IEnumerator RotateCameraCoroutine(float angle)
    {
        foreach (GameObject UIelement in UIElements)
        {
            bool currentState = UIelement.activeSelf;
            UIelement.SetActive(!currentState);
        }

        isRotating = true;

        Quaternion startRotation = mainCamera.transform.rotation; //Initial Rotation
        Quaternion endRotation = startRotation * Quaternion.Euler(0, -angle, 0); //Target Rotation

        float rotationProgress = 0f;
        while (rotationProgress < 1f)
        {
            rotationProgress += Time.deltaTime * (rotationSpeed / angle);
            mainCamera.transform.rotation = Quaternion.Lerp(startRotation, endRotation, rotationProgress);
            yield return null;
        }

        mainCamera.transform.rotation = endRotation;
        isRotating = false;
    }
}
