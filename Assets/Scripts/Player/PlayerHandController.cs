using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerHandController : MonoBehaviour
{
    class PlayerAction
    {
        public ItemType item;
        public PlayerHandActionBase action;

        public PlayerAction(ItemType _item, PlayerHandActionBase _action)
        {
            item = _item;
            action = _action;
        }
    }

    List<PlayerAction> m_actions = new List<PlayerAction>();

    int m_leftHandActionIndex = -1;
    int m_rightHandActionIndex = -1;

    SubscriberList m_subscriberList = new SubscriberList();

    private void Awake()
    {
        m_subscriberList.Add(new Event<StartUseWeaponEvent>.LocalSubscriber(StartUseWeapon, gameObject));
        m_subscriberList.Add(new Event<EndUseWeaponEvent>.LocalSubscriber(EndUseWeapon, gameObject));
        m_subscriberList.Add(new Event<StartUseItemEvent>.LocalSubscriber(StartUseItem, gameObject));
        m_subscriberList.Add(new Event<EndUseItemEvent>.LocalSubscriber(EndUseItem, gameObject));

        m_subscriberList.Add(new Event<GetStatusEvent>.LocalSubscriber(GetStatus, gameObject));
        m_subscriberList.Add(new Event<GetOffsetVelocityEvent>.LocalSubscriber(GetVelocity, gameObject));
        m_subscriberList.Subscribe();

        m_actions.Add(new PlayerAction(ItemType.Sword, new PlayerHandActionSword(this)));
    }

    private void Start()
    {
        foreach (var a in m_actions)
            a.action.OnInit();
    }

    private void OnDestroy()
    {
        foreach (var a in m_actions)
            a.action.OnDestroy();

        m_subscriberList.Unsubscribe();
    }

    private void Update()
    {
        GetInputsEvent inputs = new GetInputsEvent();
        Event<GetInputsEvent>.Broadcast(inputs, gameObject);

        UpdateCurrentAction(ref m_leftHandActionIndex, inputs.useWeapon);
        UpdateCurrentAction(ref m_rightHandActionIndex, inputs.useItem);

        foreach (var a in m_actions)
            a.action.AlwaysProcess();
    }

    int GetActionIndex(ItemType item)
    {
        for(int i = 0; i < m_actions.Count; i++)
        {
            if (m_actions[i].item == item)
                return i;
        }

        return -1;
    }

    void UpdateCurrentAction(ref int index, bool inputPressed)
    {
        GetEquipementEvent equipementData = new GetEquipementEvent(EquipementSlot.LeftHand);
        Event<GetEquipementEvent>.Broadcast(equipementData);

        int actionIndex = -1;
        if (!equipementData.IsEmpty())
            actionIndex = GetActionIndex(equipementData.GetItem());

        if(index != actionIndex)
        {
            if(index >= 0)
                m_actions[index].action.EndProcess();
            index = actionIndex;
            if(index >= 0)
                m_actions[index].action.BeginProcess();
        }

        if(index >= 0)
            m_actions[index].action.Process(inputPressed);
    }

    void GetVelocity(GetOffsetVelocityEvent e)
    {
        GetVelocity(m_leftHandActionIndex, e);
        GetVelocity(m_rightHandActionIndex, e);
    }

    void GetVelocity(int index, GetOffsetVelocityEvent e)
    {
        if (index < 0)
            return;

        Vector2 velocity = Vector2.zero;
        float multiplier = 1;

        m_actions[index].action.GetVelocity(out velocity, out multiplier);

        e.offsetVelocity += velocity;
        e.velocityMultiplier *= multiplier;
    }

    void StartUseWeapon(StartUseWeaponEvent e)
    {
        if (m_leftHandActionIndex >= 0)
            m_actions[m_leftHandActionIndex].action.OnPress();
    }

    void EndUseWeapon(EndUseWeaponEvent e)
    {
        if (m_leftHandActionIndex >= 0)
            m_actions[m_leftHandActionIndex].action.OnPressEnd();
    }

    void StartUseItem(StartUseItemEvent e)
    {
        if (m_rightHandActionIndex >= 0)
            m_actions[m_rightHandActionIndex].action.OnPress();
    }

    void EndUseItem(EndUseItemEvent e)
    {
        if (m_rightHandActionIndex >= 0)
            m_actions[m_rightHandActionIndex].action.OnPressEnd();
    }

    void GetStatus(GetStatusEvent e)
    {
        if (m_rightHandActionIndex >= 0)
            e.lockActions |= m_actions[m_rightHandActionIndex].action.AreActionsLocked();
        if (m_leftHandActionIndex >= 0)
            e.lockActions |= m_actions[m_leftHandActionIndex].action.AreActionsLocked();
    }
}
