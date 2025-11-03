using MS.Games;
using MS.Systems.CoolDown;
using System.Data;
using UnityEngine;
using UnityEngine.InputSystem;

public class Blink : MonoBehaviour, IInputActionHandler
{
    [SerializeField, Min(1f)]
    float m_Speed = 2f;
    [SerializeField, Min(0)]
    private int m_MaxCount = 5;
    [SerializeField, Min(0f)]
    private float m_CountUpTime = 1f;
    [SerializeReference]
    private CooldownBase m_CoolDown = new TimeCooldown();

    private Rigidbody m_Rigidbody;
    private int m_Count = 0;
    private float m_CountTimer = 0f;
    private bool m_IsBlink = false;

    private void Awake()
    {
        m_Rigidbody = GetComponentInParent<Rigidbody>();
    }

    public void Init(MonoBehaviour owner)
    {
        m_CoolDown.m_Owner = owner;
        m_Count = m_MaxCount;
    }

    public void Action(InputAction.CallbackContext context)
    {
        if (!m_CoolDown.IsComplete() || m_Count <= 0 || m_IsBlink)
        {
            return;
        }

        m_CoolDown.StartCooldown();
        m_Count--;
        m_IsBlink = true;
    }

    public bool IsBlink()
    {
        if(m_IsBlink)
        {
            m_IsBlink = false;
            return true;
        }

        return false;
    }

    public void Update()
    {
        if (m_Count < m_MaxCount)
        {
            if (m_CountTimer + Time.deltaTime > m_CountUpTime)
            {
                m_Count++;

                if (m_Count >= m_MaxCount)
                {
                    m_CountTimer = 0;
                    return;
                }
            }

            m_CountTimer = Mathf.Repeat(m_CountTimer + Time.deltaTime, m_CountUpTime);
        }
    }

    private void FixedUpdate()
    {
        if(m_IsBlink)
        {
            m_Rigidbody.AddForce(transform.forward * m_Speed, ForceMode.VelocityChange);
            m_IsBlink = false;
        }
    }

    public void ShowState()
    {
        GUILayout.Label($"BlinkCountTimer: {m_CountTimer}");
        GUILayout.Label($"BlinkCount: {m_Count}");
    }
}
