using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class IsDebugEnabledEvent
{
    public bool enabled = false;
}

public class DrawDebugWindowEvent
{
    public DebugWindowType type;
    public int ID;
    public Rect rect;

    public DrawDebugWindowEvent(DebugWindowType _type, int _id, Rect _rect)
    {
        type = _type;
        ID = _id;
        rect = _rect;
    }
}

public class DebugDrawEvent
{
    public DebugDrawType type;

    public DebugDrawEvent(DebugDrawType _type)
    {
        type = _type;
    }
}