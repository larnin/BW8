using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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