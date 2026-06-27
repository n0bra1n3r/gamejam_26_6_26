using UnityEngine;
using TMPro;

[RequireComponent(typeof(Collider))]
public class EnemyCollision : MonoBehaviour
{
    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private string playerTag = "Player";

    private float _startTime;

    private void Start()
    {
        _startTime = Time.time;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag(playerTag))
            return;

        float survivalTime = Time.time - _startTime;

        if (scoreText != null)
            scoreText.text = $"Time Survived\n{survivalTime:F2} seconds";

        gameOverCanvas?.SetActive(true);
    }
}
