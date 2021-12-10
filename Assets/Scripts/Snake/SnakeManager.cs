using UnityEngine;
using Enums;

public abstract class SnakeManager : MonoBehaviour
{
    protected MyLinkedList<GameObject> snakeList = new MyLinkedList<GameObject>();
    protected SnakeBody snakeBody;
    protected Movement movement;
    protected SnakeAbilities abilities;
    protected RotationDirection rotationDirection = RotationDirection.None;
    protected Ability pendingAbility = Ability.Null;
    protected bool slowState;

    public SnakeBody SnakeBody { get => snakeBody; }
    public bool SlowState { get => slowState; }
    public RotationDirection RotationDirection { get => rotationDirection; }
    public MyLinkedList<GameObject> SnakeList { get => snakeList; }
}




public abstract class AISnakeManager : SnakeManager 
{
    public bool CheckForBombCollision(Vector3 direction, out Vector3 directionToBomb, out float distanceToBomb)
    {
        directionToBomb = Vector3.zero;
        distanceToBomb = float.MaxValue;
        float radius = 3;
        if (Physics.SphereCast(transform.position, radius, direction, out RaycastHit hit, Mathf.Infinity))
        {
            if (hit.collider.CompareTag("Bomb"))
            {
                directionToBomb = (hit.transform.position - transform.position).normalized;
                distanceToBomb = Vector3.Distance(hit.transform.position, transform.position);
                return true;
            }
        }
        return false;
    }
    public bool CheckForPlayerCollision(Vector3 direction, out float distanceToPlayer, float length = Mathf.Infinity)
    {
        distanceToPlayer = float.MaxValue;
        float radius = 5;
        if (Physics.SphereCast(transform.position, radius, direction, out RaycastHit hit, length))
        {
            if (hit.collider.CompareTag("BodyPart"))
            {
                if (hit.collider.GetComponent<BodyPart>().snakeHead != gameObject)
                {
                    distanceToPlayer = Vector3.Distance(hit.transform.position, transform.position);
                    return true;
                }
            }
            else if (hit.collider.CompareTag("Player"))
            {
                distanceToPlayer = Vector3.Distance(hit.transform.position, transform.position);
                return true;
            }
        }
        return false;
    }
    public float TurnLeftOrRight(Vector3 direction, out int sign)
    {
        float angle = Vector3.SignedAngle(direction, transform.right, transform.forward);
        if (angle < 0)
        {
            sign = 1; //left
        }
        else
        {
            sign = -1; //right
        }
        return angle;
    }

    public RotationDirection FindBestDirectionToAvoidPlayer()
    {
        float radius = 5;
        for (int i = 0; i < 360; i++)
        {
            Vector3 newRightDirection = new Vector3(transform.right.x, transform.right.y, transform.right.z + i);
            Vector3 newLeftDirection = new Vector3(transform.right.x, transform.right.y, transform.right.z - i);
            if (Physics.SphereCast(transform.position, radius, newRightDirection, out RaycastHit hitRight, Mathf.Infinity))
            {
                if (hitRight.collider.CompareTag("BodyPart"))
                {
                    return RotationDirection.Right;
                }
            }
            if (Physics.SphereCast(transform.position, radius, newLeftDirection, out RaycastHit hitLeft, Mathf.Infinity))
            {
                if (hitLeft.collider.CompareTag("BodyPart"))
                {
                    return RotationDirection.Left;
                }
            }
        }

        return RotationDirection.None;
    }
}
