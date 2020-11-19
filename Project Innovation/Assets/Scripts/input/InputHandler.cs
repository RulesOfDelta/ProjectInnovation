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

    public delegate void ButtonCallback();

    [SerializeField] private InputSettings mouseSettings = InputSettings.Default();
    [SerializeField] private InputSettings gamepadSettings = InputSettings.Default();

    private event ButtonCallback OnFireCb;
    private PlayerInput input;

    private InputState state;
    public Vector2 Look => state.Look * CurrentSettings().lookSensitivity;
    public Vector2 Move => state.Move;
    public bool Moving => state.Move != Vector2.zero;
    public bool IsMouse => state.IsMouse;

    private void Start()
    {
        state = new InputState();
        input = GetComponent<PlayerInput>();
        state.IsMouse = input.currentControlScheme == mouseSettings.controlScheme;
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
        state.Move = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        state.Look = context.ReadValue<Vector2>();
    }

    public void OnControlsChanged()
    {
        state.IsMouse = input.currentControlScheme == mouseSettings.controlScheme;
    }

    private InputSettings CurrentSettings()
    {
        return state.IsMouse ? mouseSettings : gamepadSettings;
    }
}