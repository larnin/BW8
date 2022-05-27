using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using NLocalization;

public class InteractDialog : Interactable
{
    [SerializeField] DialogObject m_dialogObject;

    public override int GetInteractionTextID()
    {
        return Loc.GetTextID("SPEAK");
    }

    public override void Interact(GameObject caster)
    {
        DialogPopup.StartDialog(m_dialogObject);
    }
}
