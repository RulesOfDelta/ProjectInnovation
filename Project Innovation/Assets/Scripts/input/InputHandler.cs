using System;
using System.Collections;
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
    private event ButtonCallback OnSwitchCb;
    private event ButtonActionCallback OnShieldCb;
    private event ButtonCallback OnHiddenCb;
    private PlayerInput input;

    private InputState state;
    public Vector2 Look => state.Look * CurrentSettings().lookSensitivity;
    public Vector2 Move => state.Move;
    public bool Moving => state.Move != Vector2.zero;
    public bool IsMouse => state.IsMouse;
    public bool ShieldPressed => state.ShieldPressed;

    private void Start()
    {
        inputEnabled = true;
        state = new InputState();
        input = GetComponent<PlayerInput>();
        state.IsMouse = input.currentControlScheme == mouseSettings.controlScheme;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDestroy()
    {
        ResetRumble();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
#if UNITY_EDITOR
    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            switch (Cursor.lockState)
            {
                case CursorLockMode.None:
                    Cursor.lockState = CursorLockMode.Locked;
                    break;
                case CursorLockMode.Locked:
                case CursorLockMode.Confined:
                    Cursor.lockState = CursorLockMode.None;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Cursor.visible = !Cursor.visible;
        }
        
    }
#endif

    private bool inputEnabled;
    public void HaltInput(float time)
    {
        inputEnabled = false;
        state.Move = Vector2.zero;
        state.Look = Vector2.zero;
        OnShieldCb?.Invoke(ButtonAction.Up);
        state.ShieldPressed = false;
        ResetRumble();

        StartCoroutine(Reenable());
        
        IEnumerator Reenable()
        {
            yield return new WaitForSeconds(time);
            inputEnabled = true;
        }
    }

    public void LockInput()
    {
        inputEnabled = false;
    }

    public void ReleaseInput()
    {
        inputEnabled = true;
        fireCb = null;
        waitForFire = false;
    }

    private bool waitForFire;
    private Action fireCb;
    
    public void HaltInputUntilFire(float minTime = 0.5f, Action tempFireCb = null)
    {
        inputEnabled = false;
        fireCb = tempFireCb;
        StartCoroutine(StartWaiting());

        IEnumerator StartWaiting()
        {
            yield return new WaitForSeconds(minTime);
            waitForFire = true;
        }
    }

    public void RegisterOnFire(ButtonCallback cb)
    {
        OnFireCb += cb;
    }

    public void DeregisterOnFire(ButtonCallback cb)
    {
        OnFireCb -= cb;
    }

    public void RegisterOnSwitch(ButtonCallback cb)
    {
        OnSwitchCb += cb;
    }

    public void DeregisterOnSwitch(ButtonCallback cb)
    {
        OnSwitchCb -= cb;
    }

    public void RegisterOnShield(ButtonActionCallback cb)
    {
        OnShieldCb += cb;
    }

    public void DeregisterOnShield(ButtonActionCallback cb)
    {
        OnShieldCb -= cb;
    }

    public void RegisterOnHidden(ButtonCallback cb)
    {
        OnHiddenCb += cb;
    }
    
    public void OnFire(InputAction.CallbackContext context)
    {
        if (!inputEnabled)
        {
            if (!waitForFire) return;
            inputEnabled = true;
            waitForFire = false;
            fireCb?.Invoke();
            fireCb = null;
            return;
        }
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

    public void OnSwitch(InputAction.CallbackContext context)
    {
        if (!inputEnabled) return;
        switch (CheckButton(context))
        {
            case ButtonAction.Down:
                OnSwitchHandler();
                break;
            case ButtonAction.Up:
            case ButtonAction.Invalid:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void OnSwitchHandler()
    {
        OnSwitchCb?.Invoke();
    }

    public void OnShield(InputAction.CallbackContext context)
    {
        if (!inputEnabled) return;
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
    
    public void OnHidden(InputAction.CallbackContext context)
    {
        if (!inputEnabled) return;
        OnHiddenCb?.Invoke();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!inputEnabled) return;
        state.Move = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        if (!inputEnabled) return;
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
        if (inputEnabled && !state.IsMouse)
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