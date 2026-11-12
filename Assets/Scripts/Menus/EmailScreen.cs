using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class EmailScreen : MonoBehaviour
{
    public GameObject Email;

    public void LoadScene(string SceneName)
    {
        SceneManager.LoadScene(SceneName);
    }
}
