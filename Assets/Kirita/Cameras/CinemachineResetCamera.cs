using MS.Extensions;
using System;
using System.Runtime.CompilerServices;
using Unity.Burst.CompilerServices;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

[ExecuteAlways]
[AddComponentMenu("Cinemachine/Extensions/Cinemachine Reset Camera")]
public class CinemachineResetCamera : CinemachineExtension
{
    [SerializeField]
    private InputActionReference m_CameraResetInputActionRef;
    [SerializeField]
    private Transform m_Target;
    [SerializeField]
    private float m_ResetTime;

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
            float roundPanValue = Mathf.Round(m_PanTilt.PanAxis.Value * 10f);
            float roundTiltValue = Mathf.Round(m_PanTilt.TiltAxis.Value * 10f);
            float roundPanCenter = Mathf.Round(m_PanTilt.PanAxis.Center * 10f);
            float roundTiltCenter = Mathf.Round(m_PanTilt.TiltAxis.Center * 10f);

            if (roundPanValue == roundPanCenter &&
                roundTiltValue == roundTiltCenter)
            {

                m_PanTilt.PanAxis.Recentering.Enabled = false;
                m_PanTilt.TiltAxis.Recentering.Enabled = false;

                m_AxisController.enabled = true;
            }
        }
    }
}