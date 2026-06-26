using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputProvider : MonoBehaviour, IPlayerInput
{
    [SerializeField] private float mouseSensitivity = 0.1f;
    // We access the `PlayerInputActions` asset we created, go into the `Player` Action Map, and read the value
    // from the`Move` action using `ReadValue<Vector2>()`:
    private InputSystem_Actions actions;
    public Vector2 MoveInput => actions.Player.Move.ReadValue<Vector2>().normalized;
    public Vector2 LookInput => actions.Player.Look.ReadValue<Vector2>() * mouseSensitivity;
    private void Awake()
    {
        actions = new InputSystem_Actions();
    }
    private void OnEnable()
    {
        actions.Enable();
    }
    private void OnDisable()
    {
        actions.Disable();
    }
}
