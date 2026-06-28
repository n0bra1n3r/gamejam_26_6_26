using UnityEngine;

public class EnemyKeyDrop : MonoBehaviour
{
    [SerializeField] private GameObject keyPrefab;
    [SerializeField] private GameObject uiKeyIcon;
    [SerializeField] private string expectedArrowName = "Arrow(Clone)";

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.name.Contains(expectedArrowName)) return;
        HandleKeyDropAndCleanup(collision.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.name.Contains(expectedArrowName)) return;
        HandleKeyDropAndCleanup(other.gameObject);
    }

    private void HandleKeyDropAndCleanup(GameObject arrow)
    {
        Instantiate(keyPrefab, transform.position, Quaternion.identity);
        uiKeyIcon.SetActive(true);
        Destroy(arrow);
        Destroy(gameObject);
    }
}
