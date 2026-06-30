using UnityEngine;

public class EnemyAware : MonoBehaviour
{
    [SerializeField] private string expectedArrowName = "Arrow(Clone)";

    private MoveTo movementScript;
    private bool isAware = false;

    private void Start()
    {
        // Grab the MoveTo script attached to this same GameObject
        movementScript = GetComponent<MoveTo>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.name.Contains(expectedArrowName)) return;

        if (!isAware)
        {
            TriggerAwareness(other.gameObject);
        }
    }

    private void TriggerAwareness(GameObject arrow)
    {
        isAware = true;
        Debug.Log($"{gameObject.name} heard the arrow fly by!");

        // Tell the movement script to speed up
        if (movementScript != null)
        {
            movementScript.SetAlertState();
        }

        // You could also add other logic here later, like:
        // anim.SetTrigger("Surprised");
        // audioSource.PlayOneShot(alertSound);
    }
}
