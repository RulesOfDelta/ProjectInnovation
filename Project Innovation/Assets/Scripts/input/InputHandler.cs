using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputHandler : MonoBehaviour
{
    private class InputState
    {
        public Vector2 Move;
        public Vector2 Look;
        
        public bool ShieldPressed;
        
        public bool IsMouse;
    }

    [Serializable]
    public struct InputSettings
    {
        public string controlScheme;
        public float lookSensitivity;

        public InputSettings(string controlScheme, float lookSensitivity)
        {
            this.controlScheme = controlScheme;
            this.lookSensitivity = lookSensitivity;
        }

        public static InputSettings Default()
        {
            return new InputSettings("<NONE>", 1f);
        }
    }

    public enum ButtonAction
    {
        Down, Up, Invalid
    }

    public delegate void ButtonCallback();

    public delegate void ButtonActionCallback(ButtonAction action);

    [SerializeField] private InputSettings mouseSettings = InputSettings.Default();
    [SerializeField] private InputSettings gamepadSettings = InputSettings.Default();

    [SerializeField, Range(0f, 1f)] private float rumbleStrength = .5f;

    private event ButtonCallback OnFireCb;
    private event ButtonCallback OnSwordCb;
    private event ButtonActionCallback OnShieldCb;
    private PlayerInput input;

    private InputState state;
    public Vector2 Look => state.Look * CurrentSettings().lookSensitivity;
    public Vector2 Move => state.Move;
    public bool Moving => state.Move != Vector2.zero;
    public bool IsMouse => state.IsMouse;
    public bool ShieldPressed => state.ShieldPressed;

    private void Start()
    {
        state = new InputState();
        input = GetComponent<PlayerInput>();
        state.IsMouse = input.currentControlScheme == mouseSettings.controlScheme;
    }

    private void OnDestroy()
    {
        ResetRumble();
    }

    public void RegisterOnFire(ButtonCallback cb)
    {
        OnFireCb += cb;
    }

    public void DeregisterOnFire(ButtonCallback cb)
    {
        OnFireCb -= cb;
    }

    public void RegisterOnSword(ButtonCallback cb)
    {
        OnSwordCb += cb;
    }

    public void DeregisterOnSword(ButtonCallback cb)
    {
        OnSwordCb -= cb;
    }

    public void RegisterOnShield(ButtonActionCallback cb)
    {
        OnShieldCb += cb;
    }

    public void DeregisterOnShield(ButtonActionCallback cb)
    {
        OnShieldCb -= cb;
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        switch (CheckButton(context))
        {
            case ButtonAction.Down:
                OnFireCb?.Invoke();
                break;
            case ButtonAction.Up:
            case ButtonAction.Invalid:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void OnSword(InputAction.CallbackContext context)
    {
        switch (CheckButton(context))
        {
            case ButtonAction.Down:
                OnSwordCb?.Invoke();
                break;
            case ButtonAction.Up:
            case ButtonAction.Invalid:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void OnShield(InputAction.CallbackContext context)
    {
        var action = CheckButton(context);
        switch (action)
        {
            case ButtonAction.Down:
                OnShieldCb?.Invoke(action);
                state.ShieldPressed = true;
                break;
            case ButtonAction.Up:
                OnShieldCb?.Invoke(action);
                state.ShieldPressed = false;
                break;
            case ButtonAction.Invalid:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        state.Move = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        state.Look = context.ReadValue<Vector2>();
    }

    public void OnControlsChanged()
    {
        ResetRumble();
        state.IsMouse = input.currentControlScheme == mouseSettings.controlScheme;
    }

    private void RumbleInternal(float lowFreq, float highFreq)
    {
        Gamepad.current?.SetMotorSpeeds(Mathf.Clamp01(lowFreq) * rumbleStrength,
            Mathf.Clamp01(highFreq) * rumbleStrength);
    }

    public void ResetRumble()
    {
        RumbleInternal(0f, 0f);
    }
    
    public void Rumble(float lowFreq, float highFreq)
    {
        if (!state.IsMouse)
            RumbleInternal(lowFreq, highFreq);
    }

    private InputSettings CurrentSettings()
    {
        return state.IsMouse ? mouseSettings : gamepadSettings;
    }

    private ButtonAction CheckButton(InputAction.CallbackContext context)
    {
        if (context.action.triggered)
        {
            return context.ReadValueAsButton() ? ButtonAction.Down : ButtonAction.Up;
        }

        return ButtonAction.Invalid;
    }
}