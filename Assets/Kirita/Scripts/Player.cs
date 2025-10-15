using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using Fusion.Addons.SimpleKCC;
using TNRD;
using Unity.Cinemachine;
using Prototype.Games.UI;



#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Prototype.Games
{
    /// <summary>
    /// �v���C���[����
    /// </summary>
    [RequireComponent(typeof(PlayerInput)),
     RequireComponent(typeof(SimpleKCC))]
    public class Player : NetworkBehaviour, IDamagable
    {
        [SerializeField]
        private PlayerStateScriptableObject m_PlayerState;
        [SerializeField]
        private Transform m_Body;
        [SerializeField]
        private Transform m_At;
        [SerializeField]
        private SerializableInterface<IPlayerAction> m_AttackAction;
        [SerializeField]
        private SkillAction m_SkillAction;
        [SerializeField]
        private CinemachineCamera m_Camera;

        private enum MOVE_STATE
        {
            Walk,
            Sprint,
            Blink
        };
        private MOVE_STATE m_MoveState = MOVE_STATE.Walk;

        private SimpleKCC m_KCC;
        private Vector3 m_MoveValue;
        private Vector3 m_LookValue;
        private int m_DefeatedEnemies = 0;
        private Coroutine m_Regeneration = null;
        private bool m_IsRegenerating = false;
        private bool m_IsDead = false;
        private bool m_IsUpdate = false;

        public PlayerStateScriptableObject State => m_PlayerState;

        [Networked, OnChangedRender(nameof(OnHealthChanged)),HideInInspector]
        public float Health
        {
            get;
            set;
        }

        /// <summary>
        /// Health���ω������ۂɌĂ΂��R�[���o�b�N
        /// </summary>
        private void OnHealthChanged()
        {
            if(m_PlayerState)
            {
                m_PlayerState.HealthEvent.Invoke(Health);
            }
        }


        public override void Spawned()
        {
            TryGetComponent(out m_KCC);
            m_KCC.SetGravity(0f);

            if (HasStateAuthority)
            {
                // ���[�J���v���C���[�̏ꍇ�̂�Health��������
                Health = m_PlayerState.MaxHealth;
            }
            else
            {
                //NOTE: Health��OnRenderChanged��StateAuthority�ł����Ă΂�Ȃ��̂ŁA������UI���X�V����
                OnHealthChanged();

                m_Camera.gameObject.SetActive(false);
                var presenter = FindAnyObjectByType<OtherPlayerStatePresenter>();
                if(presenter)
                {
                    presenter.ConnectPlayer(this);
                }
                m_PlayerState = null;
            }
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            var presenter = FindAnyObjectByType<OtherPlayerStatePresenter>();
            if (presenter)
            {
                presenter.Disconnect(this);
            }
        }

        /// <summary>
        /// �ړ�����
        /// </summary>
        /// <param name="context">���͏��</param>
        public void OnMove(InputAction.CallbackContext context)
        {
            //�Q�[���p�b�h�̃X�e�B�b�N(�L�[�{�[�hWASD)�̓��͂��擾
            Vector2 horizontalMove = context.ReadValue<Vector2>();
            //NOTE: ���s�ړ��ɔ��f�����邽�߂ɁAx,z�ɒl���Z�b�g����
            m_MoveValue.Set(horizontalMove.x, m_MoveValue.y, horizontalMove.y);
        }

        /// <summary>
        /// ���_�ړ�����
        /// </summary>
        /// <param name="context">���͏��</param>
        public void OnLook(InputAction.CallbackContext context)
        {
            Vector2 look = context.ReadValue<Vector2>();
            m_LookValue.Set(look.y, look.x, 0f);
        }

        /// <summary>
        /// �X�v�����g����
        /// </summary>
        /// <param name="context">���͏��</param>
        public void OnSprint(InputAction.CallbackContext context)
        {
            if(context.started)
            {
                m_MoveState = MOVE_STATE.Blink;
            }
            else if(context.performed)
            {
                m_MoveState = MOVE_STATE.Sprint;
            }
            else
            {
                m_MoveState = MOVE_STATE.Walk;
            }
        }

        /// <summary>
        /// �A�^�b�N����
        /// </summary>
        /// <param name="context">���͏��</param>
        public void OnAttack(InputAction.CallbackContext context)
        {
            if(HasStateAuthority && context.started)
            {
                m_AttackAction.Value.Action(this);
            }
        }

        /// <summary>
        /// �X�L������
        /// </summary>
        /// <param name="context">���͏��</param>
        public void OnSkill(InputAction.CallbackContext context)
        {
            if(HasStateAuthority && context.started)
            {
                m_SkillAction.Action(this);
            }
        }

        /// <summary>
        /// �㏸����
        /// </summary>
        /// <param name="context">���͏��</param>
        public void OnUpward(InputAction.CallbackContext context)
        {
            //���͂��������ꍇ�Ay�ɏ㏸���x���Z�b�g�A�������ꍇ��0���Z�b�g����
            m_MoveValue.y = context.ReadValueAsButton() ? m_PlayerState.VerticalSpeed : 0.0f;
        }

        /// <summary>
        /// ���~����
        /// </summary>
        /// <param name="context">���͏��</param>
        public void OnDownward(InputAction.CallbackContext context)
        {
            //���͂��������ꍇ�Ay�ɉ��~���x���Z�b�g�A�������ꍇ��0���Z�b�g����
            m_MoveValue.y = context.ReadValueAsButton() ? -m_PlayerState.VerticalSpeed : 0.0f;
        }

        /// <summary>
        /// �A�N�e�B�x�[�g����
        /// </summary>
        /// <param name="context">���͏��</param>
        public void OnActivate(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                switch (Cursor.lockState)
                {
                    case CursorLockMode.None:
                        Cursor.lockState = CursorLockMode.Locked;
                        m_IsUpdate = true;
                        break;
                    case CursorLockMode.Locked:
                        Cursor.lockState = CursorLockMode.None;
                        m_IsUpdate = false;
                        m_MoveValue = Vector3.zero;
                        break;
                }
            }
        }

        public override void Render()
        {
            if (!HasStateAuthority || !m_IsUpdate)
            {
                return;
            }

            //�J��������
            m_At.Rotate(m_LookValue * m_PlayerState.Sensitivity * Time.deltaTime);
        }

        public override void FixedUpdateNetwork()
        {
            if (!HasStateAuthority || !m_IsUpdate || m_IsDead)
            {
                return;
            }

            //��]
            //HACK: �J�����̑�����@�ɂ��Č��܂�܂ł̊ȈՎ���
            Quaternion rotQuat = Quaternion.Euler(0f, Camera.main.transform.eulerAngles.y, 0f);
            if (m_MoveValue.sqrMagnitude > 0)
            {
                Vector3 moveDir = rotQuat * m_MoveValue;
                Quaternion targetQuat = Quaternion.LookRotation(moveDir);

                m_Body.rotation = targetQuat;
            }

            //�ړ�
            Vector3 move = rotQuat * m_MoveValue * m_PlayerState.HorizontalSpeed;

            //�X�v�����g
            float stamina = m_PlayerState.Stamina.Value;
            if(m_MoveState != MOVE_STATE.Walk && stamina > 0f)
            {
                switch (m_MoveState)
                {
                    case MOVE_STATE.Sprint:
                        move *= m_PlayerState.SprintSpeed;
                        m_PlayerState.Stamina.Value = Mathf.Max(stamina - m_PlayerState.StaminaDecreaseRate, 0f);
                        break;
                    case MOVE_STATE.Blink:
                        move *= m_PlayerState.BlinkSpeed;
                        m_PlayerState.Stamina.Value = Mathf.Max(stamina - m_PlayerState.StaminaDecreaseRate * m_PlayerState.BlinkSpeed, 0f);
                        break;
                }
            }
            else
            {
                if (stamina < m_PlayerState.MaxStamina)
                {
                    m_PlayerState.Stamina.Value = Mathf.Min(stamina + m_PlayerState.StaminaIncreaseRate, m_PlayerState.MaxStamina);
                }
            }


                //HACK: KCC��Move��y���̈ړ����o����悤�ɂ��邽�߁Am_MoveValue.y�𒼐ڃZ�b�g���Ă��邪�A���܂�ǂ����@�ł͂Ȃ�����
                move.y = m_MoveValue.y;

            m_KCC.Move(move);

            //���U���N�V����
            if (m_IsRegenerating)
            {
                Health += m_PlayerState.Regeneration;
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
                //NOTE: �����Ɖ񕜂̗����ɑΉ����邽�߂ɁAClamp���g�p���Ă���
                Health = Mathf.Clamp(Health - damage, 0.0f, m_PlayerState.MaxHealth);
                if (Health <= 0.0f)
                {
                    if (m_Regeneration != null)
                    {
                        StopCoroutine(m_Regeneration);
                        m_Regeneration = null;
                    }
                    m_IsDead = true;
                    return;
                }

                //NOTE: ���U���N�V�������ɍU�����ꂽ�ꍇ�A���U���N�V�����R���[�`�������X�^�[�g������
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

        public void DefeatedEnemy()
        {
            m_DefeatedEnemies++;
            m_SkillAction.AddPoint(1);
        }

        public void Heal(int heal)
        {
            if (HasStateAuthority)
            {
                Health = Mathf.Clamp(Health + heal, 0.0f, m_PlayerState.MaxHealth);

                if (m_IsDead && Health >= m_PlayerState.MaxHealth)
                {
                    m_IsDead = false;
                }
            }
            else
            {
                RPC_Heal(heal);
            }
        }

        [Rpc(RpcSources.Proxies, RpcTargets.StateAuthority)]
        private void RPC_Heal(int heal)
        {
            Heal(heal);
            Debug.Log(nameof(RPC_Heal));
        }

        private IEnumerator Regenerate()
        {
            m_IsRegenerating = false;
            yield return new WaitForSeconds(m_PlayerState.WaitRegenerationTime);
            m_IsRegenerating = true;
            m_Regeneration = null;
        }

        //****Debug****//

        private void OnGUI()
        {
            if (!HasStateAuthority)
            {
                return;
            }
        }

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