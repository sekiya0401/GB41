using MS.Systems;
using MS.Systems.CoolDown;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MS.Games
{
    public class ReviveHandler : MonoBehaviour
    {
        [SerializeReference]
        private CooldownBase m_Revive = new TimeCooldown();
        private HashSet<IActivatable> m_ReviveTargetHashSet = new();

        private void Awake()
        {
            m_Revive.m_Owner = this;
        }

        public bool IsRevivableTarget()
        {
            return m_ReviveTargetHashSet.Count > 0;
        }

        public void Revive(Bite owner)
        {
            StartCoroutine(WaitReviveFinish(owner));
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out IActivatable activatable)
                && activatable.IsDisabled())
            {
                m_ReviveTargetHashSet.Add(activatable);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out IActivatable activatable))
            {
                m_ReviveTargetHashSet.Remove(activatable);
            }
        }

        private IEnumerator WaitReviveFinish(Bite owner)
        {
            owner.State = Bite.STATE.ACTION;
            m_Revive.StartCooldown();
            HashSet<IActivatable> copy = new(m_ReviveTargetHashSet);

            yield return new WaitUntil(() => m_Revive.IsComplete());

            foreach (IActivatable activatable in copy)
            {
                activatable.Enable();
            }
            copy.Clear();

            owner.State = Bite.STATE.WAIT;
        }
    }
}