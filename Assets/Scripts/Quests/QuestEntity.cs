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
}
