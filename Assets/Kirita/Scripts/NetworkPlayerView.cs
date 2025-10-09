using TMPro;
using UnityEngine;
using Prototype.ScriptableObjects;

namespace Prototype.Games
{
    /// <summary>
    /// �l�b�g���[�N�v���C���[�̕\��
    /// HACK: �v���C���[���Ƒ̗̓Q�[�W��\�����邾���̊ȈՓI�Ȏ���
    /// </summary>
    public class NetworkPlayerView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI m_NameField;
        [SerializeField]
        private FloatGauge m_HealthGauge;

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// �\�����閼�O��ݒ�
        /// </summary>
        /// <param name="name">�\����</param>
        public void SetName(string name)
        {
            if (m_NameField != null)
            {
                m_NameField.text = name;
            }
        }

        /// <summary>
        /// �̗̓Q�[�W�̃C�x���g�`���l����ݒ�
        /// </summary>
        /// <param name="channel">�C�x���g�`�����l��</param>
        public void SetHealthGaugeChannel(FloatEventChannelScriptableObject channel)
        {
            if (m_HealthGauge != null)
            {
                m_HealthGauge.RetachEvent(channel);
            }
        }

        public string ID => m_NameField.text;
    }
}