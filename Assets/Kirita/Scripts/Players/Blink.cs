using MS.Games;
using MS.Systems.CoolDown;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Blink : MonoBehaviour, IInputActionHandler
{
    [SerializeField]
    private BlinkParameters m_Parameters;
    [SerializeReference]
    private CooldownBase m_CoolDown = new TimeCooldown();

    private Rigidbody m_Rigidbody = null;
    private bool m_IsBlink = false;
    private Coroutine m_RestoreCoroutine = null;


    private void Awake()
    {
        m_Rigidbody = GetComponentInParent<Rigidbody>();

        m_CoolDown.m_Owner = this;
    }

    private void Start()
    {
        m_Parameters.Count = m_Parameters.MaxCount;
    }

    private void FixedUpdate()
    {
        if(m_IsBlink)
        {
            m_Rigidbody.AddForce(transform.forward * m_Parameters.m_Speed, ForceMode.VelocityChange);
            m_IsBlink = false;
            
            if(m_RestoreCoroutine is null)
            {
                m_RestoreCoroutine = StartCoroutine(RestoreBlinkCount());
            }
        }
    }

    public void Action(InputAction.CallbackContext context)
    {
        if (!m_CoolDown.IsComplete() || m_Parameters.Count <= 0 || m_IsBlink)
        {
            return;
        }

        m_CoolDown.StartCooldown();
        m_Parameters.Count--;
        m_IsBlink = true;
    }

    /// <summary>
    /// ブリンク使用回数の回復
    /// </summary>
    /// <returns></returns>
    private IEnumerator RestoreBlinkCount()
    {
        yield return new WaitForSeconds(m_Parameters.m_CountRestorationTime);

        m_Parameters.Count++;

        //ブリンク回数が最大数じゃなかったら
        if(m_Parameters.Count < m_Parameters.MaxCount)
        {
            StartCoroutine(RestoreBlinkCount());
        }
        else
        {
            m_RestoreCoroutine = null;  
        }
    }
}
