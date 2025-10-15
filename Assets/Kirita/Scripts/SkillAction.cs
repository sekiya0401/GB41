using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Prototype.Games
{
    public class SkillAction : MonoBehaviour
    {
        [SerializeField]
        private SkillMonoBehaviour m_Skill;
        protected int m_CurrentPoint = 0;

        public bool Action(Player _player)
        {
            if(m_CurrentPoint >= m_Skill.RequiredActivatePoint)
            {
                Debug.Log($"Activate Skill {m_Skill.name}");
                Instantiate(m_Skill, transform).Activate(_player);
                m_CurrentPoint = 0;
                return true;
            }

            Debug.Log($"Inactivate Skill {m_Skill.name}");
            return false;
        }

        public void AddPoint(int point)
        {
            m_CurrentPoint += point;
        }

    }
}
