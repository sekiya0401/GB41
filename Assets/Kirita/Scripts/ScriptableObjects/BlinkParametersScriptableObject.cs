using UnityEngine;
using MS.SO.Notification;

namespace MS.Games
{
    [CreateAssetMenu(fileName = "BlinkParameters", menuName = "Scriptable Objects/Games/BlinkParameters")]
    public class BlinkParameters : Parameters
    {
        [Min(0f)]
        public float m_Speed;
        [SerializeField]
        private NotificationInt m_MaxCount;
        [SerializeField]
        private NotificationInt m_Count;
        [Min(0f)]
        public float m_CountRestorationTime = 3f;
        //[Min(0f)]
        //public float m_CoolDownTime = 0.2f;

        public int MaxCount
        {
            get => m_MaxCount.Value;    
            set
            {
                m_MaxCount.Value = Mathf.Max(0, value);
            }
        }

        public int Count
        {
            get => m_Count.Value;
            set
            {
                m_Count.Value = Mathf.Clamp(value, 0, MaxCount);
            }
        }
    }
}