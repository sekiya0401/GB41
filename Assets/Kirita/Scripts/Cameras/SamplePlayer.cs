using System;
using UnityEngine;
using UnityEngine.InputSystem;
using MS.Extensions;
using TNRD;
using MS.Systems;

namespace MS.Games
{
    [RequireComponent(typeof(PlayerInput))]
    public class SamplePlayer : MonoBehaviour,IDamagable,IActivatable
    {
        //移動軸
        enum MOVE_AXIS
        {
            World,
            Camera,
            Self,
            Avatar
        }

        [SerializeField]
        private PlayerInput m_PlayerInput;
        [SerializeField]
        private Transform m_Avatar;
        [SerializeField]
        private SerializableInterface<IInputActionHandler> m_AttackActionHandler;
        [SerializeField]
        private SerializableInterface<IInputActionHandler> m_SkillActionHandler;
        [SerializeField]
        private Blink m_BlinkActionHandler;
        [Header("移動")]
        [SerializeField, Min(0f)]
        private float m_HorizontalSpeed;
        [SerializeField, Min(0f)]
        private float m_VerticalSpeed;
        [SerializeField]
        private MOVE_AXIS m_MoveAxis = MOVE_AXIS.Self;



        [Header("回転")]
        [SerializeField, Min(0f)]
        private float m_RotateSpeed;

        [Header("ステータス")]
        [SerializeField]
        private short m_MaxHealth;
        private short m_Health;

        private Vector3 m_MoveInputValue;
        private Rigidbody m_Rigidbody;

        private void Awake()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
            m_BlinkActionHandler.Init(this);
        }

        private void OnEnable()
        {
            //入力コールバックの追加
            m_PlayerInput.actions["Move"].AddAllPhaseCallbacks(OnMove);
            m_PlayerInput.actions["Upward"].AddPhaseCallbacks(OnUpward, InputActionExtensions.PHASE.STARTED | InputActionExtensions.PHASE.CANCELED);
            m_PlayerInput.actions["Downward"].AddPhaseCallbacks(OnDownward, InputActionExtensions.PHASE.STARTED | InputActionExtensions.PHASE.CANCELED);
            m_PlayerInput.actions["Blink"].AddPhaseCallbacks(OnBlink, InputActionExtensions.PHASE.STARTED);
            m_PlayerInput.actions["Attack"].AddAllPhaseCallbacks(OnAttack);
            m_PlayerInput.actions["Skill"].AddAllPhaseCallbacks(OnSkill);

            m_Health = m_MaxHealth;
        }


        private void OnDisable()
        {
            //入力コールバックの削除
            m_PlayerInput.actions["Move"].RemoveAllPhaseCallbacks(OnMove);
            m_PlayerInput.actions["Upward"].RemovePhaseCallbacks(OnUpward, InputActionExtensions.PHASE.STARTED | InputActionExtensions.PHASE.CANCELED);
            m_PlayerInput.actions["Downward"].RemovePhaseCallbacks(OnDownward, InputActionExtensions.PHASE.STARTED | InputActionExtensions.PHASE.CANCELED);
            m_PlayerInput.actions["Blink"].RemovePhaseCallbacks(OnBlink, InputActionExtensions.PHASE.STARTED);
            m_PlayerInput.actions["Attack"].RemoveAllPhaseCallbacks(OnAttack);
            m_PlayerInput.actions["Skill"].RemoveAllPhaseCallbacks(OnSkill);
        }

        /// <summary>
        /// 移動コールバック処理
        /// </summary>
        /// <param name="context">入力情報</param>
        private void OnMove(InputAction.CallbackContext context)
        {
            Vector2 value = context.ReadValue<Vector2>() * m_HorizontalSpeed;
            m_MoveInputValue.Set(value.x, m_MoveInputValue.y, value.y);
        }

        /// <summary>
        /// 上昇コールバック処理
        /// </summary>
        /// <param name="context">入力情報</param>
        private void OnUpward(InputAction.CallbackContext context)
        {
            m_MoveInputValue.y = context.ReadValueAsButton() ? m_VerticalSpeed : 0f;
        }

        /// <summary>
        /// 下降コールバック処理
        /// </summary>
        /// <param name="context"><入力情報/param>
        private void OnDownward(InputAction.CallbackContext context)
        {
            m_MoveInputValue.y = context.ReadValueAsButton() ? -m_VerticalSpeed : 0f;
        }

        /// <summary>
        /// ブリンクコールバック処理
        /// </summary>
        /// <param name="context">入力情報</param>
        private void OnBlink(InputAction.CallbackContext context)
        {
            m_BlinkActionHandler.Action(context);
        }

        private void OnAttack(InputAction.CallbackContext context)
        {
            m_AttackActionHandler.Value.Action(context);
        }

        private void OnSkill(InputAction.CallbackContext context)
        {
            m_SkillActionHandler.Value.Action(context);
        }

        private void Update()
        {
            //入力による回転
            Vector3 inputValue = m_MoveInputValue;
            inputValue.y = 0f;
            if (inputValue.sqrMagnitude > 0.001f)
            {
                Vector3 direction = GetAxis() * inputValue;
                Quaternion rotation = Quaternion.LookRotation(direction);
                m_Avatar.localRotation = Quaternion.RotateTowards(m_Avatar.localRotation, rotation, m_RotateSpeed * Mathf.Rad2Deg * Time.deltaTime);
            }
        }

        private void FixedUpdate()
        {
            if(m_MoveInputValue.sqrMagnitude <= 0f)
            {
                return;
            }

            //移動
            Vector3 move = GetAxis() * m_MoveInputValue;
            m_Rigidbody.AddForce(move, ForceMode.Acceleration);
        }

        /// <summary>
        /// 移動軸の取得
        /// </summary>
        /// <returns>移動軸となるQuaternion</returns>
        /// <exception cref="NotImplementedException">未知の値が検知されたときのエラー処理</exception>
        private Quaternion GetAxis()
        {
            return m_MoveAxis switch
            {
                MOVE_AXIS.World => Quaternion.identity,
                MOVE_AXIS.Camera => Quaternion.Euler(0f, Camera.main.transform.eulerAngles.y, 0f),
                MOVE_AXIS.Self => transform.localRotation,
                MOVE_AXIS.Avatar => m_Avatar.rotation,
                _ => throw new NotImplementedException($"未知の値が設定されています {m_MoveAxis}")
            };
        }

        public void Damage(int damage)
        {
            short damaged = (short)(m_Health - damage);
            m_Health = Math.Max(damaged, (short)0);

            if(m_Health <= 0)
            {
                Disable();
            }
        }

        private void OnGUI()
        {
            //GUI.color = Color.red;
            //GUI.skin.label.fontSize = 36;

            //GUILayout.Label($"{m_Health}");
            //m_BlinkActionHandler.ShowState();
        }

        public void Enable()
        {
            enabled = true;
        }

        public bool IsEnabled()
        {
            return enabled ? true : false;
        }

        public void Disable()
        {
            enabled = false;
        }

        public bool IsDisabled()
        {
            return enabled ? false : true;
        }
    }

}