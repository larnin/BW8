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
    static string timerName = "Timer";

    float m_time = 0;

    public BSMConditionAfterTimer()
    {
        AddAttribute(timerName, new BSMAttributeObject(0.0f));
    }

    public override bool IsValid()
    {
        float timer = GetFloatAttribute(timerName);

        return m_time >= timer;
    }

    public override void BeginUpdate()
    {
        m_time = 0;
    }

    public override void Update() 
    {
        if (!Gamestate.instance.paused)
            m_time += Time.deltaTime;
    }

    public override void Load(JsonObject obj)  { }

    public override void Save(JsonObject obj) { }
}