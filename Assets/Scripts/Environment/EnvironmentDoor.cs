using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

[RequireComponent(typeof(Collider))]
public class DoorCollision : MonoBehaviour
{
    [SerializeField] private GameObject victoryCanvas;
    [SerializeField] private TMP_Text congratulationsText;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private string baseMessage = "Congratulations! Level Cleared!\nCompletion Time: ";
    [SerializeField] private string lockedMessage = "The door is locked. You need a key!";

    [Header("Optional Scene Transition")]
    [SerializeField] private string nextSceneName;
    [SerializeField] private float sceneLoadDelay = 3f;

    private float startTime;
    private bool hasTriggered;

    private void Start()
    {
        startTime = Time.time;
        victoryCanvas.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered || !other.CompareTag(playerTag)) return;

        if (other.TryGetComponent<IKeyHolder>(out var keyHolder) && keyHolder.HasKey)
        {
            hasTriggered = true;
            congratulationsText.text = $"{baseMessage}{(Time.time - startTime):F2} seconds";
            victoryCanvas.SetActive(true);

            if (!string.IsNullOrEmpty(nextSceneName))
            {
                StartCoroutine(TransitionToNextScene());
            }
        }
        else
        {
            congratulationsText.text = lockedMessage;
            victoryCanvas.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!hasTriggered && other.CompareTag(playerTag))
        {
            victoryCanvas.SetActive(false);
        }
    }

    private IEnumerator TransitionToNextScene()
    {
        yield return new WaitForSeconds(sceneLoadDelay);
        SceneManager.LoadScene(nextSceneName);
    }
}
