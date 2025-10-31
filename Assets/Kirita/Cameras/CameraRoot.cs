using Unity.Cinemachine;
using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MS.Games
{
    public class CameraRoot : MonoBehaviour
    {
        public enum BASE_CAMERA_TYPE
        {
            FREELOOK,
            ADJUSTMENT
        }

        [System.Serializable]
        private struct BASE_CAMERA_BIND
        {
            public BASE_CAMERA_TYPE m_CameraType;
            public CinemachineCamera m_Camera;
        }

        public readonly int ACTIVE_PRIORITY = 10;
        public readonly int INACTIVE_PRIORITY = 1;

        [SerializeField]
        private BASE_CAMERA_TYPE m_BaseCameraType;
        [SerializeField]
        private List<BASE_CAMERA_BIND> m_BaseCameraBindList = new List<BASE_CAMERA_BIND>();

        private CinemachineCamera m_BaseCurrent;
        public CinemachineCamera BaseCurrent => m_BaseCurrent;


        private void Awake()
        {
            SetCurrentBaseCamera();

            //ベースカメラカメラの初期化チェック
            if (BaseCurrent is null)
            {
                Debug.LogWarning("Current is null");
            }
        }


        /// <summary>
        /// 使用するベースカメラをセットする
        /// </summary>
        public void SetCurrentBaseCamera()
        {
            foreach (BASE_CAMERA_BIND bind in m_BaseCameraBindList)
            {
                if (bind.m_CameraType == m_BaseCameraType)
                {
                    m_BaseCurrent = bind.m_Camera;
                    bind.m_Camera.Priority = ACTIVE_PRIORITY;
                }
                else
                {
                    bind.m_Camera.Priority = INACTIVE_PRIORITY;
                }
            }


            Debug.Log($"Changed Base Camera Type: {m_BaseCameraType}");
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(CameraRoot))]
    public class CameraRootEditor : Editor
    {
        private SerializedProperty m_BaseCameraTypeProp;
        private SerializedProperty m_BaseCameraBindListProp;

        private void OnEnable()
        {
            m_BaseCameraTypeProp = serializedObject.FindProperty("m_BaseCameraType");
            m_BaseCameraBindListProp = serializedObject.FindProperty("m_BaseCameraBindList");
        }

        public override void OnInspectorGUI()
        {
            //シリアライズオブジェクトの更新
            serializedObject.Update();

            //カメラリストの表示
            EditorGUILayout.PropertyField(m_BaseCameraBindListProp, true);

            //ベースカメラリスト重複チェック
            HashSet<CameraRoot.BASE_CAMERA_TYPE> baseTypeSet = new HashSet<CameraRoot.BASE_CAMERA_TYPE>();
            bool hasDuplicate = false;
            foreach (SerializedProperty prop in m_BaseCameraBindListProp)
            {
                var typeProp = prop.FindPropertyRelative("m_CameraType");
                var type = (CameraRoot.BASE_CAMERA_TYPE)typeProp.enumValueIndex;
                if (!baseTypeSet.Add(type))
                {
                    hasDuplicate = true;
                }
            }
            if (hasDuplicate)
            {
                EditorGUILayout.HelpBox("CameraType が重複しています！", MessageType.Warning);
            }

            //カメラタイプ選択
            bool isChangedCurrentCamera = false;
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_BaseCameraTypeProp);
            if(EditorGUI.EndChangeCheck())
            {
                isChangedCurrentCamera = true;
            }

            serializedObject.ApplyModifiedProperties();

            if(isChangedCurrentCamera)
            {
                CameraRoot cameraRoot = (CameraRoot)target;
                cameraRoot.SetCurrentBaseCamera();
            }
        }
    }
#endif

}