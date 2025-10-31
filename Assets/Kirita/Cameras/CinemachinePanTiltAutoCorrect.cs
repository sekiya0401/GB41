using Unity.Cinemachine;
using UnityEngine;

[ExecuteAlways]
[AddComponentMenu("Cinemachine/Extensions/Cinemachine PanTilt Auto Correct")]
public class CinemachinePanTiltAutoCorrect : CinemachineExtension
{
    [Header("補正設定")]
    [Tooltip("Pan(Y軸回転)を補整するかどうか")]
    [SerializeField]
    private bool m_IsPanAutoCorrect = true;
    [Tooltip("Pan(Y軸回転)補正速度")]
    [SerializeField, Min(0f)]
    private float m_PanLerpSpeed = 4f;

    [Tooltip("Tilt(X軸回転)を補整するかどうか")]
    [SerializeField]
    private bool m_IsTiltAutoCorrect = true;
    [Tooltip("Tilt(X軸回転)補正速度")]
    [SerializeField, Min(0f)]
    private float m_TiltLerpSpeed = 2f;

    [Header("ビューポイントしきい値")]
    [SerializeField, Range(0f, 1f)]
    private float m_Threshold_X;
    [SerializeField, Range(0f, 1f)]
    private float m_Threshold_Y;

    private CinemachinePanTilt m_PanTilt;
    private Camera m_Camera;

    protected override void Awake()
    {
        base.Awake();
        m_Camera = Camera.main;
        m_PanTilt = GetComponent<CinemachinePanTilt>();
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

        if(m_PanTilt is null)
        {
            Debug.LogWarning($"CinemachinePanTiltがアタッチされていません！ {name}");
            return;
        }



        Transform trackingTarget = vcam.Follow;
        if (trackingTarget == null)
            return;

        if (m_Camera != null)
        {
            Vector3 worldDir = vcam.LookAt.position - m_PanTilt.transform.position;
            Vector3 localDir = m_PanTilt.transform.InverseTransformDirection(worldDir);

            Vector3 vp = m_Camera.WorldToViewportPoint(vcam.LookAt.position);
            float weight = 0f;

            if(m_IsPanAutoCorrect)
            {
                // ターゲットのYawを計算
                float targetYaw = Mathf.Atan2(worldDir.x, worldDir.z) * Mathf.Rad2Deg;

                // === Pan ===
                weight = Mathf.Abs(vp.x - 0.5f) * 2f;
                weight = Mathf.InverseLerp(m_Threshold_X, 1f, weight);
                weight = Mathf.SmoothStep(0f, 1f, weight);

                weight += 0.1f;

                float t = deltaTime * m_PanLerpSpeed;
                m_PanTilt.PanAxis.Value = Mathf.LerpAngle(m_PanTilt.PanAxis.Value, targetYaw, weight * t);
            }

            if(m_IsTiltAutoCorrect)
            {
                // ターゲットのPitchを計算
                float targetPitch = -Mathf.Atan2(localDir.y, localDir.z) * Mathf.Rad2Deg;

                // === Tilt ===
                weight = Mathf.Abs(vp.y - 0.5f) * 2f;
                weight = Mathf.InverseLerp(m_Threshold_Y, 1f, weight);
                weight = Mathf.SmoothStep(0f, 1f, weight);

                float t = deltaTime * m_TiltLerpSpeed;
                m_PanTilt.TiltAxis.Value = Mathf.LerpAngle(m_PanTilt.TiltAxis.Value, targetPitch, weight * t);
            }
        }
        else
        {
            Debug.LogError("PanTilt Auto Correctにカメラ無いです");
        }
    }
}
