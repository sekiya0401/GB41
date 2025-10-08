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
    /// �v���C���[����
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

        //HACK: SimpleKCC�ł͂Ȃ��ACharacterController���g���������ǂ�����
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
        /// Health���ω������ۂɌĂ΂��R�[���o�b�N
        /// </summary>
        private void OnHealthChanged()
        {
            m_HealthEventChannel?.Raise(Health);
        }


        public override void Spawned()
        {
            TryGetComponent(out m_KCC);
            TryGetComponent(out m_PlayerInput);
            //NOTE: KCC�̏d�͂�0�ɂ��Ȃ��Ɨ������Ă��܂�
            m_KCC.SetGravity(0.0f);

            if (HasStateAuthority)
            {
                // ���[�J���v���C���[�̏ꍇ�̂�Health��������
                Health = m_PlayerState.MaxHealth;
                Stamina = m_PlayerState.MaxStamina;
            }
            else
            {
                //NOTE: ���[�J���p�̃A�Z�b�g���C���X�y�N�^�[�Őݒ肵�Ă���̂ŁA���I�ɐ������ڑ�����
                m_HealthEventChannel = ScriptableObject.CreateInstance<FloatEventChannelScriptableObject>();
                NetworkPlayerUIController.Instance.ConnectPlayerView(Object.Id.ToString(), m_HealthEventChannel);

                //���̃v���C���[�̃J�����͖�����
                m_Camera.gameObject.SetActive(false);

                //NOTE: Health��OnRenderChanged��StateAuthority�ł����Ă΂�Ȃ��̂ŁA������UI���X�V����
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
        /// �ړ�����
        /// </summary>
        /// <param name="context">���͏��</param>
        public void OnMove(InputAction.CallbackContext context)
        {
            //�Q�[���p�b�h�̃X�e�B�b�N(�L�[�{�[�hWASD)�̓��͂��擾
            Vector2 horizontalMove = context.ReadValue<Vector2>() * m_PlayerState.HorizontalSpeed;
            //NOTE: ���s�ړ��ɔ��f�����邽�߂ɁAx,z�ɒl���Z�b�g����
            m_MoveValue.Set(horizontalMove.x, m_MoveValue.y, horizontalMove.y);
        }

        /// <summary>
        /// ���_�ړ�����
        /// </summary>
        /// <param name="context">���͏��</param>
        public void OnLook(InputAction.CallbackContext context)
        {
            //�Q�[���p�b�h�̃X�e�B�b�N(�}�E�X)�̓��͂��擾
            m_LookValue = context.ReadValue<Vector2>();
        }

        /// <summary>
        /// �X�v�����g����
        /// </summary>
        /// <param name="context">���͏��</param>
        public void OnSprint(InputAction.CallbackContext context)
        {
            m_IsSprinting = context.ReadValueAsButton();
        }

        /// <summary>
        /// �A�^�b�N����
        /// </summary>
        /// <param name="context">���͏��</param>
        public void OnAttack(InputAction.CallbackContext context)
        {
            //HACK: ��̃A�^�b�N�ɂ����Ή��o���Ă��Ȃ��̂ŁA�����A�N�V�����ɑΉ�������ꍇ�͏C�����K�v
            if(HasStateAuthority && context.started)
            {
                m_AttackEvent.Invoke();
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

            //���_�ړ�
            Vector2 look = m_LookValue * m_PlayerState.Sensitivity * Runner.DeltaTime;
            //NOTE: x���̓��͒l��y����]�ɁAy���̓��͒l��x����]�ɕϊ�����
            m_KCC.AddLookRotation(look.y, look.x);

            //�ړ��l�v�Z
            //NOTE: ���[�J�����ňړ������邽�߂ɁAKCC��TransformRotation���|����
            Vector3 move = m_KCC.TransformRotation * m_MoveValue;
            //HACK: KCC��Move��y���̈ړ����o����悤�ɂ��邽�߁Am_MoveValue.y�𒼐ڃZ�b�g���Ă��邪�A���܂�ǂ����@�ł͂Ȃ�����
            move.y = m_MoveValue.y;

            //�X�v�����g
            //HACK: OnSprint�̂悤�ȃA�N�V�������쐬����bool�^�̃����o�ϐ��ŊǗ��������������I����
            if (m_IsSprinting)
            {
                Stamina -= m_PlayerState.DecreaseStaminaRate;
                move *= Stamina > 0.0f ? m_PlayerState.SprintSpeed : 1.0f;
            }
            else
            {
                Stamina += m_PlayerState.IncreaseStaminaRate;
            }

            //�ړ�
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
                //NOTE: �����Ɖ񕜂̗����ɑΉ����邽�߂ɁAClamp���g�p���Ă���
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