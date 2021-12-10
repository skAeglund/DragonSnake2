using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;

public class MoveTowardFoodState : IState
{
    private AISnakeManager AISnake;
    private Vector3 directionToFood;
    private float distanceToFood;
    private GameObject targetFood;
    private float sameDirectionTimer = 0;
    private Ability pendingAbility;
    private int direction = 0;

    public MoveTowardFoodState(AISnakeManager aISnake)
    {
        this.AISnake = aISnake;
        SpawnManager.Instance.onFoodSpawn += CompareDistance;
    }
    public void Start()
    {
        
        if (SpawnManager.Instance.Foods.Count == 0) return;
        // Locate closest food
        FindClosestFood();
    }
    public void Execute(out RotationDirection rotationDirection, out Ability pendingAbility, out bool slowState)
    {
        slowState = AISnake.SlowState;
        pendingAbility = Ability.Null;
        rotationDirection = AISnake.RotationDirection;
        if (targetFood == null)
        {
            Start();
            return;
        }
        if (!targetFood.activeInHierarchy) 
            FindClosestFood();

        directionToFood = (targetFood.transform.position - AISnake.transform.position).normalized;
        distanceToFood = Vector3.Distance(targetFood.transform.position, AISnake.transform.position);
        if (AISnake.CheckForPlayerCollision(directionToFood, out float distanceToPlayer) || AISnake.CheckForPlayerCollision(AISnake.transform.right, out _))
        {
            // the player is either blocking the path to the food or is in front of the snake
            if (distanceToPlayer < 20)
            {
                rotationDirection = AISnake.FindBestDirectionToAvoidPlayer();
                slowState = distanceToPlayer < 10 && AISnake.CheckForPlayerCollision(AISnake.transform.right, out _);
                pendingAbility = Ability.Null;
                return;
            }
        }
        // each frame has a (low) chance to trigger dash skill, which scales with the current FPS
        int fps = (int)(1 / Time.smoothDeltaTime);
        pendingAbility = Random.Range(0, fps *2) == fps ? Ability.Dash : Ability.Null;

        if (AISnake.CheckForBombCollision(directionToFood, out Vector3 directionToBomb, out float distanceToBomb))
        {
            AISnake.TurnLeftOrRight(directionToBomb, out direction); // what direction is closest to the bomb
            direction *= -1; // turn away from bomb
            slowState = distanceToBomb < 5;
            pendingAbility = Ability.Null;
        }
        else if (AISnake.transform.right != directionToFood)
        {
            float angle = AISnake.TurnLeftOrRight(directionToFood, out direction);
            if (Mathf.Abs(angle) > 100)
                slowState = true;
            else if (Mathf.Abs(angle) < 5)
                slowState = false;
        }

        float minimumTurningTime = GetTurningTime();
        // adds the time spent turning in the same direction
        // doesn't allow rapid direction changes (for humanization)
        if (AISnake.RotationDirection == (RotationDirection)direction || sameDirectionTimer <= minimumTurningTime)
        {
            sameDirectionTimer += Time.deltaTime;
        }
        else if (sameDirectionTimer < 3 && sameDirectionTimer > minimumTurningTime)
        {
            rotationDirection = (RotationDirection)direction; // direction change
            sameDirectionTimer = 0;
        }
        else if (sameDirectionTimer >= 3)
        {
            // find new food if you have been turning in the same direction for too long
            FindClosestFood(targetFood);
            sameDirectionTimer = 0;
        }
        if ((RotationDirection)direction == RotationDirection.None)
            slowState = false;
    }
    public void Exit()
    {
        SpawnManager.Instance.onFoodSpawn -= CompareDistance;
        //didExit = true;
    }
   
    private void FindClosestFood(GameObject withExceptionOfThis = null)
    {
        float shortestDistance = float.MaxValue;
        sameDirectionTimer = 0;
        GameObject closestFood = SpawnManager.Instance.Foods.First.Value;
        foreach (GameObject food in SpawnManager.Instance.Foods)
        {
            if (food == withExceptionOfThis) continue;
            if (Vector3.Distance(AISnake.transform.position, food.transform.position) < shortestDistance)
            {
                closestFood = food;
                shortestDistance = Vector3.Distance(AISnake.transform.position, food.transform.position);
            }
        }
        directionToFood = (closestFood.transform.position - AISnake.transform.position).normalized;
        targetFood = closestFood;
    }
    private void CompareDistance(GameObject newFood)
    {
        if (targetFood == null || AISnake == null) return;

        float currentDistance = Vector3.Distance(AISnake.transform.position, targetFood.transform.position);

        float newDistance = Vector3.Distance(AISnake.transform.position, newFood.transform.position);

        if (newDistance < currentDistance)
        {
            targetFood = newFood;
        }

    }
    private float GetTurningTime()
    {
        float maxDistance = 20;
        float f = distanceToFood / maxDistance;
        float maxTurningTime = Mathf.Lerp(0.4f, 0.2f, (AISnake.SnakeBody.SnakeList.Count - 4) / 30);
        return Mathf.Lerp(0.01f, maxTurningTime, f);
    }
}
