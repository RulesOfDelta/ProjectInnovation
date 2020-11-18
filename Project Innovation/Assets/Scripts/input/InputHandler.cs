using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class InputHandler : MonoBehaviour
{
    public class InputState
    {
        public Vector2 Move;
        public Vector2 Look;
    }

    public delegate void ButtonCallback();

    private event ButtonCallback OnFireCb;
    
    public InputState State { get; private set; }
    public Vector2 Look => State.Look;
    public Vector2 Move => State.Move;
    public bool Moving => State.Move != Vector2.zero;

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
        State.Move = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        State.Look = context.ReadValue<Vector2>();
    }
}
