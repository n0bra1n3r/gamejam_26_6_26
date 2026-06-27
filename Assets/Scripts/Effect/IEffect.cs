using UnityEngine;

public interface IEffect
{
    Vector3 StartWorldPos { get; set; }
    Vector3 EndWorldPos { get; set; }
    float Radius { get; set; }
}
