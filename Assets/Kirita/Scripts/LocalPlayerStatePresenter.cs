using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Prototype.Games.UI
{
    /// <summary>
    /// ローカルプレイヤーのステータスとUIの制御
    /// HACK: とりあえず繋げるための簡易実装
    /// </summary>
    public class LocalPlayerStatePresenter : MonoBehaviour
    {
        [SerializeField]
        private PlayerStateScriptableObject m_PlayerState;
        [Header("ステータス")]
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
        [Header("スキル")]
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
        /// プレイヤーステータスアセットにコールバックを購読する
        /// </summary>
        private void Subscribe()
        {
            m_PlayerState.SubscribeHealthEvent(OnChangedHealth);
            m_PlayerState.SubscribeStaminaEvent(OnChangedStamina);
            m_SkillPointEventChannel.ChangedValue += OnChangedSkillPoint;
            m_SkillIconEventChannel.ChangedValue += OnChangedSkillIcon;
        }

        /// <summary>
        /// プレイヤーステータスアセットに購読したコールバックを解除する
        /// </summary>
        private void Unsubscribe()
        {
            m_PlayerState.UnsubscribeHealthEvent(OnChangedHealth);
            m_PlayerState.UnsubscribeStaminaEvent(OnChangedStamina);
            m_SkillPointEventChannel.ChangedValue -= OnChangedSkillPoint;
            m_SkillIconEventChannel.ChangedValue -= OnChangedSkillIcon;
        }

        /// <summary>
        /// プレイヤーの体力の変化を受け取るコールバック
        /// </summary>
        /// <param name="health">変化後の体力</param>
        private void OnChangedHealth(float health)
        {
            m_HealthGauge.value = health;
            m_HealthField.text = AdjustDecimalPoint(health);
        }

        /// <summary>
        /// プレイヤーのスタミナの変化を受け取るコールバック
        /// </summary>
        /// <param name="stamina">変化後のスタミナ</param>
        private void OnChangedStamina(float stamina)
        {
            m_StaminaGauge.value = stamina;
            m_StaminaField.text = AdjustDecimalPoint(stamina);
        }

        /// <summary>
        /// 表示させる桁数を調整する
        /// </summary>
        /// <param name="value">表示させる値</param>
        /// <returns>桁数を調整した値の文字列</returns>
        private string AdjustDecimalPoint(in float value)
        {
            return $"{value:F1}";
        }

        /// <summary>
        /// スキルポイントの変化を受け取るコールバック
        /// </summary>
        /// <param name="value">変化後のスキルポイントの割合</param>
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