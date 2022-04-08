using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SetWorldEvent
{
    public WorldObject world;

    public SetWorldEvent(WorldObject _world)
    {
        world = _world;
    }
}

public class StartChangeWorldEvent
{
    public WorldObject world;
    public string spawnName;

    public StartChangeWorldEvent(WorldObject _world, string _spawnName)
    {
        world = _world;
        spawnName = _spawnName;
    }
}
public class ShowLoadingScreenEvent
{
    public bool start;

    public ShowLoadingScreenEvent(bool _start)
    {
        start = _start;
    }
}

public class GetLoadedChunksEvent
{
    public List<Rect> chunks = new List<Rect>();
}

public class AddEntityEvent
{
    public GameObject entity;
    
    public AddEntityEvent(GameObject _entity)
    {
        entity = _entity;
    }
}

public class RemoveEntityEvent
{
    public GameObject entity;

    public RemoveEntityEvent(GameObject _entity)
    {
        entity = _entity;
    }
}

public class RemoveAllEntityEvent { }