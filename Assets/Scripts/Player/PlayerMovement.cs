using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(IPlayerInput))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float gravity = -9.81f;
    [Tooltip("Adjust this if your character's feet sink into the ground or float above it.")]
    [SerializeField] private float yOffset = 0f;

    private CharacterController characterController;
    private IPlayerInput input;
    private Vector3 velocity;
    private AudioSource audioSource;
    private float stepPlayTime = 0;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        input = GetComponent<IPlayerInput>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        MoveHorizontally();
        HandleVerticalPosition();
    }

    private void MoveHorizontally()
    {
        Vector3 direction = (transform.right * input.MoveInput.x) + (transform.forward * input.MoveInput.y);
        characterController.Move(direction * walkSpeed * Time.deltaTime);
        if (!audioSource.isPlaying && direction.magnitude > 0) audioSource.Play();
        if (audioSource.isPlaying &&
            ((audioSource.time >= stepPlayTime && (audioSource.time - stepPlayTime) >= 0.4f) ||
                (audioSource.time < stepPlayTime && (audioSource.time + (audioSource.clip.length - stepPlayTime)) >= 0.4f)))
        {
            audioSource.Pause();
            stepPlayTime = audioSource.time;
        }
    }

    private void HandleVerticalPosition()
    {
        if (Terrain.activeTerrain != null)
        {
            SnapToTerrain();
        }
        else
        {
            ApplyGravityFallback();
        }
    }

    private void SnapToTerrain()
    {
        float terrainHeight = Terrain.activeTerrain.SampleHeight(transform.position)
                            + Terrain.activeTerrain.transform.position.y
                            + yOffset;

        float heightDifference = terrainHeight - transform.position.y;

        characterController.Move(new Vector3(0, heightDifference, 0));
        velocity.y = 0f;
    }

    private void ApplyGravityFallback()
    {
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }
}
