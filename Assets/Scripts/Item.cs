using UnityEngine;

public class Item : MonoBehaviour
{
    [Header("Item Info")]
    public string itemName;
    [TextArea(3, 5)]
    public string itemDescription;

    public void OnPickedUp()
    {
        // Deactivate the object so it vanishes from the world
        gameObject.SetActive(false);
    }
}
