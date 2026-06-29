using UnityEngine;
using System.Collections.Generic;

public struct WorldSegment
{
    public Vector3 Start;
    public Vector3 End;
    public float Radius;
}

public interface IEffect
{
    List<WorldSegment> WorldSegments { get; set; }
    float Radius { get; set; }
}
