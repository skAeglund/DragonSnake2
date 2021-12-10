using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;

public class NonAttackingAISnake : AISnakeManager
{
    public MyLinkedList<GameObject> SnakeList { get => snakeBody.SnakeList; set => snakeBody.SnakeList = value; }
    StateMachine stateMachine = new StateMachine();

    private void Awake()
    {
        movement = GetComponent<Movement>();
        snakeBody = GetComponent<SnakeBody>();
        snakeList = snakeBody.SnakeList;
        abilities = GetComponent<SnakeAbilities>();
        stateMachine.ChangeState(new MoveTowardFoodState(this));
        snakeBody.resizeSnakeEvent += movement.UpdateSpeed;

        StartCoroutine(abilities.ImmuneRoutine(1));
    }

    private void Update()
    {
        if (!movement.StunnedState)
            stateMachine.Update(out rotationDirection, out pendingAbility, out slowState);

        movement.Rotate(rotationDirection, slowState);
        movement.Move(slowState);

        if (!movement.StunnedState)
            snakeBody.MoveBodyParts(movement.Speed);
        snakeBody.DrawSnakeBody();
        //if (!movement.StunnedState)
            

        if (pendingAbility != Ability.Null && !movement.StunnedState)
            abilities.UseAbility(pendingAbility);
    }
        

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Food"))
        {
            snakeBody.SpawnBodyPart();
            SpawnManager.Instance.ReturnFoodToPool(other.gameObject);
        }
        else if (other.CompareTag("BodyPart"))
        {
            if (!movement.StunnedState && !abilities.StunImmuneState && other.transform.root != transform.root)
            {
                StartCoroutine(abilities.StunRoutine(1));
            }
        }
    }
}
