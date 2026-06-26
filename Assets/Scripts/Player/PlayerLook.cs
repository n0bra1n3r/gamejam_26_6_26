using UnityEngine;
using Unity.Cinemachine;

[RequireComponent(typeof(IPlayerInput))]
public class PlayerLook : MonoBehaviour
{
    [SerializeField] private CinemachinePanTilt panTilt;
    [SerializeField] private float verticalClamp = 85f;
    private IPlayerInput input;
    private float pitch;
    private void Awake()
    {
        input = GetComponent<IPlayerInput>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void Update()
    {
        RotateHorizontal();
        RotateVertical();
    }
    private void RotateHorizontal()
    {
        transform.Rotate(Vector3.up * input.LookInput.x);
    }
    private void RotateVertical()
    {
        pitch -= input.LookInput.y;
        pitch = Mathf.Clamp(pitch, -verticalClamp, verticalClamp);
        panTilt.TiltAxis.Value = pitch;
    }
}
