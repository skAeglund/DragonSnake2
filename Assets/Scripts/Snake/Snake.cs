using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public abstract class Snake : SnakeAbilities
{
    [SerializeField] protected GameObject bodyParent;
    [SerializeField] protected Transform targetPos; // target position of the first bodypart
    protected MyLinkedList<GameObject> snakeList = new MyLinkedList<GameObject>();
    protected LineRenderer snakeLine;
    protected SpriteRenderer spriteRenderer;
    protected AnimationCurve widthCurve;
    protected SmoothCamera cameraScript;
    protected bool splittingIsActive = false;
    protected float lineWidth = 2;
    protected bool isDying = false;
    protected readonly float immuneTime = 0.2f;
    public UnityEvent<int> resizeSnakeEvent;

    public float Speed { get; set; } = 7;
    public float MaxSpeed { get; protected set; } = 40;
    public float RotationSpeed { get; set; } = 100;
    public float MaxRotationSpeed { get; protected set; } = 295;
    public float GridMoveTime { get; protected set; } = 0.5f;
    public float MaxBoostSpeed { get; protected set; } = 80;
    public bool StunnedState { get; protected set; } = false;
    
    public virtual void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        snakeLine = GetComponent<LineRenderer>();
        widthCurve = snakeLine.widthCurve;
        cameraScript = Camera.main.GetComponent<SmoothCamera>();
        if (transform.childCount >= 2)
            transform.GetChild(1).TryGetComponent(out spriteRenderer);
    }
    protected void MoveBodyParts()
    {
        if (snakeList.Count <= 1 || StunnedState || StoneState.IsActive)
            return;

        float bodyPartSpeed = (1 + Speed * 0.1f) * 5;
        ListNode<GameObject> current = snakeList.First;
        while (current.Next != null)
        {
            Vector3 targetPosition =  current == snakeList.First ?  targetPos.position : current.Value.transform.position;
            current.Next.Value.transform.position = Vector3.Lerp(current.Next.Value.transform.position, targetPosition, (Time.deltaTime * bodyPartSpeed));
            current.Next.Value.transform.rotation = Quaternion.Lerp(current.Next.Value.transform.rotation, current.Value.transform.rotation, (Time.deltaTime * bodyPartSpeed));
            current = current.Next;
        }
    }
    protected virtual void SpawnBodyPart(bool ofColorBlue = false)
    {
        GameObject bodyPrefab = ofColorBlue ? Prefabs.BlueBodyPart : Prefabs.BodyPart;
        GameObject newBody = Instantiate(bodyPrefab, targetPos.transform.position, targetPos.transform.rotation, bodyParent.transform);

        snakeList.AddAfter(snakeList.First.Value, newBody);
        newBody.GetComponent<BodyPart>().snakeHead = gameObject;
        newBody.transform.localScale = Vector3.zero;

        UpdateSpeed();
        cameraScript.UpdateDistance();
        resizeSnakeEvent.Invoke(snakeList.Count);
    }
    protected virtual void DrawSnakeBody()
    {
        // sets the draw positions of the linerenderer to the positions of all bodyparts
        // and adjusts the scale of each bodypart to fit inside the line
        Vector3[] bodyPartPositions = new Vector3[snakeList.Count];
        bodyPartPositions[0] = targetPos.position;
        ListNode<GameObject> current = snakeList.First.Next;
        for (int i = 1; i < snakeList.Count; i++, current = current.Next)
        {
            bodyPartPositions[i] = current.Value.transform.position;
            float f1 = (float)i / ((float)snakeList.Count - 1);
            float ballSize = Mathf.Lerp(0.001f, 0.3f, widthCurve.Evaluate(f1));
            if (i > 0)
                current.Value.transform.localScale = new Vector3(ballSize, ballSize, 1);
        }
        snakeList.Last.Value.transform.localScale = Vector3.zero;
        snakeLine.widthCurve = widthCurve;
        snakeLine.positionCount = snakeList.Count;
        snakeLine.SetPositions(bodyPartPositions);
    }
    public void ConvertIntoFood(GameObject bodyPart)
    {
        // converts all the body part after this ^ bodypart into food
        if (snakeList.Count < 3 || StoneState.IsActive)
            return;
        ListNode<GameObject> current = snakeList.IndexOf(bodyPart) <= 1 ? snakeList.First.Next.Next : snakeList.Find(bodyPart);
        int counter = 0;
        while (current != null)
        {
            if (counter % 2 == 0)
            {
                SpawnManager.SpawnFoodAtPosition(current.Value.transform.position, transform.parent.name.Substring(0,4) != "Blue");
            }
            snakeList.Remove(current.Value);
            Destroy(current.Value);

            current = current.Next;
            counter++;
        }
        resizeSnakeEvent.Invoke(snakeList.Count);
        UpdateSpeed();
        DrawSnakeBody();
    }
    public void CompleteConversionToFood()
    {
        // converts all bodyparts into food
        if (StoneState.IsActive /*|| ImmuneState*/)
            return;
        ListNode<GameObject> current = snakeList.First;
        int counter = 0;
        while (current != null)
        {
            if (counter % 2 == 0)
            {
                SpawnManager.SpawnFoodAtPosition(current.Value.transform.position, transform.parent.name.Substring(0, 4) != "Blue");
            }
            snakeList.Remove(current.Value);
            Destroy(current.Value);

            current = current.Next;
            counter++;
        }

        Destroy(transform.parent.gameObject);
        if (transform.parent.name.Substring(0, 6) == "Player")
        {
            GameManager.DelayedGameOver(1);
        }
    }
    public virtual void SplitSnake(GameObject bodyPart)
    {
        // splits the snake at the position of this ^ bodypart and creates a new NPC snake
        if (splittingIsActive)
            return;
        splittingIsActive = true;
        GameObject newHead = Instantiate(Prefabs.SplitSnakeHead, bodyPart.transform.position, Quaternion.identity);
        SplitSnake newScript = newHead.GetComponentInChildren<SplitSnake>();
        newScript.SnakeList.AddFirst(newScript.gameObject);
        newHead.transform.localScale = transform.localScale * 0.75f;

        ListNode<GameObject> current = snakeList.Find(bodyPart);
        while (current != null)
        {
            newScript.SnakeList.AddLast(current.Value);
            current.Value.transform.SetParent(newScript.bodyParent.transform);
            current.Value.GetComponent<BodyPart>().snakeHead = newScript.gameObject;
            ListNode<GameObject> nextNode = current.Next;
            snakeList.Remove(current.Value);
            current = nextNode;
        }
        if (newScript.SnakeList.Count == 1)
        {
            Destroy(newHead);
        }
        if (snakeList.Count == 1)
        {
            Destroy(transform.parent.gameObject);
        }
        resizeSnakeEvent.Invoke(snakeList.Count);
        StartCoroutine(SplittingCooldown(2));
        newScript.UpdateSpeed();
        UpdateSpeed();
        cameraScript.UpdateDistance();
    }
    public void UpdateSpeed()
    {
        RotationSpeed = Mathf.Clamp(100 + (snakeList.Count * 5), 0, MaxRotationSpeed);
        Speed = Mathf.Clamp(snakeList.Count + 5, 5, MaxSpeed);
    }
    private IEnumerator SplittingCooldown(int delay)
    {
        yield return new WaitForSeconds(delay);
        splittingIsActive = false;
    }
    public void StartStoneState()
    {
        StartCoroutine(StoneStateRoutine(snakeLine, spriteRenderer));
    }
    protected IEnumerator StunRoutine(float stunTime)
    {
        if (StunImmuneState || StunnedState || StoneState.IsActive)
            yield break;
        StunnedState = true;
        stunCircles.SetActive(true);
        DeActivateWhirlWind();
        yield return new WaitForSeconds(stunTime);
        StunnedState = false;
        stunCircles.SetActive(false);
        StartCoroutine(ImmuneRoutine(1));
    }

}
