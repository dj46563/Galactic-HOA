﻿
using System.Collections;

public class Playing : State
{
    public Playing(StateMachine stateMachine) : base(stateMachine)
    {
    }
    
    public override IEnumerator Start()
    {
        // set status text
        yield break;
    }

    public override void Update()
    {
        // if players in circle >= 2, start timer to end
        return;
    }

    public override IEnumerator End()
    {
        // transition to 
        yield break;
    }
}