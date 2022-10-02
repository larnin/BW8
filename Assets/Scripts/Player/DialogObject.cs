using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using NLocalization;

[CreateAssetMenu(fileName = "Dialog", menuName = "Game/Dialog", order = 1)]
public class DialogObject : ScriptableObject
{
    [SerializeField] List<LocText> m_texts = new List<LocText>();

    public int GetNbTexts()
    {
        return m_texts.Count;
    }

    public LocText GetText(int index)
    {
        if (index < 0 || index >= m_texts.Count)
            return null;
        return m_texts[index];
    }
}

