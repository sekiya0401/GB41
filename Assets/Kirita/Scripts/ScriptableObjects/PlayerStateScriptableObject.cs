using Prototype.Games;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "PlayerState", menuName = "Scriptable Objects/PlayerState")]
public class PlayerStateScriptableObject : ScriptableObject
{
    [Header("操作関係")]
    [SerializeField, Min(0f)]
    private float m_HorizontalSpeed;
    [SerializeField, Min(0f)]
    private float m_VerticalSpeed;
    [SerializeField, Min(0f)]
    private float m_SprintSpeed;
    [SerializeField, Min(0f)]
    private float m_BlinkSpeed;
    [SerializeField]
    private Vector2VariableScriptableObject m_Sensitivity;

    public Vector2 Sensitivity => m_Sensitivity.Value;
    public float HorizontalSpeed => m_HorizontalSpeed;
    public float VerticalSpeed => m_VerticalSpeed;
    public float SprintSpeed => m_SprintSpeed;
    public float BlinkSpeed => m_BlinkSpeed;

    [Header("ステータス関係")]
    [SerializeField, Min(0f)]
    private float m_MaxHealth;
    [SerializeField, Min(0f)]
    private float m_MaxStamina;
    [SerializeField]
    private string m_SelfName;
    [SerializeField]
    private FloatEventChannelScriptableObject m_HealthEvent;
    [SerializeField]
    private FloatReferenceScriptableObject m_StaminaReference;

    public float MaxHealth => m_MaxHealth;
    public float MaxStamina => m_MaxStamina;
    public string SelfName => m_SelfName;
    public FloatEventChannelScriptableObject HealthEvent => m_HealthEvent;
    public FloatReferenceScriptableObject Stamina => m_StaminaReference;

    [Header("その他")]
    [SerializeField, Min(0f)]
    private float m_Regeneration;
    [SerializeField, Min(0f)]
    private float m_WaitRegenerationTime;
    [SerializeField, Min(0f)]
    private int m_Heal;
    [SerializeField, Min(0f)]
    private int m_Damage;
    [SerializeField, Min(0f)]
    private float m_StaminaDecreaseRate;
    [SerializeField, Min(0f)]
    private float m_StaminaIncreaseRate;
    [SerializeField, Min(0f)]
    private float m_AttackDuration = 0.2f;
    [SerializeField]
    private float m_AttackInterval = 0.4f;

    public float Regeneration => m_Regeneration;
    public float WaitRegenerationTime => m_WaitRegenerationTime;
    public int Heal => m_Heal;
    public int Damage => m_Damage;
    public float StaminaDecreaseRate => m_StaminaDecreaseRate;
    public float StaminaIncreaseRate => m_StaminaIncreaseRate;
    public float AttackDuration => m_AttackDuration;
    public float AttackInterval => m_AttackInterval;


    public void SubscribeHealthEvent(UnityAction<float> listener) => m_HealthEvent.ChangedValue += listener;
    public void UnsubscribeHealthEvent(UnityAction<float> listener) => m_HealthEvent.ChangedValue += listener;
    public void SubscribeStaminaEvent(UnityAction<float> listener) => m_StaminaReference.ChangedValue += listener;
    public void UnsubscribeStaminaEvent(UnityAction<float> listener) => m_StaminaReference.ChangedValue -= listener;

#if UNITY_EDITOR
    [ContextMenu("ResetReference")]
    private void ResetReference()
    {
        if (m_StaminaReference is not null)
        {
            m_StaminaReference.Value = m_MaxStamina;
        }
        EditorUtility.SetDirty(m_StaminaReference);
        AssetDatabase.SaveAssets();
    }
#endif
}
