using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Prototype.Games
{
    public class BindFieldSkill : SkillMonoBehaviour
    {
        [SerializeField]
        private float m_Radius = 10f;
        [SerializeField,Range(1,40)]
        private int m_BindTime = 20;
        [SerializeField, Range(0f, 1f)]
        private float m_Slow = 0.5f;
        [SerializeField]
        private NetworkObject m_AvatarPrefab;
        private NetworkObject m_Avatar;
        private Player m_Player;
        private Dictionary<NavMeshAgent, float> m_DetainedAgent;
        private float m_Timer = 0;
        private bool m_IsRunning = true;

        public override void Activate(Player _player)
        {
            transform.SetParent(null);
            m_DetainedAgent = new Dictionary<NavMeshAgent, float>();

            m_Avatar = _player.Runner.Spawn(m_AvatarPrefab);
            m_Avatar.transform.SetParent(transform);
            m_Avatar.transform.localPosition = Vector3.zero;
            m_Avatar.transform.localScale = Vector3.one * m_Radius;
            m_Player = _player;
        }

        private void Update()
        {
            m_Timer += Time.deltaTime;
            if (m_Timer >= m_BindTime)
            {
                m_IsRunning = true;
                foreach (var agent in m_DetainedAgent)
                {
                    if(agent.Key is null)
                    {
                        continue;
                    }

                    agent.Key.speed = agent.Value;
                }

                m_Player.Runner.Despawn(m_Avatar);

                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if(!m_IsRunning)
            {
                return;
            }

            var nav = other.GetComponentInParent<NavMeshAgent>();
            if(nav)
            {
                m_DetainedAgent.Add(nav, nav.speed);
                nav.speed *= m_Slow;
            }
        }
    }
}