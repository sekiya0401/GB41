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

        //HACK:Playerに依存しているので、抽象クラスに依存する仕組みにしたほうが良いかも
        public abstract void Activate(Player _player);
    }
}