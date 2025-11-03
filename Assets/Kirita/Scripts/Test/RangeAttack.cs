using MS.Games.Skills;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace MS.Games
{
    public class RangeAttack : PlayerBaseSkill, IInputActionHandler
    {
        [SerializeField,Range(0,10)]
        private int m_Damage = 3;
        [SerializeField]
        private Image m_Active;
        private Collider m_AttackArea;

        private void Awake()
        {
            m_CoolDownClass.m_Owner = this;
            if(TryGetComponent(out m_AttackArea))
            {
                m_AttackArea.enabled = false;
            }
        }

        public void Action(InputAction.CallbackContext context)
        {
            if(context.started)
            {
                ActivateSkill();
            }
        }

        protected override void OnSkillEnd()
        {
            m_Active.enabled = true;
        }

        protected override void OnSkillStart()
        {
            m_Active.enabled = false;
            StartCoroutine(Attack());
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.TryGetComponent(out IDamagable damagable))
            {
                damagable.Damage(m_Damage);
            }    
        }

        private IEnumerator Attack()
        {
            if (m_AttackArea)
            {
                m_AttackArea.enabled = true;
                Debug.Log("<color=yellow>範囲攻撃開始！</color>");
            }
            yield return new WaitForFixedUpdate();
            if (m_AttackArea)
            {
                m_AttackArea.enabled = false;
                Debug.Log("<color=yellow>範囲攻撃終了！</color>");
            }
            yield return new WaitUntil(() => m_CoolDownClass.IsComplete());
            EndSkill();
        }
    }
}