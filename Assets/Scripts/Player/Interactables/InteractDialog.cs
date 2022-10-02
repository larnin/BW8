using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using NLocalization;

public class InteractDialog : Interactable
{
    [SerializeField] LocText m_text;
    [SerializeField] float m_textOffset;

    [SerializeField] DialogObject m_dialogObject;

    public override bool CanInteract()
    {
        return true;
    }

    public override int GetInteractionTextID()
    {
        return m_text.GetTextID();
    }

    public override Vector2 GetOffset()
    {
        return new Vector2(0, m_textOffset);
    }

    public override void Interact(GameObject caster)
    {
        DialogPopup.StartDialog(m_dialogObject);
    }

    public override bool IsInstantInteraction()
    {
        return false;
    }
}
