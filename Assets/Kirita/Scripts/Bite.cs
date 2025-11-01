using MS.Systems.CoolDown;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MS.Games
{
    public class Bite : MonoBehaviour, IInputActionHandler
    {
        [SerializeField, Min(0f)]
        private float m_Duration = 0f;
        [SerializeField, Min(0)]
        private int m_Damage = 1;
        [SerializeField]
        private InteractionDetector m_InteractDetector;
        [SerializeField]
        private Collider m_BiteRange;
        private bool m_IsAttacking = false;

        [SerializeReference]
        private CooldownBase m_CoolDown = new TimeCooldown();

        private void Awake()
        {
            m_BiteRange.enabled = false;
            m_CoolDown.m_Owner = this;
        }

        public void Action(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Started ||
                m_IsAttacking)
            {
                return;
            }

            if (m_InteractDetector.IsActivatableTarget())
            {
                m_InteractDetector.EnableActivatableTarget();
            }
            else
            {
                StartCoroutine(Attack());
            }
        }

        private IEnumerator Attack()
        {
            m_IsAttacking = true;
            m_BiteRange.enabled = true;
            yield return new WaitForSeconds(m_Duration);
            m_BiteRange.enabled = false;
            m_CoolDown.StartCooldown();
            yield return m_CoolDown.IsComplete();
            m_IsAttacking = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out IDamagable damagable))
            {
                damagable.Damage(m_Damage);
            }
        }
    }

}

