using UnityEngine;

namespace Prototype.Games
{
    public abstract class SkillMonoBehaviour : MonoBehaviour
    {
        [SerializeField]
        protected float m_RequiredActivatePoint = 0;
        [SerializeField]
        protected Sprite m_Icon;

        public float RequiredActivatePoint => m_RequiredActivatePoint;
        public Sprite Icon => m_Icon;

        //HACK:Player�Ɉˑ����Ă���̂ŁA���ۃN���X�Ɉˑ�����d�g�݂ɂ����ق����ǂ�����
        public abstract void Activate(Player _player);
    }
}