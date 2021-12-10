using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;

public class StateMachine
{
    IState currentState;
    public IState CurrentState { get => currentState; }

    public void ChangeState(IState newState)
    {
        if (currentState != null)
        {
            currentState.Exit();
        }
        currentState = newState;
        currentState.Start();
    }

    public void Update(out RotationDirection rotationDirection, out Ability pendingAbility, out bool slowState)
    {
        currentState.Execute(out rotationDirection, out pendingAbility, out slowState);
    }
}
