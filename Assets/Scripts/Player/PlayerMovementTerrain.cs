using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(IPlayerInput))]
public class PlayerMovementTerrain : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float gravity = -9.81f;

    [Tooltip("Adjust this if your character's feet sink into the ground or float above it.")]
    [SerializeField] private float yOffset = 0f;

    private CharacterController characterController;
    private IPlayerInput input;
    private Vector3 velocity;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        input = GetComponent<IPlayerInput>();
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
