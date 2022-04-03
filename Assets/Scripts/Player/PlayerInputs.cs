using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour
{
    const string InputsName = "Player";
    const string MoveName = "Move";
    const string RollName = "Roll";
    const string UseName = "Use";
    const string InteractName = "Interact";

    PlayerInput m_inputs;

    Vector2 m_direction = Vector2.zero;
    bool m_roll = false;
    bool m_use = false;
    bool m_interact = false;

    SubscriberList m_subscriberList = new SubscriberList();

    private void Awake()
    {
        m_inputs = GetComponent<PlayerInput>();
        if (m_inputs)
            m_inputs.onActionTriggered += OnInput;

        m_subscriberList.Add(new Event<GetInputsEvent>.LocalSubscriber(GetInputs, gameObject));
        m_subscriberList.Subscribe();
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    void OnInput(InputAction.CallbackContext e)
    {
        if (e.action == null)
            return;
        if (e.action.actionMap == null)
            return;
        if (e.action.actionMap.name != InputsName)
            return;

        //update current device
        string deviceClass = e.control.device.description.deviceClass;
        InputType type = Settings.instance.inputType;

        if (deviceClass == "Mouse" || deviceClass == "Keyboard")
        {
            if (type != InputType.Keyboard)
                Settings.instance.inputType = InputType.Keyboard;
        }
        else if (type != InputType.Gamepad)
            Settings.instance.inputType = InputType.Gamepad;

        if (e.action.name == MoveName)
        {
            if (e.phase == InputActionPhase.Started || e.phase == InputActionPhase.Performed)
            {
                m_direction = e.ReadValue<Vector2>();
            }
            else if (e.phase == InputActionPhase.Disabled || e.phase == InputActionPhase.Canceled)
            {
                m_direction = Vector2.zero;
            }
        }
        else if (e.action.name == RollName)
        {
            if (e.phase == InputActionPhase.Started)
            {
                m_roll = true;
                Event<StartRollEvent>.Broadcast(new StartRollEvent(), gameObject, true);
            }
            else if (e.phase == InputActionPhase.Canceled)
            {
                m_roll = false;
                Event<EndRollEvent>.Broadcast(new EndRollEvent(), gameObject, true);
            }
        }
        else if (e.action.name == UseName)
        {
            if (e.phase == InputActionPhase.Started)
            {
                m_use = true;
                Event<StartUseEvent>.Broadcast(new StartUseEvent(), gameObject, true);
            }
            else if (e.phase == InputActionPhase.Canceled)
            {
                m_roll = false;
                Event<EndUseEvent>.Broadcast(new EndUseEvent(), gameObject, true);
            }
        }
        else if(e.action.name == InteractName)
        {
            if(e.phase == InputActionPhase.Started)
            {
                m_interact = true;
                Event<StartInteractEvent>.Broadcast(new StartInteractEvent(), gameObject, true);
            }
            else if(e.phase == InputActionPhase.Canceled)
            {
                m_interact = false;
                Event<EndInteractEvent>.Broadcast(new EndInteractEvent(), gameObject, true);
            }
        }
    }

    void GetInputs(GetInputsEvent e)
    {
        e.direction = m_direction;
        e.roll = m_roll;
        e.use = m_use;
        e.interact = m_interact;
    }
}