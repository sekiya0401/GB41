using UnityEngine;
using UnityEngine.Rendering;
using System;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Prototype.Games
{
    public class SkillAction : MonoBehaviour
    {
        [SerializeField]
        private SkillMonoBehaviour m_Skill;
        private SkillMonoBehaviour m_LastSkill;
        [SerializeField]
        private SpriteEventChannelScriptableObject m_SkillIconEventChannel;
        [SerializeField]
        private FloatEventChannelScriptableObject m_SkillPointEventChannel;
        protected float m_CurrentPoint = 0;

        private void OnEnable()
        {
            m_SkillIconEventChannel.Invoke(m_Skill.Icon);
            m_SkillPointEventChannel.Invoke(0f);
        }

        public bool Action(Player _player)
        {
            if(m_CurrentPoint >= m_Skill.RequiredActivatePoint)
            {
                Debug.Log($"Activate Skill {m_Skill.name}");
                Instantiate(m_Skill, transform).Activate(_player);
                m_CurrentPoint = 0;
                UpdateSkillPoint(0f);
                return true;
            }

            Debug.Log($"Inactivate Skill {m_Skill.name}");
            return false;
        }

        public void AddPoint(float point)
        {
            m_CurrentPoint += point;

            float rate = Mathf.Clamp01(m_CurrentPoint / m_Skill.RequiredActivatePoint);
            UpdateSkillPoint(rate);
        }

        private void UpdateSkillPoint(float rate)
        {
            m_SkillPointEventChannel.Invoke(rate);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if(Application.isPlaying)
            {
                if (m_Skill != m_LastSkill)
                {
                    m_LastSkill = m_Skill;
                    m_SkillIconEventChannel.Invoke(m_Skill.Icon);
                }
            }
        }
#endif
    }
}
