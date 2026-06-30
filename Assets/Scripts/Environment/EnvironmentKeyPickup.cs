using UnityEngine;

[RequireComponent(typeof(Collider))]
public class KeyPickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IKeyHolder>(out var keyHolder))
        {
            if (!keyHolder.HasKey)
            {
                keyHolder.AddKey();
                Destroy(gameObject);
            }
        }
    }
}
