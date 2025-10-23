using Prototype.Systems;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Prototype.Games
{

    public class TargetCamera : MonoBehaviour
    {
        enum TARGET_TYPE
        {
            Group,
            Fixed,
            Flex,
        };
        [SerializeField]
        private TARGET_TYPE m_Type;
        [SerializeField]
        private Collider m_Collider;
        [SerializeField]
        private CinemachineTargetGroup m_Group;
        [SerializeField]
        private Transform m_Target;

        [SerializeField, ShowIfEnum(nameof(m_Type), TARGET_TYPE.Group)]
        private CinemachineCamera m_GroupCamera;

        [SerializeField, ShowIfEnum(nameof(m_Type), TARGET_TYPE.Fixed)]
        private CinemachineCamera m_FixedCamera;
        [SerializeField,ShowIfEnum(nameof(m_Type),TARGET_TYPE.Fixed, TARGET_TYPE.Flex)]
        private Transform m_CameraRoot;

        [SerializeField, ShowIfEnum(nameof(m_Type), TARGET_TYPE.Flex)]
        private CinemachineCamera m_FlexCamera;
        private CinemachinePanTilt m_PanTilt;

        private Vector3 m_LookInputValue;

        public bool IsTarget
        {
            get;
            private set;
        }

        private void Awake()
        {
            m_PanTilt = m_FlexCamera.GetCinemachineComponent(CinemachineCore.Stage.Aim).GetComponent<CinemachinePanTilt>();

            m_GroupCamera.enabled = false;
            m_FixedCamera.enabled = false;
            m_FlexCamera.enabled = false;
        }

        private void OnEnable()
        {
        }

        private void OnDisable()
        {
        }

        public void Target()
        {
            IsTarget = true;
            m_Collider.enabled = true;

            switch(m_Type)
            {
                case TARGET_TYPE.Group:
                    m_GroupCamera.transform.position = Camera.main.transform.position;
                    m_GroupCamera.enabled = true;
                    m_GroupCamera.Priority.Value = 100;
                    break;
                case TARGET_TYPE.Fixed:
                    m_FixedCamera.transform.position = Camera.main.transform.position;
                    m_FixedCamera.enabled = true;
                    m_FixedCamera.Priority.Value = 100;
                    break;
                case TARGET_TYPE.Flex:
                    m_FlexCamera.transform.position = Camera.main.transform.position;
                    m_FlexCamera.enabled = true;
                    m_FlexCamera.Priority.Value = 100;          
                    break;
            }
        }

        public void Detarget()
        {
            IsTarget = false;
            m_Collider.enabled = false;

            switch (m_Type)
            {
                case TARGET_TYPE.Group:
                    m_GroupCamera.Priority.Value = 0;
                    m_GroupCamera.enabled = false;
                    break;
                case TARGET_TYPE.Fixed:
                    m_FixedCamera.Priority.Value = 0;
                    m_FixedCamera.enabled = false;
                    break;
                case TARGET_TYPE.Flex:
                    m_FlexCamera.Priority.Value = 0;
                    m_FlexCamera.enabled = false;
                    break;
            }

            m_Target.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (!IsTarget)
            {
                return;
            }

            switch(m_Type)
            {
                case TARGET_TYPE.Group:
                    if (m_Group.Targets[1].Object is null)
                    {
                        Debug.LogWarning("m_Group.Targets[1].Object is null");
                        return;
                    }
                    m_Target.position = m_Group.Targets[1].Object.position;
                    break;
                case TARGET_TYPE.Fixed:
                    if (m_FixedCamera.LookAt is null)
                    {
                        Debug.LogWarning("m_FixedCamera.LookAt is null");
                        return;
                    }

                    var direction = m_FixedCamera.LookAt.position - m_CameraRoot.position;
                    m_CameraRoot.rotation = Quaternion.LookRotation(direction);

                    m_Target.position = m_FixedCamera.LookAt.position;
                    break;
                case TARGET_TYPE.Flex:
                    if(m_FlexCamera.LookAt is null)
                    {
                        Debug.LogWarning("m_FlexCamera.LookAt is null");
                        return;
                    }

                    direction = m_FlexCamera.LookAt.position - m_FlexCamera.transform.position;
                    Quaternion directionQuat = Quaternion.LookRotation(direction);
                    float t = Time.deltaTime * 5f;
                    m_PanTilt.PanAxis.Value = Mathf.LerpAngle(m_PanTilt.PanAxis.Value, directionQuat.eulerAngles.y, t);

                    Vector3 vp = Camera.main.WorldToViewportPoint(m_FlexCamera.LookAt.position);
                    float weight = Mathf.Abs(vp.y - 0.5f) * 2f;
                    weight = Mathf.InverseLerp(0.7f, 1f, weight); // 0.5以下→0, 1以上→1
                    weight = Mathf.SmoothStep(0f, 1f, weight);

                    m_PanTilt.TiltAxis.Value = Mathf.LerpAngle(m_PanTilt.TiltAxis.Value, directionQuat.eulerAngles.x, weight * t);

                    m_Target.position = m_FlexCamera.LookAt.position;

                    if (vp.x < 0f || vp.x > 1f || vp.y < 0f || vp.y > 1f)
                    {
                        Detarget();
                    }

                    break;
            }

            if(m_Target.gameObject.activeSelf)
            {
                Vector3 toTarget = m_Target.position - Camera.main.transform.position;
                m_Target.LookAt(toTarget);
            }
        }

        private void LateUpdate()
        {
            if (!IsTarget)
            {
                return;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            switch(m_Type)
            {
                case TARGET_TYPE.Group:
                    m_Group.Targets[1].Object = other.transform;
                    break;
                case TARGET_TYPE.Fixed:
                    m_FixedCamera.LookAt = other.transform;
                    break;
                case TARGET_TYPE.Flex:
                    m_FlexCamera.LookAt = other.transform;
                    break;
            }

            if(!m_Target.gameObject.activeSelf)
            {
                m_Target.gameObject.SetActive(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            switch (m_Type)
            {
                case TARGET_TYPE.Group:
                    if (m_Group.Targets[1].Object == other.transform)
                    {
                        m_Group.Targets[1].Object = null;
                        Detarget();

                    }
                    break;
                case TARGET_TYPE.Fixed:
                    if (m_FixedCamera.LookAt == other.transform)
                    {
                        m_FixedCamera.LookAt = null;
                        Detarget();
                    }
                    break;
                case TARGET_TYPE.Flex:
                    if (m_FlexCamera.LookAt == other.transform)
                    {
                        m_FlexCamera.LookAt = null;
                        Detarget();
                    }
                    break;
            }
        }

        private void OnGUI()
        {
            GUI.skin.label.fontSize = 28;
            GUI.color = Color.cyan;
            GUILayout.Label($"m_LookInputValue: {m_LookInputValue}");
        }
    }
}