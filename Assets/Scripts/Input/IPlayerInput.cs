using System;
using UnityEngine;

public interface IPlayerInput
{
    Vector2 MoveInput { get; }
    Vector2 LookInput { get; }
}
