using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class BSMConditionAfterTimer : BSMConditionBase
{
    [SerializeField] float m_fTimer;
    public float timer { get { return m_fTimer; } set { m_fTimer = value; } }
}