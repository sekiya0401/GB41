using UnityEngine;
using Unity.Cinemachine;

[ExecuteAlways]
[AddComponentMenu("Cinemachine/Extensions/Cinemachine Orbital Auto Correct")]
public class CinemachineOrbitalAutoCorrect : CinemachineExtension
{
    [Header("ï‚ê≥ê›íË")]
    [Tooltip("Yawï˚å¸(êÖïΩ)Ç…ï‚êÆÇ∑ÇÈÇ©Ç«Ç§Ç©")]
    [SerializeField]
    private bool m_IsYawAutoCorrect = true;
    [Tooltip("Yawï˚å¸(êÖïΩ)ï‚ê≥ë¨ìx")]
    [SerializeField, Min(0f)]
    private float m_YawLerpSpeed = 4f;

    [Tooltip("Pitchï˚å¸(êÇíº)Ç…ï‚êÆÇ∑ÇÈÇ©Ç«Ç§Ç©")]
    [SerializeField]
    private bool m_IsPitchAutoCorrect = true;
    [Tooltip("Pitchï˚å¸(êÇíº)ï‚ê≥ë¨ìx")]
    [SerializeField, Min(0f)]
    private float m_PitchLerpSpeed = 2f;

    [SerializeField]
    private Transform m_Target;

    private CinemachineOrbitalFollow m_Orbital;

    protected override void Awake()
    {
        base.Awake();
        m_Orbital = GetComponent<CinemachineOrbitalFollow>();
    }

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage,
        ref CameraState state,
        float deltaTime)
    {
        if (stage != CinemachineCore.Stage.Aim)
            return;

        if (m_IsYawAutoCorrect && m_Target)
        {
            m_Orbital.HorizontalAxis.Value = Mathf.LerpAngle(m_Orbital.HorizontalAxis.Value, m_Target.localRotation.eulerAngles.y, Time.deltaTime * m_YawLerpSpeed);
        }

        if (m_IsPitchAutoCorrect && m_Target)
        {
            m_Orbital.VerticalAxis.Value = Mathf.LerpAngle(m_Orbital.VerticalAxis.Value, m_Target.localRotation.eulerAngles.x, Time.deltaTime * m_PitchLerpSpeed);
        }
    }
}