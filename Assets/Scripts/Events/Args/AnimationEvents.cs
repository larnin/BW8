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

    public static Vector2 GetDirection(AnimationDirection dir)
    {
        switch(dir)
        {
            case AnimationDirection.none:
                return Vector2.zero;
            case AnimationDirection.Up:
                return new Vector2(0, 1);
            case AnimationDirection.Down:
                return new Vector2(0, -1);
            case AnimationDirection.Left:
                return new Vector2(-1, 0);
            case AnimationDirection.Right:
                return new Vector2(1, 0);
            default:
                break;
        }

        return Vector2.zero;
    }
}

public class PlayAnimationEvent
{
    public AnimationDirection direction;
    public string name;
    public bool loop;
    public bool after;
    public int layer;
    public float startNormTime;

    public PlayAnimationEvent(string _name, bool _loop = false, bool _after = false, float _startNormTime = -1)
    : this(_name, AnimationDirection.none, 0, _loop, _after, _startNormTime) { }

    public PlayAnimationEvent(string _name, int _layer, bool _loop = false, bool _after = false, float _startNormTime = -1)
    : this(_name, AnimationDirection.none, _layer, _loop, _after, _startNormTime) { }

    public PlayAnimationEvent(string _name, AnimationDirection _direction, bool _loop = false, bool _after = false, float _startNormTime = -1)
        : this(_name, _direction, 0, _loop, _after, _startNormTime) { }

    public PlayAnimationEvent(string _name, AnimationDirection _direction, int _layer, bool _loop = false, bool _after = false, float _startNormTime = -1)
    {
        direction = _direction;
        layer = _layer;
        loop = _loop;
        after = _after;
        name = _name;
        startNormTime = _startNormTime;
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
    public float normTime;

    public GetPlayingAnimationEvent()
    {

    }
}

public class GetAnimationDurationEvent
{
    public string name;
    public AnimationDirection direction;

    public float duration;

    public GetAnimationDurationEvent(string _name, AnimationDirection _direction)
    {
        name = _name;
        direction = _direction;

        duration = 0;
    }
}