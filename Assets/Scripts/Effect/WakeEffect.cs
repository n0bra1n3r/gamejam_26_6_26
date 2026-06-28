using UnityEngine;

[ExecuteAlways]
public class WakeEffect : MonoBehaviour, IEffect
{
    public Vector3 StartWorldPos { get; set; }
    public Vector3 EndWorldPos { get; set; }
    public float Radius { get { return _Radius; } set { _Radius = value; } }

    [SerializeField] private float _Radius = 4;

    public Color Color = new Color(1, 1, 0, 0.1f);
    public float DistortionAmplitude = 0.15f;
    public float DistortionFrequency = 10.0f;
    public float DistortionSpeed = 2.0f;

    void Update()
    {
        Vector3 StartPos = StartWorldPos;
        StartPos.y = 0;
        Vector3 EndPos = EndWorldPos;
        EndPos.y = -Radius;

        Shader.SetGlobalVector("_WakeShader_StartWorldPos", StartPos);
        Shader.SetGlobalVector("_WakeShader_EndWorldPos", EndPos);
        Shader.SetGlobalFloat("_WakeShader_Radius", Radius);
        Shader.SetGlobalVector("_WakeShader_Color", Color);
        Shader.SetGlobalFloat("_WakeShader_DistortionAmplitude", DistortionAmplitude);
        Shader.SetGlobalFloat("_WakeShader_DistortionFrequency", DistortionFrequency);
        Shader.SetGlobalFloat("_WakeShader_DistortionSpeed", DistortionSpeed);
    }
}
