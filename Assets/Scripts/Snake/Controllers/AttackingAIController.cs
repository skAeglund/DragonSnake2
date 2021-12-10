using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;

public class AttackingAIController : AISnakeManager
{
    private PlayerController player;

    private bool CouldBeStunned => !movement.StunnedState && !abilities.StunImmuneState && !abilities.Whirlwind.IsActive;
    private bool FailedWhirlwind(SnakeAbilities collider) => collider.StoneState.IsActive && abilities.Whirlwind.IsActive;
    private bool SuccessfulWhirlwind(SnakeAbilities collider) => !collider.StoneState.IsActive && abilities.Whirlwind.IsActive;
    public StateMachine StateMachine { get; set; }

    #region Unity Messages / Events
    private void Awake()
    {
        StateMachine = new StateMachine();
        movement = GetComponent<Movement>();
        snakeBody = GetComponent<SnakeBody>();
        snakeBody.IsBlue = true;
        abilities = GetComponent<SnakeAbilities>();
        player = GameObject.Find("PlayerSnake").GetComponentInChildren<PlayerController>();
        snakeBody.resizeSnakeEvent += movement.UpdateSpeed;
    }
    private void Start()
    {
        StateMachine.ChangeState(new MoveTowardFoodState(this));
    }
    private void Update()
    {
        if (abilities.StoneState.IsActive || movement.StunnedState) return;

        StateMachine.Update(out rotationDirection, out pendingAbility, out slowState);
        
        movement.Rotate(rotationDirection, slowState);
        movement.Move(slowState);
        snakeBody.MoveBodyParts(movement.Speed);
        snakeBody.DrawSnakeBody();

        if (pendingAbility != Ability.Null)
            abilities.UseAbility(pendingAbility);
    }
    private void OnDestroy()
    {
        SpawnManager.Instance.SpawnBlueDragon(10);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Food"))
        {
            snakeBody.SpawnBodyPart(true);
            SpawnManager.Instance.ReturnFoodToPool(other.gameObject);
            SpawnManager.Instance.GetClosestFood(transform.position, out float distanceToFood);

            bool longRangeAttack = distanceToFood > 30 && !abilities.Fireball.IsOnCooldown && GetComponent<Renderer>().isVisible;

            IState newState = longRangeAttack ? (IState)new LongRangeAttackState(this, abilities, player)
                                              : (IState)new MoveTowardFoodState(this);

            StateMachine.ChangeState(newState);

        }
        else if (other.CompareTag("BodyPart"))
        {
            // check if the bodypart is part of this snake body
            if (other.CompareTag("BodyPart"))
                if (other.GetComponent<BodyPart>().snakeHead == gameObject) return;

            SnakeAbilities otherAbilities = other.transform.root.GetComponentInChildren<SnakeAbilities>();
            if (otherAbilities == null) return;

            if (FailedWhirlwind(otherAbilities))
            {
                StartCoroutine(abilities.StunRoutine(2));
            }
            else if (SuccessfulWhirlwind(otherAbilities))
            {
                StateMachine.ChangeState(new MoveTowardFoodState(this));
                other.GetComponent<BodyPart>().ConvertIntoFood();
                abilities.DeActivateWhirlWind();
                StartCoroutine(abilities.ImmuneRoutine(1));
            }
            else if (CouldBeStunned)
            {
                // colliding with stunned snake doesn't stun
                if (otherAbilities.StunnedState || otherAbilities.Whirlwind.IsActive)
                    return;
                StartCoroutine(abilities.StunRoutine(1));
            }
        }
    }
    #endregion

    public void OnEnterAttackRange()
    {
        if (StateMachine.CurrentState.GetType() != typeof(ShortRangeAttackState))
        {
            StateMachine.ChangeState(new ShortRangeAttackState(this, player, abilities));
        }
    }
    public void OnExitAttackRange()
    {

        if (StateMachine.CurrentState.GetType() != typeof(MoveTowardFoodState) && !abilities.Whirlwind.IsActive)
        {
            StateMachine.ChangeState(new MoveTowardFoodState(this));
        }
    }
    public void DefendAgainstWhirlwind(float timeSpentLooking)
    {
        if (abilities.Whirlwind.IsActive || movement.StunnedState) return;

        Debug.Log("time spent looking : " + timeSpentLooking);
        if (timeSpentLooking < 0.4f) return;
        int probability = timeSpentLooking < 0.6f ? 25 :
                          timeSpentLooking < 1 ? 50 :
                          timeSpentLooking < 1.5f ? 75 : 100;

        ProbabilityBasedStoneState(probability);
    }
    private bool ProbabilityBasedStoneState(int probability)
    {
        int randomNr = Random.Range(1, 101);
        Debug.Log("Random nr: " + randomNr + "Probability nr: " + probability);
        if (randomNr <= probability)
        {
            abilities.UseAbility(Ability.StoneState);
            return true;
        }
        return false;
    }

    
}
