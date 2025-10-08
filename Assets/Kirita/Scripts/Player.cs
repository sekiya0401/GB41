using Fusion;
using Fusion.Addons.SimpleKCC;
using UnityEngine;
using UnityEngine.InputSystem;
using Prototype.ScriptableObjects;
using Unity.Cinemachine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Prototype.Games
{
    /// <summary>
    /// プレイヤー制御
    /// </summary>
    [RequireComponent(typeof(PlayerInput)),
     RequireComponent(typeof(SimpleKCC))]
    public class Player : NetworkBehaviour, IDamageable
    {
        [SerializeField]
        private CinemachineCamera m_Camera;
        [SerializeField]
        private DelayUnityEvent m_AttackEvent;
        [SerializeField]
        private PlayerStateSO m_PlayerState;
        [SerializeField]
        private FloatEventChannelScriptableObject m_StaminaEventChannel;
        [SerializeField]
        private FloatEventChannelScriptableObject m_HealthEventChannel;

        //HACK: SimpleKCCではなく、CharacterControllerを使った方が良いかも
        private SimpleKCC m_KCC;
        private PlayerInput m_PlayerInput;
        private Vector3 m_MoveValue;
        private Vector2 m_LookValue;
        private Coroutine m_Regeneration = null;
        private bool m_IsSprinting = false;
        private bool m_IsRegenerating = false;
        private bool m_IsDead = false;
        private float m_Stamina;
        public float Stamina
        {
            get => m_Stamina;
            private set
            {
                value = Mathf.Clamp(value, 0.0f, m_PlayerState.MaxStamina);
                m_Stamina = value;
                m_StaminaEventChannel?.Raise(m_Stamina);
            }
        }

        [Networked, OnChangedRender(nameof(OnHealthChanged)), HideInInspector]
        public float Health
        {
            get;
            set;
        }

        /// <summary>
        /// Healthが変化した際に呼ばれるコールバック
        /// </summary>
        private void OnHealthChanged()
        {
            m_HealthEventChannel?.Raise(Health);
        }


        public override void Spawned()
        {
            TryGetComponent(out m_KCC);
            TryGetComponent(out m_PlayerInput);
            //NOTE: KCCの重力を0にしないと落下してしまう
            m_KCC.SetGravity(0.0f);

            if (HasStateAuthority)
            {
                // ローカルプレイヤーの場合のみHealthを初期化
                Health = m_PlayerState.MaxHealth;
                Stamina = m_PlayerState.MaxStamina;
            }
            else
            {
                //NOTE: ローカル用のアセットをインスペクターで設定しているので、動的に生成し接続する
                m_HealthEventChannel = ScriptableObject.CreateInstance<FloatEventChannelScriptableObject>();
                NetworkPlayerUIController.Instance.ConnectPlayerView(Object.Id.ToString(), m_HealthEventChannel);

                //他のプレイヤーのカメラは無効化
                m_Camera.gameObject.SetActive(false);

                //NOTE: HealthのOnRenderChangedはStateAuthorityでしか呼ばれないので、ここでUIを更新する
                OnHealthChanged();
            }
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            if (!HasStateAuthority)
            {
                NetworkPlayerUIController.Instance.DisconnectPlayerView(Object.Id.ToString());
            }
        }

        /// <summary>
        /// 移動入力
        /// </summary>
        /// <param name="context">入力情報</param>
        public void OnMove(InputAction.CallbackContext context)
        {
            //ゲームパッドのスティック(キーボードWASD)の入力を取得
            Vector2 horizontalMove = context.ReadValue<Vector2>() * m_PlayerState.HorizontalSpeed;
            //NOTE: 平行移動に反映させるために、x,zに値をセットする
            m_MoveValue.Set(horizontalMove.x, m_MoveValue.y, horizontalMove.y);
        }

        /// <summary>
        /// 視点移動入力
        /// </summary>
        /// <param name="context">入力情報</param>
        public void OnLook(InputAction.CallbackContext context)
        {
            //ゲームパッドのスティック(マウス)の入力を取得
            m_LookValue = context.ReadValue<Vector2>();
        }

        /// <summary>
        /// スプリント入力
        /// </summary>
        /// <param name="context">入力情報</param>
        public void OnSprint(InputAction.CallbackContext context)
        {
            m_IsSprinting = context.ReadValueAsButton();
        }

        /// <summary>
        /// アタック入力
        /// </summary>
        /// <param name="context">入力情報</param>
        public void OnAttack(InputAction.CallbackContext context)
        {
            //HACK: 一つのアタックにしか対応出来ていないので、複数アクションに対応させる場合は修正が必要
            if(HasStateAuthority && context.started)
            {
                m_AttackEvent.Invoke();
            }
        }

        /// <summary>
        /// 上昇入力
        /// </summary>
        /// <param name="context">入力情報</param>
        public void OnUpward(InputAction.CallbackContext context)
        {
            //入力があった場合、yに上昇速度をセット、離した場合は0をセットする
            m_MoveValue.y = context.ReadValueAsButton() ? m_PlayerState.VerticalSpeed : 0.0f;
        }

        /// <summary>
        /// 下降入力
        /// </summary>
        /// <param name="context">入力情報</param>
        public void OnDownward(InputAction.CallbackContext context)
        {
            //入力があった場合、yに下降速度をセット、離した場合は0をセットする
            m_MoveValue.y = context.ReadValueAsButton() ? -m_PlayerState.VerticalSpeed : 0.0f;
        }

        /// <summary>
        /// アクティベート入力
        /// </summary>
        /// <param name="context">入力情報</param>
        public void OnActivate(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                switch (Cursor.lockState)
                {
                    case CursorLockMode.None:
                        Cursor.lockState = CursorLockMode.Locked;
                        break;
                    case CursorLockMode.Locked:
                        Cursor.lockState = CursorLockMode.None;
                        break;
                }
            }
        }
        
        public override void FixedUpdateNetwork()
        {
            if(!HasStateAuthority || m_IsDead)
            {
                return;
            }

            //視点移動
            Vector2 look = m_LookValue * m_PlayerState.Sensitivity * Runner.DeltaTime;
            //NOTE: x軸の入力値をy軸回転に、y軸の入力値をx軸回転に変換する
            m_KCC.AddLookRotation(look.y, look.x);

            //移動値計算
            //NOTE: ローカル軸で移動させるために、KCCのTransformRotationを掛ける
            Vector3 move = m_KCC.TransformRotation * m_MoveValue;
            //HACK: KCCのMoveでy軸の移動も出来るようにするため、m_MoveValue.yを直接セットしているが、あまり良い方法ではないかも
            move.y = m_MoveValue.y;

            //スプリント
            //HACK: OnSprintのようなアクションを作成してbool型のメンバ変数で管理した方が効率的かも
            if (m_IsSprinting)
            {
                Stamina -= m_PlayerState.DecreaseStaminaRate;
                move *= Stamina > 0.0f ? m_PlayerState.SprintSpeed : 1.0f;
            }
            else
            {
                Stamina += m_PlayerState.IncreaseStaminaRate;
            }

            //移動
            m_KCC.Move(move, 0);

            if (m_IsRegenerating)
            {
                Health += m_PlayerState.RegenerateHealthRate;
                if (Health >= m_PlayerState.MaxHealth)
                {
                    Health = m_PlayerState.MaxHealth;
                    m_IsRegenerating = false;
                }
            }
        }

        public void Damage(int damage)
        {
            if (HasStateAuthority && !m_IsDead)
            {
                //NOTE: 負傷と回復の両方に対応するために、Clampを使用している
                Health = Mathf.Clamp(Health - damage, 0.0f, m_PlayerState.MaxHealth);
                if (Health <= 0.0f)
                {
                    if(m_Regeneration != null)
                    {
                        StopCoroutine(m_Regeneration);
                        m_Regeneration = null;
                    }
                    m_IsDead = true;
                    return;
                }

                if (m_Regeneration == null)
                {
                    m_Regeneration = StartCoroutine(Regenerate());
                }
                else
                {
                    StopCoroutine(m_Regeneration);
                    m_Regeneration = StartCoroutine(Regenerate());
                }
            }
        }

        public void Heal(int heal)
        {
            if (!HasStateAuthority)
            {
                RPC_Heal(heal);
            }
        }

        [Rpc(RpcSources.Proxies,RpcTargets.StateAuthority)]
        private void RPC_Heal(int heal)
        {
            Health = Mathf.Clamp(Health + heal, 0.0f, m_PlayerState.MaxHealth);
            
            if(m_IsDead)
            {
                m_IsDead = false;
                StartCoroutine(Regenerate());
            }
        }

        private IEnumerator Regenerate()
        {
            m_IsRegenerating = false;
            yield return new WaitForSeconds(m_PlayerState.StartHealthRegenerationDelay);
            Debug.Log("Start Regenerate");
            m_IsRegenerating = true;
            m_Regeneration = null;
        }

        //****Debug****//
        public void OnResetHealth(InputAction.CallbackContext context)
        {
            if (HasStateAuthority)
            {
                Health = m_PlayerState.MaxHealth;
                m_IsDead = false;
            }
        }
    }
}