﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    protected StateMachine _stateMachine;

    public State(StateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }
    
    public virtual IEnumerator Start()
    {
        yield break;
    }

    public virtual void Update()
    {
        return;
    }
    
    public virtual void End()
    {
        return;
    }
}
