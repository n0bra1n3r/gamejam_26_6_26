using UnityEngine;

[ExecuteAlways]
public class WakeEffect : MonoBehaviour, IEffect
{
    public Vector3 StartWorldPos { get; set; }
    public Vector3 EndWorldPos { get; set; }
    public float Radius { get { return _Radius; } set { _Radius = value; } }

    [SerializeField] private float _Radius = 4;

    public Camera WakeCamera;
    public float DistortionAmplitude = 0.15f;
    public float DistortionFrequency = 10.0f;
    public float DistortionSpeed = 2.0f;

    private RenderTexture wakeTexture;
    private int lastScreenWidth;
    private int lastScreenHeight;

    public void Awake()
    {
        wakeTexture = new RenderTexture(Screen.width, Screen.height, 24);
    }
    void Update()
    {
        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
        {
            ResizeTexture();
        }

        WakeCamera.targetTexture = wakeTexture;

        Vector3 StartPos = StartWorldPos;
        StartPos.y = 0;
        Vector3 EndPos = EndWorldPos;
        EndPos.y = -Radius;

        Shader.SetGlobalVector("_WakeShader_StartWorldPos", StartPos);
        Shader.SetGlobalVector("_WakeShader_EndWorldPos", EndPos);
        Shader.SetGlobalFloat("_WakeShader_Radius", Radius);
        Shader.SetGlobalFloat("_WakeShader_DistortionAmplitude", DistortionAmplitude);
        Shader.SetGlobalFloat("_WakeShader_DistortionFrequency", DistortionFrequency);
        Shader.SetGlobalFloat("_WakeShader_DistortionSpeed", DistortionSpeed);
        Shader.SetGlobalTexture("_WakeShader_WakeTexture", wakeTexture);
    }
    private void ResizeTexture()
    {
        wakeTexture.Release();

        wakeTexture.width = Screen.width;
        wakeTexture.height = Screen.height;

        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;
    }
}
