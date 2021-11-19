using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitSnake : Snake
{
    [SerializeField] private GameObject targetFood = null;
    [SerializeField] private Vector3 directionToFood;
    [SerializeField] private float distanceToFood;
    [SerializeField] private bool isMovingTowardBomb = false;
    [SerializeField] private bool isChangingPath = false;
    [SerializeField] private bool isRotatingTowardsFood = false;

    public MyLinkedList<GameObject> SnakeList { get => snakeList; set => snakeList = value; }

    public override void Awake()
    {
        base.Awake();
        //bodyParent = GameObject.Find("Body");
        targetPos = transform.GetChild(0);
        UpdateSpeed();
        GameManager.ActiveSnakes.Add(SnakeList);
    }

    void Update()
    {
        MoveTowardFood();
        MoveBodyParts();
        DrawSnakeBody();
    }
    private void FixedUpdate()
    {
        float radius = 1;
        if (Physics.SphereCast(transform.position, radius, transform.right, out RaycastHit hit, Mathf.Infinity))
        {
            if (hit.collider.CompareTag("Bomb") && isMovingTowardBomb == false)
            {
                isMovingTowardBomb = true;
                if (!isChangingPath)
                    StartCoroutine(ChangePath());
            }
        }
        else
            isMovingTowardBomb = false;
    }

    private void MoveTowardFood()
    {
        if (!isChangingPath)
        {
            if (SpawnManager.Foods.Count > 0)
                FindClosestFood();

            if (targetFood != null) // 
            {
                if (/*!CheckForBombCollision(directionToFood) && */!isRotatingTowardsFood)
                {
                    //transform.right = directionToFood;
                    StartCoroutine(RotateTowardsFood());
                }
                else if (!isChangingPath && CheckForBombCollision(directionToFood))
                    StartCoroutine(ChangePath());
            }
        }
        transform.position += transform.right * Time.deltaTime * Speed;
    }
    private bool CheckForBombCollision(Vector3 direction)
    {
        float radius = 3;
        if (Physics.SphereCast(transform.position, radius, direction, out RaycastHit hit, Mathf.Infinity))
        {
            if (hit.collider.CompareTag("Bomb"))
            {
                return true;
            }
        }
        return false;
    }
    private IEnumerator RotateTowardsFood()
    {

        isRotatingTowardsFood = true;
        float rotatedAmount = 0;
        while (Mathf.Abs(TurnLeftOrRight(out int sign)) > 1)
        {
            //float speed = Mathf.Abs(rotatedAmount) > 360 ? Mathf.Abs(rotatedAmount) : rotationSpeed;
            float rSpeed = RotationSpeed + (Mathf.Abs(rotatedAmount) * 0.5f);
            transform.Rotate(0, 0, Time.deltaTime * rSpeed * sign);
            rotatedAmount += Time.deltaTime * RotationSpeed * sign;
            //Debug.Log(rotatedAmount);
            yield return null;
        }
        transform.right = directionToFood;
        isRotatingTowardsFood = false;
    }
    private float TurnLeftOrRight(out int sign)
    {
        // current direction = transform.right
        // current position = transform.position
        float angle = Vector3.SignedAngle(directionToFood, transform.right, transform.forward);
        if (angle <0)
        {
            //left
            sign = 1;
        }
        else 
        {
            //right
            sign = -1;
        }
        return angle;
    }


    private IEnumerator ChangePath()
    {
        isChangingPath = true;
        //int randomNr = Random.Range(0, 2);
        // Changes path to avoid bomb
        while(CheckForBombCollision(directionToFood))
        {                                                   //<--------- FIXA TIME-BASED PATH CHANGE
            transform.Rotate(0, 0, Time.deltaTime * RotationSpeed);
            yield return new WaitForEndOfFrame();
        }
        isChangingPath = false;
    }
    private void FindClosestFood()
    {
        //directionToFood = Vector3.zero;
        if (SpawnManager.Foods.Count == 0)
            return;

        if (targetFood == null)
        {
            float shortestDistance = 99999;
            GameObject closestFood = SpawnManager.Foods.First.Value;
            foreach (GameObject food in SpawnManager.Foods)
            {
                if (Vector3.Distance(transform.position, food.transform.position) < shortestDistance)
                {
                    closestFood = food;
                    shortestDistance = Vector3.Distance(transform.position, food.transform.position);
                }
            }

            directionToFood = (closestFood.transform.position - transform.position).normalized;
            distanceToFood = shortestDistance;
            targetFood = closestFood;
        }
        else
        {
            directionToFood = (targetFood.transform.position - transform.position).normalized;
            distanceToFood = Vector3.Distance(transform.position, targetFood.transform.position); 
        }
    }
    //private void DrawLineBetweenParts()
    //{
    //    Vector3[] bodyPartPositions = new Vector3[snakeList.Count];
    //    ListNode<GameObject> current = snakeList.First;
    //    bodyPartPositions[0] = targetPos.position;
    //    current = current.Next;
    //    for (int i = 1; i < snakeList.Count; i++, current = current.Next)
    //    {
    //        bodyPartPositions[i] = current.Value.transform.position;
    //        //LineRenderer lineRenderer = current.Value.GetComponent<LineRenderer>();
    //        float f1 = (float)i / (float)snakeList.Count;
    //        float ballSize = Mathf.Lerp(0.27f, 0.01f, AnimationCurves.SnakeSize.Evaluate(f1));
    //        if (i > 0)
    //            current.Value.transform.localScale = new Vector3(ballSize, ballSize, 1);
    //    }
    //    snakeList.Last.Value.transform.localScale = Vector3.zero;
    //    LineRenderer lineRenderer = GetComponent<LineRenderer>();
    //    lineRenderer.positionCount = snakeList.Count;
    //    lineRenderer.SetPositions(bodyPartPositions);
    //}
    //private void MoveBodyParts()
    //{
    //    if (snakeList.Count <= 1)
    //        return;

    //    float bodyPartSpeed = (1 + Speed * 0.1f) * 5;
    //    ListNode<GameObject> current = snakeList.First;
    //    current.Next.Value.transform.position = Vector3.Lerp(current.Next.Value.transform.position, targetPos.position, (Time.deltaTime * bodyPartSpeed));
    //    current = current.Next;

    //    for (int i = 1; i < snakeList.Count - 1; i++, current = current.Next)
    //    {
    //        Vector3 targetPos = current.Value.transform.position;
    //        current.Next.Value.transform.position = Vector3.Lerp(current.Next.Value.transform.position, targetPos, (Time.deltaTime * bodyPartSpeed));
    //    }
    //}

    //private void UpdateSpeed()
    //{
    //    RotationSpeed = Mathf.Clamp(100 + (snakeList.Count * 5), 0, MaxRotationSpeed);
    //    Speed = Mathf.Clamp(snakeList.Count + 5, 5, MaxSpeed);
    //}
    //private void SpawnBodyPart()
    //{
    //    Transform tailTransform = snakeList.Last.Value.transform;
    //    GameObject newBody = Instantiate(Prefabs.bodyPart, tailTransform.position /*+ (tailTransform.right *-0.8f)*/, tailTransform.rotation, transform.parent);
    //    snakeList.AddLast(newBody);
    //    newBody.transform.localScale = Vector3.zero;

    //    cameraScript.UpdateDistance();
    //    UpdateSpeed();
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Food"))
        {
            SpawnBodyPart();
            SpawnManager.Foods.Remove(other.gameObject);
            Destroy(other.gameObject);
        }
        //else if (other.CompareTag("BodyPart") && Time.time > 2 && !splittingIsActive && snakeList.Count > 3)
        //{
        //    SplitSnake(other.gameObject);
        //}
    }
}
