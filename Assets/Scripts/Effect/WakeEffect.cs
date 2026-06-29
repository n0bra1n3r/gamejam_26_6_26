using UnityEngine;
using System.Collections.Generic;

[ExecuteAlways]
public class WakeEffect : MonoBehaviour, IEffect
{
    public List<WorldSegment> WorldSegments { get; set; }
    public float Radius { get { return _Radius; } set { _Radius = value; } }
    public float _Radius = 4;

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

        if (WorldSegments == null || WorldSegments.Count == 0) return;

        List<Vector4> startPosArray = new List<Vector4>();
        List<Vector4> endPosArray = new List<Vector4>();
        List<float> radiusArray = new List<float>();

        int wakeCount = Mathf.Min(WorldSegments.Count, 8);

        for (int i = 0; i < wakeCount; i++)
        {
            float radius = WorldSegments[i].Radius;
            radius = radius >= 0 ? radius : _Radius;
            Vector3 endPos = WorldSegments[i].End;
            endPos.y = -radius;
            Vector3 startPos = WorldSegments[i].Start;
            startPos.y = 0;
            startPosArray.Add(new Vector4(startPos.x, startPos.y, startPos.z, 1));
            endPosArray.Add(new Vector4(endPos.x, endPos.y, endPos.z, 1));
            radiusArray.Add(radius);
        }

        while (startPosArray.Count < 8)
        {
            startPosArray.Add(Vector4.zero);
            endPosArray.Add(Vector4.zero);
            radiusArray.Add(0f);
        }

        Shader.SetGlobalVectorArray($"_WakeShader_StartWorldPos", startPosArray);
        Shader.SetGlobalVectorArray($"_WakeShader_EndWorldPos", endPosArray);
        Shader.SetGlobalFloatArray($"_WakeShader_Radius", radiusArray);
        Shader.SetGlobalInt("_WakeShader_WakeCount", wakeCount);
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
