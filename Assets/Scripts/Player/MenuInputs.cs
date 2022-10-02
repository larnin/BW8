using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuInputs : MonoBehaviour
{
    const string InputsName = "UI";
    const string NavigateName = "Navigate";
    const string SubmitName = "Submit";
    const string CancelName = "Cancel";
    const string PointName = "Point";
    const string ScrollWheelName = "ScrollWheel";
    const string ClickName = "Click";
    const string MiddleClickName = "MiddleClick";
    const string RightClickName = "RightClick";

    SubscriberList m_subscriberList = new SubscriberList();

    PlayerInput m_inputs;

    Vector2 m_navigation = Vector2.zero;
    bool m_submit = false;
    bool m_cancel = false;
    Vector2 m_point = Vector2.zero;
    Vector2 m_scroll = Vector2.zero;
    bool m_click = false;
    bool m_middleClick = false;
    bool m_rightClick = false;

    private void Awake()
    {
        m_inputs = GetComponent<PlayerInput>();
        if (m_inputs)
            m_inputs.onActionTriggered += OnInput;

        m_subscriberList.Add(new Event<GetUIInputsEvent>.Subscriber(GetInputs));
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

        if(e.action.name == NavigateName)
        {
            if (e.phase == InputActionPhase.Started || e.phase == InputActionPhase.Performed)
                m_navigation = e.ReadValue<Vector2>();
            else if (e.phase == InputActionPhase.Disabled || e.phase == InputActionPhase.Canceled)
                m_navigation = Vector2.zero;
        }
        else if(e.action.name == SubmitName)
        {
            if (e.phase == InputActionPhase.Started)
            {
                m_submit = true;
                Event<StartSubmitEvent>.Broadcast(new StartSubmitEvent());
            }
            else if (e.phase == InputActionPhase.Canceled)
            {
                m_submit = false;
                Event<EndSubmitEvent>.Broadcast(new EndSubmitEvent());
            }
        }
        else if (e.action.name == CancelName)
        {
            if (e.phase == InputActionPhase.Started)
            {
                m_cancel = true;
                Event<StartCancelEvent>.Broadcast(new StartCancelEvent());
            }
            else if (e.phase == InputActionPhase.Canceled)
            {
                m_cancel = false;
                Event<EndCancelEvent>.Broadcast(new EndCancelEvent());
            }
        }
        else if (e.action.name == PointName)
        {
            if (e.phase == InputActionPhase.Started || e.phase == InputActionPhase.Performed)
                m_point = e.ReadValue<Vector2>();
            else if (e.phase == InputActionPhase.Disabled || e.phase == InputActionPhase.Canceled)
                m_point = Vector2.zero;
        }
        else if (e.action.name == ScrollWheelName)
        {
            if (e.phase == InputActionPhase.Started || e.phase == InputActionPhase.Performed)
                m_scroll = e.ReadValue<Vector2>();
            else if (e.phase == InputActionPhase.Disabled || e.phase == InputActionPhase.Canceled)
                m_scroll = Vector2.zero;
        }
        else if (e.action.name == ClickName)
        {
            if (e.phase == InputActionPhase.Started)
            {
                m_click = true;
                Event<StartClickEvent>.Broadcast(new StartClickEvent());
            }
            else if (e.phase == InputActionPhase.Canceled)
            {
                m_click = false;
                Event<EndClickEvent>.Broadcast(new EndClickEvent());
            }
        }
        else if (e.action.name == MiddleClickName)
        {
            if (e.phase == InputActionPhase.Started)
            {
                m_middleClick = true;
                Event<StartMiddleClickEvent>.Broadcast(new StartMiddleClickEvent());
            }
            else if (e.phase == InputActionPhase.Canceled)
            {
                m_middleClick = false;
                Event<EndMiddleClickEvent>.Broadcast(new EndMiddleClickEvent());
            }
        }
        else if (e.action.name == RightClickName)
        {
            if (e.phase == InputActionPhase.Started)
            {
                m_rightClick = true;
                Event<StartRightClickEvent>.Broadcast(new StartRightClickEvent());
            }
            else if (e.phase == InputActionPhase.Canceled)
            {
                m_rightClick = false;
                Event<EndRightClickEvent>.Broadcast(new EndRightClickEvent());
            }
        }
    }

    void GetInputs(GetUIInputsEvent e)
    {
        e.navigation = m_navigation;
        e.submit = m_submit;
        e.cancel = m_cancel;
        e.point = m_point;
        e.scroll = m_scroll;
        e.click = m_click;
        e.middleClick = m_middleClick;
        e.rightClick = m_rightClick;
    }
}