using NLocalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class QuestEntity : Interactable
{
    enum QuestInteractionType
    { 
        None,
        Dialog,
    }

    [SerializeField] string m_nameID;
    [SerializeField] float m_textOffset;

    SubscriberList m_subscriberList = new SubscriberList();

    QuestInteractionType m_interactionType = QuestInteractionType.None;

    LocText m_text;
    DialogObject m_dialogObject;

    private void Awake()
    {
        m_subscriberList.Add(new Event<GetQuestEntityIDEvent>.LocalSubscriber(GetID, gameObject));
        m_subscriberList.Add(new Event<DeathEvent>.LocalSubscriber(OnDeath, gameObject));
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

    public string GetNameID()
    {
        return m_nameID;
    }

    public override bool CanInteract()
    {
        if (m_interactionType == QuestInteractionType.Dialog)
            return true;
        return false;
    }

    public override bool IsInstantInteraction()
    {
        return false;
    }

    public override int GetInteractionTextID()
    {
        if (m_interactionType == QuestInteractionType.Dialog)
            return m_text.GetTextID();
        return LocTable.invalidID;
    }

    public override Vector2 GetOffset()
    {
        return new Vector2(0, m_textOffset);
    }

    public override void Interact(GameObject caster)
    {
        if (m_interactionType == QuestInteractionType.Dialog)
        {
            DialogPopup.StartDialog(m_dialogObject);
            Event<QuestStartTalkEvent>.Broadcast(new QuestStartTalkEvent(this)); 
            return;
        }
        return;
    }

    public void ResetInteraction()
    {
        m_interactionType = QuestInteractionType.None;
    }

    public void SetDialogInteraction(LocText text, DialogObject dialog)
    {
        m_interactionType = QuestInteractionType.Dialog;
        m_text = text;
        m_dialogObject = dialog;
    }

    void OnDeath(DeathEvent e)
    {
        Event<QuestKillEntityEvent>.Broadcast(new QuestKillEntityEvent(this));
    }
}
