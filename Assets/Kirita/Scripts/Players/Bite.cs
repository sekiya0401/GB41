using MS.Systems.CoolDown;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MS.Games
{
    public class Bite : MonoBehaviour, IInputActionHandler
    {
        [SerializeField]
        private BiteParameters m_Parameters;
        [SerializeField]
        private AttackHandler m_AttackHandler;
        [SerializeField]
        private ReviveHandler m_ReviveHandler;
        [SerializeReference]
        private CooldownBase m_CoolDown = new TimeCooldown();
        [SerializeReference]
        private CooldownBase m_ConsecutiveAttacksMotionWait = new TimeCooldown();
        [SerializeReference]
        private CooldownBase m_ConsecutiveAttacksInputWait = new TimeCooldown();

        private int m_ConsecutiveCount = 0;
        public enum STATE
        {
            WAIT,
            CHAIN,
            STAY,
            ACTION,
            COOLDOWN
        }
        public STATE State
        {
            get;
            set;
        } = STATE.WAIT;

        private Coroutine m_AttackCoroutine;

        private void Awake()
        {
            m_CoolDown.m_Owner = this;
            m_ConsecutiveAttacksMotionWait.m_Owner = this;
            m_ConsecutiveAttacksInputWait.m_Owner = this;
        }

        public void Action(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Started)
            {
                return;
            }

            if(m_ReviveHandler.IsRevivableTarget)
            {
                m_ReviveHandler.Revive(this);

                Debug.Log("Start Revive");
            }

            if (State == STATE.WAIT || State == STATE.CHAIN)
            {
                if (m_AttackCoroutine != null)
                {
                    StopCoroutine(m_AttackCoroutine);
                    m_AttackCoroutine = null;
                }

                m_AttackCoroutine = StartCoroutine(Attack());

                Debug.Log("Start Attack");
            }
        }

        /// <summary>
        /// çUåÇÉRÉãÅ[É`Éì
        /// </summary>
        /// <returns></returns>
        private IEnumerator Attack()
        {
            State = STATE.ACTION;
            m_ConsecutiveCount++;

            Debug.Log($"{m_ConsecutiveCount}åÇñ⁄");
            m_AttackHandler.Attack();
            yield return new WaitForFixedUpdate();

            m_AttackHandler.Finish();
            if(m_ConsecutiveCount < m_Parameters.m_MaxConsecutiveAttacksCount)
            {
                State = STATE.STAY;
                m_ConsecutiveAttacksInputWait.StartCooldown();
                yield return new WaitUntil(() => m_ConsecutiveAttacksInputWait.IsComplete());

                State = STATE.CHAIN;
                m_ConsecutiveAttacksMotionWait.StartCooldown();
                yield return new WaitUntil(() => m_ConsecutiveAttacksMotionWait.IsComplete());

                yield return StartCoroutine(StartCooldown());
            }
            else
            {
                yield return StartCoroutine(StartCooldown());
            }
        }

        private IEnumerator StartCooldown()
        {
            State = STATE.COOLDOWN;
            m_CoolDown.StartCooldown();

            yield return new WaitUntil(() => m_CoolDown.IsComplete());
            m_ConsecutiveCount = 0;
            State = STATE.WAIT;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Color color;
            if (State == STATE.COOLDOWN && m_ConsecutiveCount < m_Parameters.m_MaxConsecutiveAttacksCount)
            {
                color = Color.magenta;
            }
            else
            {
                color = m_ConsecutiveCount switch
                {
                    0 => Color.white,
                    1 => Color.yellow,
                    2 => Color.blue,
                    3 => Color.red,
                    _ => Color.green,
                };
            }
            Gizmos.color = color;
            Gizmos.DrawWireCube(transform.position, Vector3.one * 2f);
        }
#endif
    }

}

