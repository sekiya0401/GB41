using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using MS.Extensions;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MS.Games
{
    /// <summary>
    /// ターゲットカメラ制御
    /// </summary>
    public class TargetCamera : MonoBehaviour
    {
        public enum TARGET_CAMERA_TYPE
        {
            FIXED,
            FLEX,
        }

        [System.Serializable]
        private struct TARGET_CAMERA_BIND
        {
            public TARGET_CAMERA_TYPE m_CameraType;
            public CinemachineCamera m_Camera;
        }

        [SerializeField]
        private CameraRoot m_CameraRoot;
        [SerializeField]
        private InputActionReference m_TargetInputActionRef;
        [SerializeField]
        private TARGET_CAMERA_TYPE m_TargetCameraType;
        [SerializeField]
        private List<TARGET_CAMERA_BIND> m_TargetCameraBindList = new List<TARGET_CAMERA_BIND>();
        [SerializeField]
        private GameObject m_TargetMarker;
        private Collider m_TargetRange;
        private CinemachineCamera m_TargetCurrent;
        public CinemachineCamera TargetCurrent => m_TargetCurrent;
        private bool m_IsTarget = false;
        private bool m_IsFindTarget = false;

        //HACK: ターゲットカメラの動的切り替えはテスト用なので、今後消す可能性大
        private Transform m_CurrentTarget;

        private void Awake()
        {
            SetCurrentTargetCamera();

            //入力アクションをアクティブにする
            m_TargetInputActionRef.action.Enable();

            if (TryGetComponent(out m_TargetRange))
            {
                m_TargetRange.enabled = false;
            }

            //ターゲットカメラの初期化チェック
            if (TargetCurrent is null)
            {
                Debug.LogWarning("TargetCurrent is null");
            }
        }

        private void OnEnable()
        {
            //入力コールバックの追加
            m_TargetInputActionRef.action.AddPhaseCallbacks(OnTarget, InputActionExtensions.PHASE.STARTED);
        }

        private void OnDisable()
        {
            //入力コールバックの削除
            m_TargetInputActionRef.action.RemovePhaseCallbacks(OnTarget, InputActionExtensions.PHASE.STARTED);
        }

        /// <summary>
        /// ターゲットコールバック
        /// </summary>
        /// <param name="context">入力情報</param>
        private void OnTarget(InputAction.CallbackContext context)
        {
            //コライダーチェック
            if (m_TargetRange)
            {
                //ターゲットの切り替え
                if (m_IsTarget)
                {
                    Detarget();
                }
                else
                {
                    StartCoroutine(FindTarget());
                }
            }
        }

        private void LateUpdate()
        {
            //HACK: ターゲット可視化用の簡易処理
            //ターゲットマーカーがアクティブ状態のチェック
            if (m_TargetMarker.activeSelf)
            {
                //ビルボード処理
                m_TargetMarker.transform.position = TargetCurrent.LookAt.transform.position;
                m_TargetMarker.transform.LookAt(Camera.main.transform.position);
                m_TargetMarker.transform.Rotate(0f, 180f, 0f);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!m_IsTarget)
            {
                Target(other.transform);
                m_CurrentTarget = other.transform;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (m_IsTarget && TargetCurrent.LookAt == other.transform)
            {
                Detarget();
            }
        }

        /// <summary>
        /// 現在使用中のターゲットカメラの注視点の更新
        /// </summary>
        /// <param name="target"></param>
        private void Target(Transform target)
        {
            m_CameraRoot.BaseCurrent.LookAt = target;
            m_TargetCurrent.LookAt = target;
            m_IsTarget = true;
            m_TargetMarker.SetActive(true);
            m_IsFindTarget = true;
            ChangeTargetCamera();
        }

        /// <summary>
        /// 現在使用中のターゲットカメラの注視点を追跡対象に戻す
        /// </summary>
        private void Detarget()
        {
            m_CameraRoot.BaseCurrent.LookAt = m_CameraRoot.BaseCurrent.Follow;
            m_IsTarget = false;
            m_TargetMarker.SetActive(false);
            m_IsFindTarget = false;
            m_TargetRange.enabled = false;
            ChangeTargetCamera();
        }

        /// <summary>
        /// ベースカメラとターゲットカメラを切り替える
        /// </summary>
        private void ChangeTargetCamera()
        {
            if (m_IsFindTarget)
            {
                TargetCurrent.ForceCameraPosition(m_CameraRoot.BaseCurrent.transform.position, m_CameraRoot.BaseCurrent.transform.rotation);
                m_CameraRoot.BaseCurrent.Priority = m_CameraRoot.INACTIVE_PRIORITY;
                TargetCurrent.Priority = m_CameraRoot.ACTIVE_PRIORITY;
            }
            else
            {
                m_CameraRoot.BaseCurrent.ForceCameraPosition(TargetCurrent.transform.position, TargetCurrent.transform.rotation);
                m_CameraRoot.BaseCurrent.Priority = m_CameraRoot.ACTIVE_PRIORITY;
                TargetCurrent.Priority = m_CameraRoot.INACTIVE_PRIORITY;
            }
        }

        /// <summary>
        /// ターゲットになりうるオブジェクトが範囲内に無かったら、コリジョンをオフにする
        /// </summary>
        /// <returns></returns>
        private IEnumerator FindTarget()
        {
            m_TargetRange.enabled = true;
            yield return new WaitForFixedUpdate();
            if (!m_IsFindTarget)
            {
                m_TargetRange.enabled = false;
            }
        }

        /// <summary>
        /// 使用するターゲットカメラをセットする
        /// </summary>
        public void SetCurrentTargetCamera()
        {
            foreach (TARGET_CAMERA_BIND bind in m_TargetCameraBindList)
            {
                if (bind.m_CameraType == m_TargetCameraType)
                {
                    m_TargetCurrent = bind.m_Camera;

                    //HACK: 動的にターゲットカメラを更新した場合にターゲット対処がずれないようにするための簡易処理
                    if (m_IsTarget && m_CurrentTarget is not null)
                    {
                        m_TargetCurrent.LookAt = m_CurrentTarget;
                    }
                }
                bind.m_Camera.Priority = m_CameraRoot.INACTIVE_PRIORITY;
            }

            Debug.Log($"Changed Target Camera Type: {m_TargetCameraType}");
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(TargetCamera))]
    public class TargetCameraEditor : Editor
    {
        private SerializedProperty m_CameraRootProp;
        private SerializedProperty m_TargetInputActionRefProp;
        private SerializedProperty m_TargetCameraTypeProp;
        private SerializedProperty m_TargetCameraListProp;
        private SerializedProperty m_TargetMarkerProp;

        private void OnEnable()
        {
            m_CameraRootProp = serializedObject.FindProperty("m_CameraRoot");
            m_TargetInputActionRefProp = serializedObject.FindProperty("m_TargetInputActionRef");
            m_TargetCameraTypeProp = serializedObject.FindProperty("m_TargetCameraType");
            m_TargetCameraListProp = serializedObject.FindProperty("m_TargetCameraBindList");
            m_TargetMarkerProp = serializedObject.FindProperty("m_TargetMarker");
        }

        public override void OnInspectorGUI()
        {
            //シリアライズオブジェクトの更新
            serializedObject.Update();

            //カメラルートの表示
            EditorGUILayout.PropertyField(m_CameraRootProp, true);

            //インプットアクションリファレンスの表示
            EditorGUILayout.PropertyField(m_TargetInputActionRefProp, true);

            //ターゲットカメラリストの表示
            EditorGUILayout.PropertyField(m_TargetCameraListProp, true);

            //ターゲットカメラリスト重複チェック
            HashSet<TargetCamera.TARGET_CAMERA_TYPE> targetTypeSet = new HashSet<TargetCamera.TARGET_CAMERA_TYPE>();
            bool hasDuplicate = false;
            foreach (SerializedProperty prop in m_TargetCameraListProp)
            {
                var typeProp = prop.FindPropertyRelative("m_CameraType");
                var type = (TargetCamera.TARGET_CAMERA_TYPE)typeProp.enumValueIndex;
                if (!targetTypeSet.Add(type))
                {
                    hasDuplicate = true;
                }
            }
            if (hasDuplicate)
            {
                EditorGUILayout.HelpBox("CameraType が重複しています！", MessageType.Warning);
            }

            //ターゲットカメラタイプ選択
            bool isChangedTargetCamera = false;
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_TargetCameraTypeProp);
            if (EditorGUI.EndChangeCheck())
            {
                isChangedTargetCamera = true;
            }

            EditorGUILayout.PropertyField(m_TargetMarkerProp, true);

            serializedObject.ApplyModifiedProperties();

            if (isChangedTargetCamera)
            {
                TargetCamera targetCamera = (TargetCamera)target;
                targetCamera.SetCurrentTargetCamera();
            }
        }
    }
#endif
}