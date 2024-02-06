using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

public class BSMAttributeObjectView : BSMDropdownCallback
{
    BSMAttributeObject m_object;

    public BSMAttributeObjectView(BSMAttributeObject obj)
    {
        m_object = obj;
    }

    public BSMAttributeObject GetObject()
    {
        return m_object;
    }

    public VisualElement GetElement()
    {
        return null;
    }

    public void SendResult(int result)
    {
        
    }
}
