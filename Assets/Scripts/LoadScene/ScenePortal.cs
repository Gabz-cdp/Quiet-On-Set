using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenePortal : MonoBehaviour
{
    [Tooltip("EndScene")]
    public string sceneToLoad;

    public void TriggerSceneChange()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            //Debug.LogWarning("EndScene is empty on " + gameObject.name);
        }
    }
}
