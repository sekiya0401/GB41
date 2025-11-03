using MS.Systems.CoolDown;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MS.Games
{
    public class Bite : MonoBehaviour, IInputActionHandler
    {
        [SerializeField, Min(1)]
        private int m_MaxConsecutive = 1;
        [SerializeField]
        private AttackHandler m_AttackHandler;
        [SerializeField]
        private ReviveHandler m_ReviveHandler;
        [SerializeReference]
        private CooldownBase m_CoolDown = new TimeCooldown();
        [SerializeReference]
        private CooldownBase m_Consecutive = new TimeCooldown();
        [SerializeReference]
        private CooldownBase m_Stay = new TimeCooldown();

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
            m_Consecutive.m_Owner = this;
            m_Stay.m_Owner = this;
        }

        public void Action(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Started)
            {
                return;
            }

            if(m_ReviveHandler.IsRevivableTarget())
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
        /// 攻撃コルーチン
        /// </summary>
        /// <returns></returns>
        private IEnumerator Attack()
        {
            State = STATE.ACTION;
            m_ConsecutiveCount++;

            Debug.Log($"{m_ConsecutiveCount}撃目");
            m_AttackHandler.Attack();
            yield return new WaitForFixedUpdate();

            m_AttackHandler.Finish();
            if(m_ConsecutiveCount < m_MaxConsecutive)
            {
                State = STATE.STAY;
                m_Stay.StartCooldown();
                yield return new WaitUntil(() => m_Stay.IsComplete());

                State = STATE.CHAIN;
                m_Consecutive.StartCooldown();
                yield return new WaitUntil(() => m_Consecutive.IsComplete());

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
        private void OnGUI()
        {
            GUI.color = Color.red;
            GUI.skin.label.fontSize = 36;

            GUILayout.Label($"連続攻撃数: {m_ConsecutiveCount}");
            GUILayout.Label($"ステート: {State}");
        }

        private void OnDrawGizmosSelected()
        {
            Color color;
            if (State == STATE.COOLDOWN && m_ConsecutiveCount < m_MaxConsecutive)
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

