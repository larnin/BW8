using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class PlayerHandActionBase
{
    protected PlayerHandController m_player;

    public PlayerHandActionBase(PlayerHandController player)
    {
        m_player = player;
    }

    public virtual void OnInit() { }
    public virtual void OnDestroy() { }
    public virtual void BeginProcess() { }
    public virtual void Process(bool inputPressed) { }
    public virtual void EndProcess() { }
    public virtual void AlwaysProcess() { }
    public virtual void OnPress() { }
    public virtual void OnPressEnd() { }
    public virtual void GetVelocity(out Vector2 offsetVelocity, out float velocityMultiplier) 
    { 
        offsetVelocity = Vector2.zero;
        velocityMultiplier = 1;
    }
    public virtual bool AreActionsLocked()
    {
        return false;
    }
}
