using UnityEngine;

public class DieUponHit : MonoBehaviour
{
    [SerializeField] private string expectedProjectileName = "Arrow(Clone)";
    [SerializeField] private GameObject deathEffectPrefab;

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.name.Contains(expectedProjectileName)) return;
        HandleDeathAndCleanup(collision.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.name.Contains(expectedProjectileName)) return;
        HandleDeathAndCleanup(other.gameObject);
    }

    private void HandleDeathAndCleanup(GameObject projectile)
    {
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

        Destroy(projectile);
        Destroy(gameObject);
    }
}
