using UnityEngine.InputSystem;

namespace MS.Games
{
    public interface IInputActionHandler
    {
        public void Action(InputAction.CallbackContext context);
    }
}

