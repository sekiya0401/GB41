using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Prototype.Games.UI
{
    public class OtherPlayerStateView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI m_NameField;
        [SerializeField]
        private TextMeshProUGUI m_HealthFeild;
        [SerializeField]
        private Slider m_HealtGauge;
        private Player m_Player;

        public Player Player => m_Player;

        public void Attach(Player player)
        {
            m_Player = player;
            m_NameField.text = player.Id.ToString();
            m_HealtGauge.maxValue = player.State.MaxHealth;
            m_HealtGauge.value = player.Health;
        }

        public void Detach()
        {
            m_Player = null;
            m_NameField.text = string.Empty;
        }

        private void Update()
        {
            m_HealthFeild.text = $"{m_Player.Health:F0}";
            m_HealtGauge.value = m_Player.Health;
        }
    }
}