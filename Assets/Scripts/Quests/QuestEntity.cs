using NLocalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class QuestEntity : Interactable
{
    [SerializeField] string m_nameID;

    SubscriberList m_subscriberList = new SubscriberList();

    private void Awake()
    {
        m_subscriberList.Add(new Event<GetQuestEntityIDEvent>.LocalSubscriber(GetID, gameObject));
        m_subscriberList.Subscribe();
    }

    private void Start()
    {
        Event<RegisterQuestEntityEvent>.Broadcast(new RegisterQuestEntityEvent(this, m_nameID));
    }

    private void OnDestroy()
    {
        Event<UnregisterQuestEntityEvent>.Broadcast(new UnregisterQuestEntityEvent(this, m_nameID));
        m_subscriberList.Unsubscribe();
    }

    void GetID(GetQuestEntityIDEvent e)
    {
        e.id = m_nameID;
    }

    public override bool CanInteract()
    {
        return false;
    }

    public override bool IsInstantInteraction()
    {
        return false;
    }

    public override int GetInteractionTextID()
    {
        return LocTable.invalidID;
    }

    public override Vector2 GetOffset()
    {
        return Vector2.zero;
    }

    public override void Interact(GameObject caster)
    {
        return;
    }
}
