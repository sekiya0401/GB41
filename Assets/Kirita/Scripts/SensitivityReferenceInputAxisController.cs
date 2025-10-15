using UnityEngine;
using Unity.Cinemachine;
using System;
using UnityEngine.InputSystem;

//The component that you will add to your CinemachineCamera.
public class SensitivityReferenceInputAxisController : InputAxisControllerBase<SensitivityReferenceInputAxisController.SensitivityReferenceReader>
{
    void Update()
    {
        if (Application.isPlaying)
            UpdateControllers();
    }

    [Serializable]
    public class SensitivityReferenceReader : IInputAxisReader
    {
        public InputActionReference m_LookAction;
        public Vector2VariableScriptableObject m_Sensitivity;
        float IInputAxisReader.GetValue(UnityEngine.Object context, IInputAxisOwner.AxisDescriptor.Hints hint)
        {
            float value = hint switch
            {
                IInputAxisOwner.AxisDescriptor.Hints.X => m_Sensitivity.Value.x,
                IInputAxisOwner.AxisDescriptor.Hints.Y => m_Sensitivity.Value.y,
                _ => throw new NotImplementedException()
            };


            return ReadInput(m_LookAction, hint, context) * value;
        }

        /// <summary>
        /// Definition of how we read input. Override this in your child classes to specify
        /// the InputAction's type to read if it is different from float or Vector2.
        /// </summary>
        /// <param name="action">The action being read.</param>
        /// <param name="hint">The axis hint of the action.</param>
        /// <param name="context">The owner object, can be used for logging diagnostics</param>
        /// <param name="defaultReader">Not used</param>
        /// <returns>Returns the value of the input device.</returns>
        float ReadInput(InputAction action, IInputAxisOwner.AxisDescriptor.Hints hint, UnityEngine.Object context)
        {
            var control = action.activeControl;
            if (control != null)
            {
                try
                {
                    // If we can read as a Vector2, do so
                    if (control.valueType == typeof(Vector2) || action.expectedControlType == "Vector2")
                    {
                        var value = action.ReadValue<Vector2>();
                        return hint == IInputAxisOwner.AxisDescriptor.Hints.Y ? value.y : value.x;
                    }
                    // Default: assume type is float
                    return action.ReadValue<float>();
                }
                catch (InvalidOperationException)
                {
                    Debug.LogError($"An action in {context.name} is mapped to a {control.valueType.Name} "
                        + "control.  CinemachineInputAxisController.Reader can only handle float or Vector2 types.  "
                        + "To handle other types you can install a custom handler for "
                        + "CinemachineInputAxisController.ReadControlValueOverride.");
                }
            }
            return 0f;
        }
    }
}