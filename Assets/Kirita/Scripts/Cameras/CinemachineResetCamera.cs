using MS.Extensions;
using Unity.Cinemachine;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.InputSystem;

[ExecuteAlways]
[AddComponentMenu("Cinemachine/Extensions/Cinemachine Reset Camera")]
[RequireComponent(typeof(CinemachinePanTilt))]
[RequireComponent(typeof(CinemachineInputAxisController))]
public class CinemachineResetCamera : CinemachineExtension
{
    [SerializeField]
    private InputActionReference m_CameraResetInputActionRef;
    [SerializeField]
    private Transform m_Target;
    [SerializeField]
    private float m_ResetTime;
    [SerializeField,Range(0f,1f)]
    private float m_Threshold = float.Epsilon;

    private CinemachinePanTilt m_PanTilt;
    private CinemachineInputAxisController m_AxisController;

    protected override void Awake()
    {
        base.Awake();

        m_PanTilt = GetComponent<CinemachinePanTilt>();
        m_AxisController = GetComponent<CinemachineInputAxisController>();

        if(Application.isPlaying)
        {
            m_CameraResetInputActionRef?.action.Enable();
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        if(Application.isPlaying)
        {
            m_CameraResetInputActionRef.action.AddPhaseCallbacks(OnCameraReset,InputActionExtensions.PHASE.STARTED);
        }
    }

    private void OnDisable()
    {
        if (Application.isPlaying)
        {
            m_CameraResetInputActionRef.action.RemovePhaseCallbacks(OnCameraReset, InputActionExtensions.PHASE.STARTED);
        }
    }

    private void OnCameraReset(InputAction.CallbackContext context)
    {
        if (m_AxisController.enabled)
        {
            m_AxisController.enabled = false;

            m_PanTilt.PanAxis.Center = m_Target.localRotation.eulerAngles.y;

            m_PanTilt.PanAxis.Recentering.Time = m_ResetTime;
            m_PanTilt.TiltAxis.Recentering.Time = m_ResetTime;

            m_PanTilt.PanAxis.Recentering.Wait = 0f;
            m_PanTilt.TiltAxis.Recentering.Wait = 0f;

            m_PanTilt.PanAxis.Recentering.Enabled = true;
            m_PanTilt.TiltAxis.Recentering.Enabled = true;
        }
    }

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage,
        ref CameraState state,
        float deltaTime)
    {

        if (stage != CinemachineCore.Stage.Finalize)
        {
            return;
        }

        if(!m_AxisController.enabled)
        {
            if (ApproximatelyAngles(m_PanTilt.PanAxis.Value, m_PanTilt.PanAxis.Center, m_Threshold) &&
                ApproximatelyAngles(m_PanTilt.TiltAxis.Value, m_PanTilt.TiltAxis.Center, m_Threshold)) 
            {

                m_PanTilt.PanAxis.Recentering.Enabled = false;
                m_PanTilt.TiltAxis.Recentering.Enabled = false;

                m_AxisController.enabled = true;

                Debug.Log($"<color=yellow>Completed {name}</color>");
            }
        }
    }

    private bool ApproximatelyAngles(float angle1, float angle2, float tolerance = float.Epsilon)
    {
        //最短の角度差を計算する
        float delta = Mathf.DeltaAngle(angle1, angle2);

        //差の絶対値が許容誤差以下かチェックする
        return Mathf.Abs(delta) <= tolerance;
    }
}