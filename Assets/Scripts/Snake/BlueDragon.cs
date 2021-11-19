using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueDragon : Snake
{
    [SerializeField] private GameObject targetFood = null;
    [SerializeField] private Vector3 directionToFood;
    [SerializeField] private float distanceToFood;
    [SerializeField] private bool isMovingTowardBomb = false;
    [SerializeField] private bool isChangingPath = false;
    [SerializeField] private bool isRotatingTowardsFood = false;

    private PlayerSnake player;
    private int startingLength = 5;
    private int eatCount = 0;
    private int minEatBeforeAttack;
    

    public MyLinkedList<GameObject> SnakeList { get => snakeList; set => snakeList = value; }
    public int StartingLength { get => startingLength; set => startingLength = value; }

    public override void Awake()
    {
        base.Awake();
        player = GameObject.Find("PlayerSnake").GetComponentInChildren<PlayerSnake>();
        UpdateSpeed();
        GameManager.ActiveSnakes.Add(SnakeList);
        snakeList.AddFirst(gameObject);
        cameraScript = Camera.main.GetComponent<SmoothCamera>();
        minEatBeforeAttack = Random.Range(1, 5);
    }
    private void Start()
    {
        for (int i = 0; i < startingLength; i++)
        {
            SpawnBodyPart(true);
        }
    }

    void Update()
    {
        if (StoneState.IsActive || StunnedState)
            return;
        MoveTowardTarget();
        MoveBodyParts();
        DrawSnakeBody();
        if (distanceToFood <10 && player.SnakeList.Contains(targetFood) && !Whirlwind.IsActive)
        {
            whirlWindObject.SetActive(true);
            Whirlwind.IsActive = true;
            capsuleCollider.enabled = true;
            boxCollider.enabled = false;
        }
        else if (distanceToFood >= 15 && distanceToFood < 30 &&player.SnakeList.Contains(targetFood) && !Fireball.IsOnCooldown)
        {
            if (GetComponent<Renderer>().isVisible)
            {
                Vector3 headDirection = ((player.transform.position + player.transform.right * (player.Speed * 0.5f)) - transform.position).normalized;
                StartCoroutine(FireballRoutine(headDirection));
            }
        }
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

    private void MoveTowardTarget()
    {
        if (!isChangingPath)
        {
            if (player.SnakeList.Count > 4 && !Whirlwind.IsOnCooldown && eatCount >= minEatBeforeAttack)
            {
                FindMiddleOfSnakeTarget();
            }
            else if (SpawnManager.Foods.Count > 0)
                FindClosestFood();

            if (targetFood != null) 
            {
                if (!CheckForBombCollision(directionToFood) && !isRotatingTowardsFood)
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
    private void FindMiddleOfSnakeTarget()
    {
        int middleIndex = (player.SnakeList.Count - 1) / 2;

        ListNode<GameObject> current = player.SnakeList.First;
        for (int i = 0; i <= middleIndex; i++, current = current.Next)
        {
            if (i == middleIndex)
            {
                targetFood = current.Value;
                distanceToFood = Vector3.Distance(transform.position, targetFood.transform.position);
                directionToFood = (targetFood.transform.position - transform.position).normalized;
                return;
            }
        }
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
            float rSpeed = RotationSpeed + (Mathf.Abs(rotatedAmount) * 0.5f);
            transform.Rotate(0, 0, Time.deltaTime * rSpeed * sign);
            rotatedAmount += Time.deltaTime * rSpeed * sign;
            yield return null;
        }
        transform.right = directionToFood;
        isRotatingTowardsFood = false;
    }
    private float TurnLeftOrRight(out int sign)
    {
        float angle = Vector3.SignedAngle(directionToFood, transform.right, transform.forward);
        if (angle <0)
        {
            sign = 1; //left
        }
        else 
        {
            sign = -1; //right
        }
        return angle;
    }


    private IEnumerator ChangePath()
    {
        isChangingPath = true;

        // Changes path to avoid bomb - this could be improved
        while(CheckForBombCollision(directionToFood))
        {                                                   
            transform.Rotate(0, 0, Time.deltaTime * RotationSpeed);
            yield return new WaitForEndOfFrame();
        }
        isChangingPath = false;
    }
    private void FindClosestFood()
    {
        if (SpawnManager.Foods.Count == 0)
            return;

        if (targetFood == null || targetFood.CompareTag("BodyPart"))
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
    protected override void DrawSnakeBody()
    {
        Vector3[] bodyPartPositions = new Vector3[snakeList.Count];
        ListNode<GameObject> current = snakeList.First;
        bodyPartPositions[0] = targetPos.position;
        current = current.Next;
        for (int i = 1; i < snakeList.Count; i++, current = current.Next)
        {
            bodyPartPositions[i] = current.Value.transform.position;
            float f1 = (float)i / (float)snakeList.Count;
            float ballSize = Mathf.Lerp(0.33f, 0.01f, AnimationCurves.SnakeSize.Evaluate(f1));
            if (i > 0)
                current.Value.transform.localScale = new Vector3(ballSize, ballSize, 1);
        }
        snakeList.Last.Value.transform.localScale = Vector3.zero;
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = snakeList.Count;
        lineRenderer.SetPositions(bodyPartPositions);
    }
    protected override void SpawnBodyPart(bool ofColorBlue = true)
    {
        Transform tailTransform = snakeList.Last.Value.transform;
        GameObject newBody = Instantiate(Prefabs.BlueBodyPart, tailTransform.position /*+ (tailTransform.right *-0.8f)*/, tailTransform.rotation, bodyParent.transform);
        snakeList.AddLast(newBody);
        newBody.transform.localScale = Vector3.zero;
        newBody.GetComponent<BodyPart>().snakeHead = gameObject;

        cameraScript.UpdateDistance();
        UpdateSpeed();
        
    }
    public void DisableLevelSystem()
    {
        Fireball.RequiredLevel = 0;
        Whirlwind.RequiredLevel = 0;
        StoneState.RequiredLevel = 0;
        Dash.RequiredLevel = 0;
        Fireball.Learn();
        Whirlwind.Learn();
        StoneState.Learn();
        Dash.Learn();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Food"))
        {
            SpawnBodyPart(true);
            SpawnManager.Foods.Remove(other.gameObject);
            Destroy(other.gameObject);
            eatCount++;
        }
        else if (other.CompareTag("BodyPart") || other.CompareTag("Player"))
        {
            if (player.StoneState.IsActive && !isDying && Whirlwind.IsActive)
            {
                player.StoneState.EndCooldown();
                StartCoroutine(StunRoutine(2));
            }
            else if (!player.StoneState.IsActive && Whirlwind.IsActive)
            {
                player.ConvertIntoFood(other.gameObject);
                whirlWindObject.SetActive(false);
                Whirlwind.IsActive = false;
                capsuleCollider.enabled = false;
                boxCollider.enabled = true;
                StartCoroutine(WhirlwindCooldown(20));
            }
        }
    }
    private void OnDestroy()
    {
        SpawnManager.SpawnBlueDragon(10);
    }
    private IEnumerator WhirlwindCooldown(float time)
    {
        Whirlwind.IsOnCooldown = true;
        yield return new WaitForSeconds(time);
        Whirlwind.IsOnCooldown = false;
    }
}
