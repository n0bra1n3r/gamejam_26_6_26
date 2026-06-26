using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputProvider : MonoBehaviour, IPlayerInput
{
    [SerializeField] private float mouseSensitivity = 0.1f;
    private InputSystem_Actions actions;
    public Vector2 MoveInput => actions.Player.Move.ReadValue<Vector2>().normalized;
    public Vector2 LookInput => actions.Player.Look.ReadValue<Vector2>() * mouseSensitivity;
    public float AttackInput => actions.Player.Attack.ReadValue<float>();
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
