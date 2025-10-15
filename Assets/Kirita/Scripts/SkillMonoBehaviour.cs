using UnityEngine;

namespace Prototype.Games
{
    public abstract class SkillMonoBehaviour : MonoBehaviour
    {
        [SerializeField]
        protected int m_RequiredActivatePoint = 0;

        public int RequiredActivatePoint => m_RequiredActivatePoint;

        //HACK:Player�Ɉˑ����Ă���̂ŁA���ۃN���X�Ɉˑ�����d�g�݂ɂ����ق����ǂ�����
        public abstract void Activate(Player _player);
    }
}