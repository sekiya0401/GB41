using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Prototype.Games.UI
{
    /// <summary>
    /// ���[�J���v���C���[�̃X�e�[�^�X��UI�̐���
    /// HACK: �Ƃ肠�����q���邽�߂̊ȈՎ���
    /// </summary>
    public class LocalPlayerStatePresenter : MonoBehaviour
    {
        [SerializeField]
        private PlayerStateScriptableObject m_PlayerState;
        [Header("�X�e�[�^�X")]
        [SerializeField]
        private TextMeshProUGUI m_NameTextMesh;
        [SerializeField]
        private Slider m_HealthGauge;
        [SerializeField]
        private Slider m_StaminaGauge;
        [SerializeField]
        private TextMeshProUGUI m_HealthField;
        [SerializeField]
        private TextMeshProUGUI m_StaminaField;
        [Header("�X�L��")]
        [SerializeField]
        private FloatEventChannelScriptableObject m_SkillPointEventChannel;
        [SerializeField]
        private SpriteEventChannelScriptableObject m_SkillIconEventChannel;
        [SerializeField]
        private Image m_SkillIcon;

        private void Awake()
        {
            if (m_NameTextMesh is not null)
            {
                m_NameTextMesh.text = m_PlayerState.SelfName;
            }

            if (m_HealthGauge is not null)
            {
                m_HealthGauge.maxValue = m_PlayerState.MaxHealth;
                m_HealthGauge.value = m_PlayerState.MaxHealth;
            }

            if (m_StaminaGauge is not null)
            {
                m_StaminaGauge.maxValue = m_PlayerState.MaxStamina;
                m_StaminaGauge.value = m_PlayerState.Stamina.Value;
            }

            if (m_HealthField is not null)
            {
                m_HealthField.text = AdjustDecimalPoint(m_HealthGauge.value);
            }

            if (m_StaminaField is not null)
            {
                m_StaminaField.text = AdjustDecimalPoint(m_PlayerState.Stamina.Value);
            }
        }

        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        /// <summary>
        /// �v���C���[�X�e�[�^�X�A�Z�b�g�ɃR�[���o�b�N���w�ǂ���
        /// </summary>
        private void Subscribe()
        {
            m_PlayerState.SubscribeHealthEvent(OnChangedHealth);
            m_PlayerState.SubscribeStaminaEvent(OnChangedStamina);
            m_SkillPointEventChannel.ChangedValue += OnChangedSkillPoint;
            m_SkillIconEventChannel.ChangedValue += OnChangedSkillIcon;
        }

        /// <summary>
        /// �v���C���[�X�e�[�^�X�A�Z�b�g�ɍw�ǂ����R�[���o�b�N����������
        /// </summary>
        private void Unsubscribe()
        {
            m_PlayerState.UnsubscribeHealthEvent(OnChangedHealth);
            m_PlayerState.UnsubscribeStaminaEvent(OnChangedStamina);
            m_SkillPointEventChannel.ChangedValue -= OnChangedSkillPoint;
            m_SkillIconEventChannel.ChangedValue -= OnChangedSkillIcon;
        }

        /// <summary>
        /// �v���C���[�̗̑͂̕ω����󂯎��R�[���o�b�N
        /// </summary>
        /// <param name="health">�ω���̗̑�</param>
        private void OnChangedHealth(float health)
        {
            m_HealthGauge.value = health;
            m_HealthField.text = AdjustDecimalPoint(health);
        }

        /// <summary>
        /// �v���C���[�̃X�^�~�i�̕ω����󂯎��R�[���o�b�N
        /// </summary>
        /// <param name="stamina">�ω���̃X�^�~�i</param>
        private void OnChangedStamina(float stamina)
        {
            m_StaminaGauge.value = stamina;
            m_StaminaField.text = AdjustDecimalPoint(stamina);
        }

        /// <summary>
        /// �\�������錅���𒲐�����
        /// </summary>
        /// <param name="value">�\��������l</param>
        /// <returns>�����𒲐������l�̕�����</returns>
        private string AdjustDecimalPoint(in float value)
        {
            return $"{value:F1}";
        }

        /// <summary>
        /// �X�L���|�C���g�̕ω����󂯎��R�[���o�b�N
        /// </summary>
        /// <param name="value">�ω���̃X�L���|�C���g�̊���</param>
        private void OnChangedSkillPoint(float value)
        {
            if(value >= 1f)
            {
                m_SkillIcon.color = Color.red;
            }
            else
            {
                m_SkillIcon.color = Color.white;
            }

            m_SkillIcon.fillAmount = value;
        }

        private void OnChangedSkillIcon(Sprite icon)
        {
            m_SkillIcon.sprite = icon; 
        }
    }
}