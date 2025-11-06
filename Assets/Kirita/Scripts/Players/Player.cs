using System;
using UnityEngine;
using UnityEngine.InputSystem;
using MS.Extensions;
using TNRD;
using MS.Systems;
using Unity.Netcode;

namespace MS.Games
{
    [RequireComponent(typeof(PlayerInput))]
    public class Player : NetworkBehaviour,IDamagable,IActivatable
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
        private PlayerParameters m_Parameters;
        [SerializeField]
        private SerializableInterface<IInputActionHandler> m_AttackActionHandler;
        [SerializeField]
        private SerializableInterface<IInputActionHandler> m_SkillActionHandler;
        [SerializeField]
        private SerializableInterface<IInputActionHandler> m_BlinkActionHandler;
        [SerializeField]
        private MOVE_AXIS m_MoveAxis = MOVE_AXIS.Self;

        private Vector3 m_MoveInputValue;
        private Rigidbody m_Rigidbody;

        private NetworkVariable<bool> m_IsDead = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            // 自分のプレイヤー以外はInput無効化
            if (!IsOwner)
            {
                m_PlayerInput.enabled = false;
            }
            else
            {
                // 自分の入力のみ有効
                m_PlayerInput.enabled = true;
            }

            m_IsDead.OnValueChanged += OnStateChanged;
        }



        private void Awake()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
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

            m_Parameters.Health = m_Parameters.MaxHealth;
            m_Parameters.DefeatedEnemiesCount = 0;
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

        public override void OnDestroy()
        {
            base.OnDestroy();

            m_IsDead.OnValueChanged -= OnStateChanged;
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
                m_Avatar.localRotation = Quaternion.RotateTowards(m_Avatar.localRotation, rotation, m_Parameters.m_RotationSpeed * Mathf.Rad2Deg * Time.deltaTime);
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
        /// 移動入力コールバック処理
        /// </summary>
        /// <param name="context">入力情報</param>
        private void OnMove(InputAction.CallbackContext context)
        {
            Vector2 value = context.ReadValue<Vector2>() * m_Parameters.m_HorizontalSpeed;
            m_MoveInputValue.Set(value.x, m_MoveInputValue.y, value.y);
        }

        /// <summary>
        /// 上昇入力コールバック処理
        /// </summary>
        /// <param name="context">入力情報</param>
        private void OnUpward(InputAction.CallbackContext context)
        {
            m_MoveInputValue.y = context.ReadValueAsButton() ? m_Parameters.m_VerticalSpeed : 0f;
        }

        /// <summary>
        /// 下降入力コールバック処理
        /// </summary>
        /// <param name="context"><入力情報/param>
        private void OnDownward(InputAction.CallbackContext context)
        {
            m_MoveInputValue.y = context.ReadValueAsButton() ? -m_Parameters.m_VerticalSpeed : 0f;
        }

        /// <summary>
        /// ブリンク入力コールバック処理
        /// </summary>
        /// <param name="context">入力情報</param>
        private void OnBlink(InputAction.CallbackContext context)
        {
            m_BlinkActionHandler.Value.Action(context);
        }

        /// <summary>
        /// 攻撃入力コールバック処理
        /// </summary>
        /// <param name="context">入力情報</param>
        private void OnAttack(InputAction.CallbackContext context)
        {
            m_AttackActionHandler.Value.Action(context);
        }

        /// <summary>
        /// スキル入力コールバック処理
        /// </summary>
        /// <param name="context">入力情報</param>
        private void OnSkill(InputAction.CallbackContext context)
        {
            m_SkillActionHandler.Value.Action(context);
        }
        public void Damage(int damage)
        {
            int damaged = m_Parameters.Health - damage;
            m_Parameters.Health = Math.Max(damaged, 0);

            if(m_Parameters.Health <= 0)
            {
                Disable();
            }
        }

        public void Enable()
        {
            RequestSetStateRpc(false);
        }

        public bool IsEnabled()
        {
            return m_IsDead.Value ? false : true;
        }

        public void Disable()
        {
            RequestSetStateRpc(true);
        }

        public bool IsDisabled()
        {
            return m_IsDead.Value ? true : false;
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

        private void OnStateChanged(bool previousValue, bool newValue)
        {
            m_Parameters.StateEventChannel.Invoke((OwnerClientId, newValue));
            enabled = !newValue;
        }

        [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
        private void RequestSetStateRpc(bool isDead)
        {
            m_IsDead.Value = isDead;
        }
    }

}