using UnityEngine;
using TMPro;

[RequireComponent(typeof(Collider))]
public class DoorCollision : MonoBehaviour
{
    [SerializeField] private GameObject victoryCanvas;
    [SerializeField] private TMP_Text congratulationsText;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private string baseMessage = "Congratulations! Level Cleared!\nCompletion Time: ";

    private float _startTime;
    private bool _hasTriggered;

    private void Start()
    {
        _startTime = Time.time;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_hasTriggered || !other.CompareTag(playerTag)) return;
        _hasTriggered = true;

        float completionTime = Time.time - _startTime;

        congratulationsText.text = $"{baseMessage}{completionTime:F2} seconds";
        victoryCanvas.SetActive(true);
    }
}
