using Prototype.Systems;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.Cinemachine;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Prototype.Games
{
    public class SampleCameraController : MonoBehaviour
    {
        [SerializeField]
        private CharacterController m_CharacterController;
        [SerializeField]
        private PlayerInput m_PlayerInput;
        [SerializeField]
        private Transform m_Body;
        [SerializeField]
        private Transform m_CameraRoot;
        [SerializeField]
        private CinemachineCamera m_ThirdPerson;
        [SerializeField]
        private CinemachineCamera m_FreeLook;
        [SerializeField]
        private CinemachineCamera m_MarioKartWorldCamera;
        [SerializeField]
        private TextMeshProUGUI m_OperationInstructionsField;
        [SerializeField, Range(0.1f, 30.0f)]
        private float m_MoveSpeed;
        private CinemachinePanTilt m_PanTilt;
        private CinemachineOrbitalFollow m_OrbitalFollow;

        private enum MODE
        {
            Follow,
            FreeLook,
            SplatoonStyle,
            MarioKartWorldStyle,
        };
        [SerializeField]
        private MODE m_Mode;

        [SerializeField, ShowIfEnum(nameof(m_Mode),MODE.Follow)]
        [Tooltip("視点感度(簡易版)")]
        private Vector2 m_Sensitivity = new Vector2(5f, 5f);
        [SerializeField, ShowIfEnum(nameof(m_Mode), MODE.Follow)]
        [Tooltip("X軸回転の限界値")]
        private Vector2 m_TiltLimit = new Vector2(-80f, 80f);

        [SerializeField, ShowIfEnum(nameof(m_Mode),MODE.FreeLook, MODE.SplatoonStyle,MODE.MarioKartWorldStyle), Range(0f, 30f)]
        [Tooltip("モデルの回転速度")]
        private float m_RotationSpeed = 10f;

        [SerializeField, ShowIfEnum(nameof(m_Mode), MODE.SplatoonStyle), Range(0f, 3f)]
        [Tooltip("水平移動する時間")]
        private float m_HorizontalMovementTime = 0.5f;

        [SerializeField, ShowIfEnum(nameof(m_Mode), MODE.MarioKartWorldStyle), Range(1f, 30f)]
        [Tooltip("デフォルトのポジションに戻るスピード")]
        private float m_RecenterSpeed = 1f;

        private Vector3 m_InputMoveValue;
        private Vector3 m_InputLookValue;
        private float m_MovementTime = 0f;
        private bool m_IsFront = false;
        private bool m_IsRecentering = false;

        [Header("デバック")]
        [SerializeField, Range(0, 512)]
        [Tooltip("移動した座標のカウント数")]
        private int m_TrailCount = 100;
        private List<Vector3> m_TrailPoints = new List<Vector3>();

        private void Awake()
        {
            m_PanTilt = m_FreeLook.GetCinemachineComponent(CinemachineCore.Stage.Aim).GetComponent<CinemachinePanTilt>();
            m_OrbitalFollow = m_MarioKartWorldCamera.GetCinemachineComponent(CinemachineCore.Stage.Body).GetComponent<CinemachineOrbitalFollow>();
        }

        private void Update()
        {
            switch (m_Mode)
            {
                case MODE.Follow:
                    Follow();
                    break;
                case MODE.FreeLook:
                    FreeLook();
                    break;
                case MODE.SplatoonStyle:
                    SplatoonStyle();
                    break;
                case MODE.MarioKartWorldStyle:
                    MarioKartWorldStyle();
                    break;
            }

            if (m_TrailPoints.Count == 0 || Vector3.Distance(m_TrailPoints[^1], transform.position) > 0.2f)
            {
                m_TrailPoints.Add(transform.position);
                if (m_TrailPoints.Count > m_TrailCount) m_TrailPoints.RemoveAt(0); // �Â��_���폜
            }
        }

        private void OnEnable()
        {
            m_PlayerInput.actions["Move"].AddAllPhaseCallbacks(OnMove);
            m_PlayerInput.actions["Look"].AddAllPhaseCallbacks(OnLook);
            m_PlayerInput.actions["Upward"].AddAllPhaseCallbacks(OnUpward);
            m_PlayerInput.actions["Downward"].AddAllPhaseCallbacks(OnDownward);
            m_PlayerInput.actions["Activate"].AddPhaseCallbacks(OnActivate, InputActionExtensions.PHASE.STARTED);
            m_PlayerInput.actions["ChangeCamera"].AddAllPhaseCallbacks(OnChangedCamera);
        }

        private void OnDisable()
        {
            m_PlayerInput.actions["Move"].RemoveAllPhaseCallbacks(OnMove);
            m_PlayerInput.actions["Look"].RemoveAllPhaseCallbacks(OnLook);
            m_PlayerInput.actions["Upward"].RemoveAllPhaseCallbacks(OnUpward);
            m_PlayerInput.actions["Downward"].RemoveAllPhaseCallbacks(OnDownward);
            m_PlayerInput.actions["Activate"].RemovePhaseCallbacks(OnActivate, InputActionExtensions.PHASE.STARTED);
            m_PlayerInput.actions["ChangeCamera"].RemoveAllPhaseCallbacks(OnChangedCamera);
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            Vector2 value = context.ReadValue<Vector2>();
            m_InputMoveValue.Set(value.x, 0f, value.y);

            if(context.canceled)
            {
                m_MovementTime = 0f;
            }
        }

        private void OnLook(InputAction.CallbackContext context)
        {
            m_InputLookValue = context.ReadValue<Vector2>();

            if(context.started)
            {
                m_IsRecentering = false;
            }
            else if(context.canceled)
            {
                m_IsRecentering = true;
            }
        }

        public void OnUpward(InputAction.CallbackContext context)
        {
            //入力があった場合、yに上昇速度をセット、離した場合は0をセットする
            m_InputMoveValue.y = context.ReadValueAsButton() ? 1.0f : 0.0f;
        }

        public void OnDownward(InputAction.CallbackContext context)
        {
            //入力があった場合、yに下降速度をセット、離した場合は0をセットする
            m_InputMoveValue.y = context.ReadValueAsButton() ? -1.0f : 0.0f;
        }

        private void OnActivate(InputAction.CallbackContext context)
        {
            Cursor.lockState = Cursor.lockState switch
            {
                CursorLockMode.None => CursorLockMode.Locked,
                CursorLockMode.Locked => CursorLockMode.None,
                _ => throw new InvalidOperationException("カーソルに未知のロックステートが選択されました")
            };
        }

        private void OnChangedCamera(InputAction.CallbackContext context)
        {
            if(context.started)
            {
                switch(m_Mode)
                {
                    case MODE.Follow or MODE.MarioKartWorldStyle:
                        m_IsFront = true;
                        break;
                    case MODE.SplatoonStyle:
                        float targetYaw = m_Body.localEulerAngles.y;

                        m_PanTilt.PanAxis.Value = targetYaw;
                        m_PanTilt.TiltAxis.Value = m_PanTilt.TiltAxis.Center;
                        break;
                }
            }
            else if(context.canceled)
            {
                switch (m_Mode)
                {
                    case MODE.Follow or MODE.MarioKartWorldStyle:
                        m_IsFront = false;
                        break;
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            for (int i = 0; i < m_TrailPoints.Count - 1; i++)
            {
                Gizmos.DrawLine(m_TrailPoints[i], m_TrailPoints[i + 1]);
            }
        }

        private void Follow()
        {
            transform.Rotate(Vector3.up * m_InputLookValue.x * m_Sensitivity.x * Time.deltaTime);

            // 現在のX軸角度を -180〜180 に正規化
            float pitch = m_CameraRoot.localEulerAngles.x;
            if (pitch > 180f)
                pitch -= 360f;

            pitch -= m_InputLookValue.y * m_Sensitivity.y * Time.deltaTime;
            pitch = Mathf.Clamp(pitch, m_TiltLimit.x, m_TiltLimit.y);

            float yaw = m_IsFront ? 180f : 0f;

            m_CameraRoot.localRotation = Quaternion.Euler(pitch, yaw, 0f);

            Vector3 direction = transform.rotation * m_InputMoveValue;
            m_CharacterController.Move(direction * m_MoveSpeed * Time.deltaTime);
        }

        private void FreeLook()
        {
            Quaternion rotation = Quaternion.identity;
            Vector3 moveValue = m_InputMoveValue;
            moveValue.y = 0f;
            if (moveValue.sqrMagnitude > 0.001f)
            {
                rotation = Quaternion.Euler(0f, Camera.main.transform.eulerAngles.y, 0f);
                Vector3 target = rotation * moveValue;

                Quaternion targetRotation = Quaternion.LookRotation(target);
                if(m_RotationSpeed > 0)
                {
                    m_Body.rotation = Quaternion.Slerp(m_Body.rotation, targetRotation, Time.deltaTime * m_RotationSpeed);
                }
                else
                {
                    m_Body.rotation = targetRotation;
                }
            }

            Vector3 direction = rotation * m_InputMoveValue;
            m_CharacterController.Move(direction * m_MoveSpeed * Time.deltaTime);
        }

        private void SplatoonStyle()
        {
            float t = m_HorizontalMovementTime <= 0f ? 1f : (int)(m_MovementTime / m_HorizontalMovementTime);
            transform.Rotate(Vector3.up * m_InputMoveValue.x * m_MoveSpeed * Time.deltaTime * t);

            Quaternion rotation = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
            Vector3 moveValue = m_InputMoveValue;
            moveValue.y = 0f;
            if (moveValue.sqrMagnitude > 0.001f)
            {
                Vector3 target = rotation * moveValue;
                Quaternion targetRotation = Quaternion.LookRotation(target);
                if (m_RotationSpeed > 0)
                {
                    m_Body.rotation = Quaternion.Slerp(m_Body.rotation, targetRotation, Time.deltaTime * m_RotationSpeed);
                }
                else
                {
                    m_Body.rotation = targetRotation;
                }

                m_MovementTime = Mathf.Min(m_MovementTime + Time.deltaTime, m_HorizontalMovementTime);
            }

            Vector3 direction = rotation * m_InputMoveValue;
            m_CharacterController.Move(direction * m_MoveSpeed * Time.deltaTime);
        }

        private void MarioKartWorldStyle()
        {

            if (m_InputMoveValue.z < 0.001f)
            {
                return;
            }

            if (m_IsRecentering)
            {
                m_OrbitalFollow.HorizontalAxis.Value = Mathf.Lerp(m_OrbitalFollow.HorizontalAxis.Value, m_OrbitalFollow.HorizontalAxis.Center, Time.deltaTime * m_RecenterSpeed);
                m_OrbitalFollow.VerticalAxis.Value = Mathf.Lerp(m_OrbitalFollow.VerticalAxis.Value, m_OrbitalFollow.VerticalAxis.Center, Time.deltaTime * m_RecenterSpeed);
            }

            transform.Rotate(Vector3.up * m_InputMoveValue.x * m_MoveSpeed * Time.deltaTime * m_RotationSpeed);

            float yaw = m_IsFront ? 180f : 0f;
            m_CameraRoot.localRotation = Quaternion.Euler(0, yaw, 0f);

            Vector3 direction = transform.rotation * m_InputMoveValue;
            m_CharacterController.Move(direction * m_MoveSpeed * Time.deltaTime);
        }

        private void OnValidate()
        {
            switch(m_Mode)
            {
                case MODE.Follow:
                    m_ThirdPerson.Priority = 10;
                    m_FreeLook.Priority = 0;
                    m_MarioKartWorldCamera.Priority = 0;
                    break;
                case MODE.FreeLook:
                    m_ThirdPerson.Priority = 0;
                    m_FreeLook.Priority = 10;
                    m_MarioKartWorldCamera.Priority = 0;
                    break;
                case MODE.SplatoonStyle:
                    m_ThirdPerson.Priority = 0;
                    m_FreeLook.Priority = 10;
                    m_MarioKartWorldCamera.Priority = 0;
                    break;
                case MODE.MarioKartWorldStyle:
                    m_ThirdPerson.Priority = 0;
                    m_FreeLook.Priority = 0;
                    m_MarioKartWorldCamera.Priority = 10;
                    break;
            }

            if(Application.isPlaying)
            {
                m_CameraRoot.localRotation = Quaternion.identity;
                m_Body.localRotation = Quaternion.identity;
                SetOperationInstructions();
            }
        }

        private void SetOperationInstructions()
        {
            switch (m_Mode)
            {
                case MODE.Follow:
                    m_OperationInstructionsField.text = FollowOperation;
                    break;
                case MODE.FreeLook:
                    m_OperationInstructionsField.text = "特になし";
                    break;
                case MODE.SplatoonStyle:
                    m_OperationInstructionsField.text = SplatoonStyleOperation;
                    break;
                case MODE.MarioKartWorldStyle:
                    m_OperationInstructionsField.text = MarioKartWorldStyleOperation;
                    break;
            }
        }

        private string FollowOperation
            = "<size=30>押している間カメラが正面に移動</size>\nXbox: B\nプロコン: A\nプレステ: 〇\nキーボード: Tab";
        private string SplatoonStyleOperation
            = "<size=30>押すとカメラがモデルの背面に移動</size>\nXbox: B\nプロコン: A\nプレステ: 〇\nキーボード: Tab";
        private string MarioKartWorldStyleOperation
            = "<size=30>押している間カメラが正面に移動</size>\nXbox: B\nプロコン: A\nプレステ: 〇\nキーボード: Tab" + "\n-----注意-----\n" +
            "前進しているときのみ方向変更可能\n移動時:追従カメラだが見渡すことが出来る\n停止時:フリーカメラ";
    }
}