using UnityEngine;
using MS.SO.Notification;
using MS.SO.EventChannel;

namespace MS.Games
{
    [CreateAssetMenu(fileName = "PlayerParameters", menuName = "Scriptable Objects/Games/PlayerParameters")]
    public class PlayerParameters : Parameters
    {
        [Min(0f)]
        public float m_HorizontalSpeed;
        [Min(0f)]
        public float m_VerticalSpeed;
        [Min(0f)]
        public float m_RotationSpeed = 3f;
        [SerializeField]
        private NotificationInt m_MaxHealth;
        [SerializeField]
        private NotificationInt m_Health;
        [SerializeField]
        private NotificationInt m_DefeatedEnemiesCount;
        [SerializeField]
        private PlayerStateEventChannel m_StateEventChannel;

        public int MaxHealth
        {
            get => m_MaxHealth.Value;
            set
            {
                m_MaxHealth.Value = Mathf.Max(value, 0);
            }
        }

        public int Health
        {
            get => m_Health.Value;
            set
            {
                m_Health.Value = Mathf.Clamp(value, 0, MaxHealth);
            }
        }

        public int DefeatedEnemiesCount
        {
            get => m_DefeatedEnemiesCount.Value;
            set
            {
                m_DefeatedEnemiesCount.Value = Mathf.Max(value, 0);
            }
        }

        public PlayerStateEventChannel StateEventChannel => m_StateEventChannel;
    }
}