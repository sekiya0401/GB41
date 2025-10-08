using Prototype.ScriptableObjects;
using UnityEngine;

namespace Prototype.Games
{
    /// <summary>
    /// �v���C���[�̍U������
    /// </summary>
    public class PlayerAttack : MonoBehaviour
    {
        [SerializeField]
        private PlayerStateSO m_PlayerState;

        //HACK: Trigger�ɓ������I�u�W�F�N�g�ɑ΂��ă_���[�W�܂��͉񕜂�^����ȈՓI�Ȏ���
        private void OnTriggerEnter(Collider other)
        {
            // NOTE: �Ώۂ��q�I�u�W�F�N�g�̏ꍇ���l�����Đe�I�u�W�F�N�g�ƑΏۃI�u�W�F�N�g��������IDamageable���Ȃ����T��

            Player player = other.GetComponentInParent<Player>();
            if (player != null)
            {
                player.Heal(m_PlayerState.Heal);
                return;
            }

            IDamageable damageable = other.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                damageable.Damage(m_PlayerState.Damage);
            }
        }
    }
}