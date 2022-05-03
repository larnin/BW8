using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NLocalization
{
    public class LocTable : SerializedScriptableObject
    {
        class LocElement
        {
            public int id;
            public string textID;

            public LocElement(int _id, string _textID)
            {
                id = _id;
                textID = _textID;
            }
        }

        public const int invalidID = -1;

        [HideInInspector]
        [SerializeField] int m_nextID = 0;

        [HideInInspector]
        [SerializeField]
        List<LocElement> m_locs = new List<LocElement>();

        [HideInInspector]
        [SerializeField] string m_defaultLanguageID;
        public string defaultLanguageID { get { return m_defaultLanguageID; } set { m_defaultLanguageID = value; } }

        public int Add(string textID)
        {
            if (Contains(textID))
                return invalidID;

            ValidateNextID();

            var element = new LocElement(m_nextID, textID);

            m_locs.Add(element);
            m_nextID++;

            return element.id;
        }

        public void Remove(int id)
        {
            for (int i = 0; i < m_locs.Count; i++)
            {
                if (m_locs[i].id == id)
                {
                    m_locs.RemoveAt(i);
                    return;
                }
            }
        }

        public void Remove(string textID)
        {
            for (int i = 0; i < m_locs.Count; i++)
            {
                if (m_locs[i].textID == textID)
                {
                    m_locs.RemoveAt(i);
                    return;
                }
            }
        }

        public bool Contains(int id)
        {
            return GetInternal(id) != null;
        }

        public bool Contains(string textID)
        {
            return GetInternal(textID) != null;
        }

        public string Get(int id)
        {
            var l = GetInternal(id);
            if (l != null)
                return l.textID;
            return null;
        }

        public int Get(string textID)
        {
            var l = GetInternal(textID);
            if (l != null)
                return l.id;
            return invalidID;
        }

        LocElement GetInternal(int id)
        {
            foreach (var l in m_locs)
                if (l.id == id)
                    return l;
            return null;
        }

        LocElement GetInternal(string textID)
        {
            foreach (var l in m_locs)
                if (l.textID == textID)
                    return l;
            return null;
        }

        void ValidateNextID()
        {
            foreach (var l in m_locs)
            {
                if (m_nextID <= l.id)
                    m_nextID = l.id + 1;
            }
        }
    }
}