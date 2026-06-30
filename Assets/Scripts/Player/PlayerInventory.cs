using UnityEngine;

public class PlayerInventory : MonoBehaviour, IKeyHolder
{

    public bool HasKey { get; private set; }

    public void AddKey()
    {
        HasKey = true;
        Debug.Log("Key added to inventory!");
    }
}
