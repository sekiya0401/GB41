using Fusion;
using UnityEngine;

namespace Prototype.Games
{
    [RequireComponent(typeof(NetworkCharacterController))]
    public class SimplePlayer : NetworkBehaviour
    {
        private NetworkCharacterController m_NCC;

        public override void Spawned()
        {
            TryGetComponent(out m_NCC);
        }

        public override void FixedUpdateNetwork()
        {
            Vector3 move = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));

            m_NCC.Move(move);
        }
    }
}