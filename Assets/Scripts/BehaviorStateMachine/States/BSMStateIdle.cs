using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BSMStateIdle : BSMStateBase
{
    [SerializeField] string m_animation = "Idle";
    [SerializeField] bool m_isOrientable = true;

    public string animation { get { return m_animation; } set { m_animation = value; } }
    public bool isOrientable { get { return m_isOrientable; } set { m_isOrientable = value; } }
}
