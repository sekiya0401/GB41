using MS.Extensions;
using UnityEngine;
using UnityEngine.InputSystem;

public class ColliderCheck : MonoBehaviour
{
    [SerializeField]
    private InputActionReference m_InputRef;

    private void Awake()
    {
        m_InputRef.action.Enable();
    }

    private void OnEnable()
    {
        m_InputRef.action.AddPhaseCallbacks(OnAction, InputActionExtensions.PHASE.STARTED);
    }

    private void OnDisable()
    {
        m_InputRef.action.AddPhaseCallbacks(OnAction, InputActionExtensions.PHASE.STARTED);
    }

    private void OnAction(InputAction.CallbackContext context)
    {
        var colliders = Physics.OverlapSphere(transform.position, 100);
        foreach (var collider in colliders)
        {
            Debug.Log($"{collider.name}");
            Debug.Log($"{Vector3.Distance(transform.position, collider.transform.position)}");
        }
    }
}
