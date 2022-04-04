using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum AnimationDirection
{
    none,
    Up,
    Down,
    Left,
    Right
}

public class PlayAnimationEvent
{
    public AnimationDirection direction;
    public string name;
    public bool loop;
    public bool after;
    public int layer;

    public PlayAnimationEvent(string _name, AnimationDirection _direction, bool _loop = false, bool _after = false)
        : this(_name, _direction, 0, _loop, _after) { }

    public PlayAnimationEvent(string _name, AnimationDirection _direction, int _layer, bool _loop = false, bool _after = false)
    {
        direction = _direction;
        layer = _layer;
        loop = _loop;
        after = _after;
        name = _name;
    }
}

public class StopAnimationEvent
{
    public int layer;
    public int index;

    public StopAnimationEvent(int _index) : this(0, _index) { }

    public StopAnimationEvent(int _layer, int _index)
    {
        layer = _layer;
        index = _index;
    }
}

public class GetAnimationCountEvent
{
    public int layer;
    public int animationNb;

    public GetAnimationCountEvent(int _layer = 0)
    {
        layer = _layer;
    }
}

public class GetAnimationEvent
{
    public int layer;
    public int index;

    public string name;
    public AnimationDirection direction;
    public bool loop;

    public GetAnimationEvent(int _index) : this(0, _index) { }

    public GetAnimationEvent(int _layer, int _index)
    {
        layer = _layer;
        index = _index;
    }
}