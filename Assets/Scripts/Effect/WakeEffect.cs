using UnityEngine;

[ExecuteAlways]
public class WakeEffect : MonoBehaviour, IEffect
{
    public Vector3 StartWorldPos { get; set; }
    public Vector3 EndWorldPos { get; set; }

    public float LightRadius = 1.0f;
    public float DistortionAmplitude = 0.15f;
    public float DistortionFrequency = 10.0f;
    public float DistortionSpeed = 2.0f;

    void Start()
    {
    }
    void Update()
    {
        Shader.SetGlobalVector("_WakeShader_StartWorldPos", StartWorldPos);
        Shader.SetGlobalVector("_WakeShader_EndWorldPos", EndWorldPos);
        Shader.SetGlobalFloat("_WakeShader_LightRadius", LightRadius);
        Shader.SetGlobalFloat("_WakeShader_DistortionAmplitude", DistortionAmplitude);
        Shader.SetGlobalFloat("_WakeShader_DistortionFrequency", DistortionFrequency);
        Shader.SetGlobalFloat("_WakeShader_DistortionSpeed", DistortionSpeed);
    }
}
