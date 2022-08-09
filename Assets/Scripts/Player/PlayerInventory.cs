using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum ItemType
{
    Heart,
    Money,
}

class PlayerInventory : MonoBehaviour
{
    int m_money = 10;

    SubscriberList m_subscriberList = new SubscriberList();

    private void Awake()
    {
        m_subscriberList.Add(new Event<PickupEvent>.LocalSubscriber(Pickup, gameObject));
        m_subscriberList.Add(new Event<GetPlayerMoneyEvent>.Subscriber(GetMoney));

        m_subscriberList.Subscribe();
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    public void Pickup(PickupEvent e)
    {
        switch(e.type)
        {
            case ItemType.Heart:
                Event<RegenLifeEvent>.Broadcast(new RegenLifeEvent(e.stack), gameObject);
                break;
            case ItemType.Money:
                m_money += e.stack;
                break;
            default:

                break;
        }
    }

    public void GetMoney(GetPlayerMoneyEvent e)
    {
        e.money = m_money;
    }
}
