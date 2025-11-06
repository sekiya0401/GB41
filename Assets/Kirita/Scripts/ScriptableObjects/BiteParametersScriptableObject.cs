using UnityEngine;
using MS.SO.Variable;

namespace MS.Games
{
    [CreateAssetMenu(fileName = "BiteParameters", menuName = "Scriptable Objects/Games/BiteParameters")]
    public class BiteParameters : Parameters
    {
        [SerializeField]
        private IntVariable m_Damage;
        [Min(0)]
        public int m_MaxConsecutiveAttacksCount = 3;
        //[Min(0f)]
        //public float m_ConsecutiveAttacksInputWaitTime = 0.2f;
        //[Min(0f)]
        //public float m_ConsecutiveAttacksMotionWaitTime = 0.5f;
        //[Min(0f)]
        //public float m_Range = 15f;
        //[Min(0f)]
        //public float m_CoolDownTime = 2f;
        //[Min(0f)]
        //public float m_ReviewWaitTime = 3f;
    }
}
