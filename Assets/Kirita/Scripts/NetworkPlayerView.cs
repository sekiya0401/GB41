using TMPro;
using UnityEngine;
using Prototype.ScriptableObjects;

namespace Prototype.Games
{
    /// <summary>
    /// ネットワークプレイヤーの表示
    /// HACK: プレイヤー名と体力ゲージを表示するだけの簡易的な実装
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
        /// 表示する名前を設定
        /// </summary>
        /// <param name="name">表示名</param>
        public void SetName(string name)
        {
            if (m_NameField != null)
            {
                m_NameField.text = name;
            }
        }

        /// <summary>
        /// 体力ゲージのイベントチャネルを設定
        /// </summary>
        /// <param name="channel">イベントチャンネル</param>
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