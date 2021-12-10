using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;

public class ShortRangeAttackState : IState
{
    AttackingAIController AISnake;
    PlayerController player;
    GameObject targetBodyPart;
    SnakeAbilities abilities;
    
    float attackRange = 5;

    public ShortRangeAttackState(AttackingAIController aISnake, PlayerController player, SnakeAbilities abilities)
    {
        this.AISnake = aISnake;
        this.player = player;
        this.abilities = abilities;
    }

    public void Start()
    {
        targetBodyPart = FindTargetBodyPart();
    }
    
    public void Execute(out RotationDirection rotationDirection, out Ability pendingAbility, out bool slowState)
    {
        slowState = AISnake.SlowState;
        pendingAbility = Ability.Null;
        rotationDirection = AISnake.RotationDirection;
        if (targetBodyPart == null)
        {
            Start();
            return;
        }
        if (abilities.Whirlwind.IsOnCooldown && !abilities.Whirlwind.IsActive)
        {
            InternalExit();
            return;
        }
        float distanceToTarget = Vector3.Distance(AISnake.transform.position, targetBodyPart.transform.position);
        Vector3 directionToBodyPart = (AISnake.transform.position - targetBodyPart.transform.position).normalized;
        AISnake.TurnLeftOrRight(directionToBodyPart, out int direction);
        rotationDirection = (RotationDirection)(direction*-1);

        // each frame has a (low) chance to trigger dash skill, which scales with the current FPS
        int fps = (int)(1 / Time.smoothDeltaTime);
        pendingAbility = Random.Range(0, fps * 2) == fps ? Ability.Dash : Ability.Null;

        if (AISnake.CheckForPlayerCollision(AISnake.transform.right, out _, 5))
        {
            pendingAbility = Ability.Whirlwind;
        }

        if (distanceToTarget> attackRange)
        {
            return;
        }
        if (!abilities.Whirlwind.IsOnCooldown)
        {
            pendingAbility = Ability.Whirlwind;
        }
    }
    public void Exit()
    {
        
    }
    private void InternalExit()
    {
        AISnake.StateMachine.ChangeState(new MoveTowardFoodState(AISnake));
    }
    private GameObject FindTargetBodyPart()
    {
        if (player == null)
        {
            return null;
        }
        int mid = player.SnakeList.Count / 2;
        GameObject[] possibleTargets = new GameObject[mid]; // the first half of the player snake
        player.SnakeList.CopyTo(possibleTargets, 1, mid);
        float closestDistance = float.MaxValue;
        int targetIndex = 0;
        for (int i = 0; i < possibleTargets.Length; i++)
        {
            float distance = Vector3.Distance(AISnake.transform.position, possibleTargets[i].transform.position);
            if (distance < closestDistance)
            {
                targetIndex = i;
                closestDistance = distance;
            }
        }
        return possibleTargets[targetIndex];
    }
}
