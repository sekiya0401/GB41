using UnityEngine;

namespace Prototype.Games
{
    public abstract class SkillMonoBehaviour : MonoBehaviour
    {
        [SerializeField]
        protected int m_RequiredActivatePoint = 0;

        public int RequiredActivatePoint => m_RequiredActivatePoint;

        //HACK:Playerに依存しているので、抽象クラスに依存する仕組みにしたほうが良いかも
        public abstract void Activate(Player _player);
    }
}