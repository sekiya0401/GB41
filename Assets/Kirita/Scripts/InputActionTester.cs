using MS.Extensions;
using TNRD;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MS.Games
{
    public class InputActionTester : MonoBehaviour
    {
        [SerializeField]
        private InputActionReference m_ActionReference;
        [SerializeField]
        private SerializableInterface<IInputActionHandler> m_AttackActionHandler;

        private void Awake()
        {
            m_ActionReference.action.Enable();
        }

        private void OnEnable()
        {
            m_ActionReference.action.AddAllPhaseCallbacks(OnTest);
        }

        private void OnDisable()
        {
            m_ActionReference.action.RemoveAllPhaseCallbacks(OnTest);
        }

        private void OnTest(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    m_AttackActionHandler.Value.Action(context);
                    Debug.Log($"<color=red>OnTest: Started</color>");
                    break;
                case InputActionPhase.Performed:
                    Debug.Log($"<color=blue>OnTest: Performed</color>");
                    break;
                case InputActionPhase.Canceled:
                    Debug.Log($"<color=yellow>OnTest: Canceled</color>");
                    break;
                case InputActionPhase.Waiting:
                    Debug.Log($"<color=gray>OnTest: Waiting</color>");
                    break;
            }
        }
    }

}