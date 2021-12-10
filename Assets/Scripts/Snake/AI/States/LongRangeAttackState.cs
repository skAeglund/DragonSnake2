using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;

public class LongRangeAttackState : IState
{
    AttackingAIController AISnake;
    SnakeAbilities abilities;
    PlayerController player;

    public LongRangeAttackState(AttackingAIController aISnake, SnakeAbilities abilities, PlayerController player)
    {
        this.AISnake = aISnake;
        this.abilities = abilities;
        this.player = player;
    }
    public void Start()
    {
        
    }
    public void Execute(out RotationDirection rotationDirection, out Ability pendingAbility, out bool slowState)
    {
        slowState = AISnake.SlowState;
        pendingAbility = Ability.Null;
        rotationDirection = AISnake.RotationDirection;

        if (abilities.Fireball.IsOnCooldown)
        {
            InternalExit();
            return;
        }
        Vector3 headDirection = (player.transform.position + player.transform.right * (player.Movement.Speed * 0.5f) - AISnake.transform.position).normalized;
        if (Vector3.Angle(AISnake.transform.right, headDirection) > 1)
        {
            AISnake.TurnLeftOrRight(headDirection, out int signedAngle);
            rotationDirection = (RotationDirection)signedAngle;
        }
        else
        {
            pendingAbility = Ability.Fireball;
        }
    }

    public void Exit()
    {
        
    }
    private void InternalExit()
    {
        AISnake.StateMachine.ChangeState(new MoveTowardFoodState(AISnake));
    }

}
