using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Vector2 = UnityEngine.Vector2;

public class InputHandler : MonoBehaviour
{
    public class InputState
    {
        public Vector2 move;
        public Vector2 look;
    }

    public delegate void ButtonCallback();

    private event ButtonCallback OnFireCb;
    
    public InputState State { get; private set; }

    private void Start()
    {
        State = new InputState();
    }

    public void RegisterOnFire(ButtonCallback cb)
    {
        OnFireCb += cb;
    }

    public void DeregisterOnFire(ButtonCallback cb)
    {
        OnFireCb -= cb;
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.ReadValueAsButton() && context.action.triggered)
        {
            OnFireCb?.Invoke();
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        State.move = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        State.look = context.ReadValue<Vector2>();
    }
}
