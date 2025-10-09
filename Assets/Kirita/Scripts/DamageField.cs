using UnityEngine;

namespace Prototype.Games
{
    /// <summary>
    /// �_���[�W�t�B�[���h
    /// HACK: Trigger�ɓ������I�u�W�F�N�g�ɑ΂��ă_���[�W��^����ȈՓI�Ȏ���
    /// </summary>
    public class DamageField : MonoBehaviour
    {
        [SerializeField, Range(-100, 100)]
        private int m_Damage = 10;

        private void OnTriggerEnter(Collider other)
        {
            //NOTE: �Ώۂ��q�I�u�W�F�N�g�̏ꍇ���l�����Đe�I�u�W�F�N�g�ƑΏۃI�u�W�F�N�g��������IDamagable���Ȃ����T��
            var damageable = other.GetComponentInParent<IDamagable>();
            if (damageable != null)
            {
                damageable.Damage(m_Damage);
            }
        }
    }
}