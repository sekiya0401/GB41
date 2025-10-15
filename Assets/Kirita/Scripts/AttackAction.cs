using System.Collections;
using UnityEngine;

namespace Prototype.Games
{
    public class AttackAction : MonoBehaviour, IPlayerAction
    {
        private Player m_Player;
        private Collider m_AttackCollider;
        private bool m_IsAttacking = false;
        //HACK:çUåÇîÕàÕÇ∆çUåÇÇÃó¨ÇÍÇ™å©Ç¶ÇÈÇÊÇ§ä»à’ìIÇ…â¬éãâª
        private MeshRenderer m_Renderer;

        private void Awake()
        {
            TryGetComponent(out m_AttackCollider);
            m_AttackCollider.enabled = false;

            m_Player = GetComponentInParent<Player>();

            TryGetComponent(out m_Renderer);
            m_Renderer.enabled = false;
        }

        void IPlayerAction.Action(Player _player)
        {
            if(m_IsAttacking is false)
            {
                StartCoroutine(AttackFlow());
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Player player = other.GetComponentInParent<Player>();   
            if(player)
            {
                if(player == m_Player)
                {
                    return;
                }
                player.Heal(m_Player.State.Heal);
                return;
            }

            IDamagable damagable = other.GetComponentInParent<IDamagable>();
            if(damagable != null)
            {
                damagable.Damage(m_Player.State.Damage);

                m_Player.DefeatedEnemy();
            }
        }

        private IEnumerator AttackFlow()
        {
            m_IsAttacking = true;
            m_AttackCollider.enabled = true;
            m_Renderer.enabled = true;

            yield return new WaitForSeconds(m_Player.State.AttackDuration);

            m_AttackCollider.enabled=false;
            m_Renderer.enabled = false;

            yield return new WaitForSeconds(m_Player.State.AttackInterval);

            m_IsAttacking = false;
        }
    }
}