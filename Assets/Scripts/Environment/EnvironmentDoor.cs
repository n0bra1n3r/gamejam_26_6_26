using UnityEngine;
using TMPro;

[RequireComponent(typeof(Collider))]
public class DoorCollision : MonoBehaviour
{
    [SerializeField] private GameObject victoryCanvas;
    [SerializeField] private TMP_Text congratulationsText;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private string baseMessage = "Congratulations! Level Cleared!\nCompletion Time: ";
    [SerializeField] private string lockedMessage = "The door is locked. You need a key!";

    private float _startTime;
    private bool _hasTriggered;

    private void Start()
    {
        _startTime = Time.time;
        victoryCanvas.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_hasTriggered || !other.CompareTag(playerTag)) return;

        if (other.TryGetComponent<IKeyHolder>(out var keyHolder) && keyHolder.HasKey)
        {
            _hasTriggered = true;
            congratulationsText.text = $"{baseMessage}{(Time.time - _startTime):F2} seconds";
            victoryCanvas.SetActive(true);
        }
        else
        {
            congratulationsText.text = lockedMessage;
            victoryCanvas.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!_hasTriggered && other.CompareTag(playerTag))
        {
            victoryCanvas.SetActive(false);
        }
    }
}
