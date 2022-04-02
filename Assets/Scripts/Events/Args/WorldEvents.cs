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