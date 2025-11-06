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

        public bool IsRevivableTarget => m_ReviveTargetHashSet.Count > 0;

        private void Awake()
        {
            m_Revive.m_Owner = this;
        }

        public void Revive(Bite owner)
        {
            StartCoroutine(WaitReviveFinish(owner));
        }

        private void OnTriggerEnter(Collider other)
        {
            IActivatable activatable = other.GetComponentInParent<IActivatable>();
            if (activatable != null && activatable.IsDisabled())
            {
                m_ReviveTargetHashSet.Add(activatable);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            IActivatable activatable = other.GetComponentInParent<IActivatable>();
            if (activatable != null)
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

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if(IsRevivableTarget)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(transform.position, Vector3.one * 5f);
            }
        }
#endif
    }
}