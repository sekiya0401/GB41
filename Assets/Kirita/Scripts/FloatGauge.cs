using UnityEngine;
using Prototype.ScriptableObjects;
using UnityEngine.UI;
using TMPro;

namespace Prototype.Games
{
    /// <summary>
    /// Float�^�̃Q�[�W��\������UI
    /// </summary>
    public class FloatGauge : MonoBehaviour
    {
        [SerializeField]
        private FloatEventChannelScriptableObject m_EventChannel;
        [SerializeField]
        private Image m_Background;
        [SerializeField]
        private Image m_Fill;

        //HACK: Slider���g�킸��Image��fillAmount���g���������ǂ�����
        private Slider m_Slider;

        private void Awake()
        {
            TryGetComponent(out m_Slider);
        }

        private void OnEnable()
        {
            if(m_EventChannel)
            {
                m_EventChannel.Event += OnUpdateGauge;
            }
        }

        private void OnDisable()
        {
            if (m_EventChannel)
            {
                m_EventChannel.Event -= OnUpdateGauge;
            }
        }

        /// <summary>
        /// �Q�[�W�X�V�p�̃R�[���o�b�N
        /// </summary>
        /// <param name="value">�X�V�l</param>
        private void OnUpdateGauge(float value)
        {
            if (m_Slider != null)
            {
                m_Slider.value = value;
            }
        }

        /// <summary>
        /// �C�x���g�`���l���������ւ���
        /// </summary>
        /// <param name="floatEvent">�C�x���g�`�����l��</param>
        public void RetachEvent(FloatEventChannelScriptableObject floatEvent)
        {
            //NOTE: �C�x���g�̑��d�o�^��h�����߁A��U���������Ă���ēo�^
            enabled = false;
            m_EventChannel = floatEvent;
            enabled = true;
        }

        /// <summary>
        /// �Q�[�W�̍ő�l�ƍŏ��l��ݒ肷��
        /// </summary>
        /// <param name="max">�ő�l</param>
        /// <param name="min">�ŏ��l</param>
        public void SetMaxMin(float max, float min)
        {
            if (m_Slider != null)
            {
                m_Slider.maxValue = max;
                m_Slider.minValue = min;
            }
        }

        /// <summary>
        /// �w�i�F��ݒ肷��
        /// </summary>
        /// <param name="color">�w�i�F</param>
        public void SetBackgroundColor(Color color)
        {
            if (m_Background != null)
            {
                m_Background.color = color;
            }
        }

        /// <summary>
        /// �Q�[�W�̐F��ݒ肷��
        /// </summary>
        /// <param name="color">�Q�[�W�̐F</param>
        public void SetFillColor(Color color)
        {
            if (m_Fill != null)
            {
                m_Fill.color = color;
            }
        }
    }
}