using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Measures how long the player has been looking at another snake
/// and sends this information before colliding with an AI snake
/// </summary>
public class RaycastTimer : MonoBehaviour
{
    private float timeSpentLookingAtTheSameSnake;
    private SnakeBody currentTarget;
    private float distanceToTarget = float.MaxValue;
    private bool messageSent = false;
    private SnakeBody hitBody;
    private float interval = 0.05f;

    private void Start()
    {
        InvokeRepeating("UpdateTime", 1, interval);
    }
    private void UpdateTime()
    {
        if (Physics.SphereCast(transform.position, 5, transform.right, out RaycastHit hit, Mathf.Infinity) ||
            Physics.SphereCast(transform.position, 20, transform.right, out hit, Mathf.Infinity))
        {
            if (hit.collider.TryGetComponent(out BodyPart bodyPart) || hit.collider.TryGetComponent(out hitBody))
            {
                if (bodyPart != null)
                    hitBody = bodyPart.snakeBody;
                if (currentTarget != hitBody)
                {
                    currentTarget = hitBody;
                    timeSpentLookingAtTheSameSnake = 0;
                }
                else if (hitBody != null)
                {
                    timeSpentLookingAtTheSameSnake += interval;
                    distanceToTarget = Vector3.Distance(transform.position + transform.right * 2, hit.transform.position);
                }
                if (distanceToTarget < 5 && !messageSent && hitBody != null)
                {
                    StartCoroutine(SendDefendMessage(3, hitBody));
                }
            }
        }
        else if (timeSpentLookingAtTheSameSnake != 0)
        {
            timeSpentLookingAtTheSameSnake = 0;
            distanceToTarget = float.MaxValue;
        }
    }
    private IEnumerator SendDefendMessage(float delay,SnakeBody snakeBody)
    {
        if (snakeBody == null) yield break;

        messageSent = true;
        if (currentTarget.TryGetComponent(out AttackingAIController aiSnake))
        {
            aiSnake.DefendAgainstWhirlwind(timeSpentLookingAtTheSameSnake);
        }
        yield return new WaitForSeconds(delay);
        messageSent = false;
    }
}
