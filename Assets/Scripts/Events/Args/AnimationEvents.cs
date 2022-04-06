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

static class AnimationDirectionEx
{
    public static AnimationDirection GetDirection(Vector2 dir)
    {
        if (dir.x == 0 && dir.y == 0)
            return AnimationDirection.none;

        if(Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            if (dir.x < 0)
                return AnimationDirection.Left;
            return AnimationDirection.Right;
        }

        if (dir.y > 0)
            return AnimationDirection.Up;
        return AnimationDirection.Down;
    }
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

public class GetPlayingAnimationEvent
{
    public string name;
    public AnimationDirection direction;
    public bool loop;
    public int layer;

    public GetPlayingAnimationEvent()
    {

    }
}