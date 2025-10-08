using UnityEngine;

namespace Prototype.ScriptableObjects
{
    [CreateAssetMenu(fileName = "PlayerStateSO", menuName = "Scriptable Objects/PlayerStateSO")]
    public class PlayerStateSO : ScriptableObject
    {
        [SerializeField, Min(0.0f)]
        [Tooltip("マウス感度")]
        private float m_Sensitivity = 1.0f;
        [SerializeField, Min(1.0f)]
        private float m_SprintSpeed = 2.0f;
        [SerializeField, Min(1.0f)]
        private float m_HorizontalSpeed = 3.0f;
        [SerializeField, Min(0.1f)]
        private float m_VerticalSpeed = 1.0f;


        [SerializeField]
        private float m_MaxHealth = 100f;
        [SerializeField]
        private float m_MaxStamina = 100f;
        [SerializeField]
        private float m_DecreaseStaminaRate = 10f;
        [SerializeField]
        private float m_IncreaseStaminaRate = 5f;
        [SerializeField]
        private float m_RegenerateHealthRate = 2f;
        [SerializeField]
        private float m_StartHealthRegenerationDelay = 5f;

        public float Sensitivity => m_Sensitivity;
        public float SprintSpeed => m_SprintSpeed;
        public float HorizontalSpeed => m_HorizontalSpeed;
        public float VerticalSpeed => m_VerticalSpeed;

        public float MaxHealth => m_MaxHealth;
        public float MaxStamina => m_MaxStamina;
        public float DecreaseStaminaRate => m_DecreaseStaminaRate;
        public float IncreaseStaminaRate => m_IncreaseStaminaRate;
        public float RegenerateHealthRate => m_RegenerateHealthRate;
        public float StartHealthRegenerationDelay => m_StartHealthRegenerationDelay;
    }

}