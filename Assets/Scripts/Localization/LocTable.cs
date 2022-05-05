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
            public string remark;

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

        public string Add(int id)
        {
            var element = GetInternal(id);
            if (element != null)
                return element.textID;

            if (m_nextID <= id)
                m_nextID = id + 1;

            string label = "ID_";
            string text = label + id;

            for(int i = 0; i <= m_nextID; i++)
            {
                element = GetInternal(text);
                if (element == null)
                {
                    m_locs.Add(new LocElement(id, text));
                    return text;
                }
                text = label + id;
            }

            return null;
        }

        public bool ForceAdd(int id, string textID)
        {
            var element = GetInternal(id);
            var textElement = GetInternal(textID);

            if(element != null && textElement != null)
                return element == textElement;

            if(element != null)
            {
                element.textID = textID;
                return true;
            }

            if(textElement != null)
            {
                textElement.id = id;
                return true;
            }

            if (m_nextID <= id)
                m_nextID = id + 1;

            m_locs.Add(new LocElement(id, textID));

            return true;
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

        public bool Set(int id, string textID)
        {
            if (Contains(textID))
                return false;

            var l = GetInternal(id);
            if (l != null)
            {
                l.textID = textID;
                return true;
            }
            return false;
        }

        void SetRemark(int id, string remark)
        {
            var l = GetInternal(id);

            if (l != null)
                l.remark = remark;
        }

        public string GetRemark(int id)
        {
            var l = GetInternal(id);
            if (l != null)
                return l.remark;
            return null;
        }

        public int Count()
        {
            return m_locs.Count();
        }

        public int GetIdAt(int index)
        {
            if (index < 0 || index >= m_locs.Count)
                return invalidID;
            return m_locs[index].id;
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