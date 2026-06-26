using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(IPlayerInput))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float gravity = -9.81f;
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
        ApplyGravity();
        Move();
    }
    private void Move()
    {
        Vector3 direction = transform.right * input.MoveInput.x
                          + transform.forward * input.MoveInput.y;
        characterController.Move(direction * walkSpeed * Time.deltaTime);
        characterController.Move(velocity * Time.deltaTime);
    }
    private void ApplyGravity()
    {
        if (characterController.isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
            return;
        }
        velocity.y += gravity * Time.deltaTime;
    }
}
