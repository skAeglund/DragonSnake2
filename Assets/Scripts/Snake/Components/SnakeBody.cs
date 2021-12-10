using System.Collections;
using UnityEngine;
using System;

public class SnakeBody : MonoBehaviour
{
    [SerializeField] private GameObject bodyParent;
    [SerializeField] private Transform targetPos; // target position for the first bodypart
    private LineRenderer snakeLine;
    private AnimationCurve widthCurve;
    private bool splittingIsActive = false;
    public Action<int> resizeSnakeEvent;

    public MyLinkedList<GameObject> SnakeList { get; set; } = new MyLinkedList<GameObject>();
    public int StartingLength { get; set; } = 5;
    public bool IsEatingBomb { get; set; } = false;
    public bool IsBlue { get; set; } = false;


    public void Awake()
    {
        if (SnakeList.Count > 0) return;
        SnakeList.AddFirst(gameObject);
        snakeLine = GetComponent<LineRenderer>();
        widthCurve = snakeLine.widthCurve;
    }
    private void Start()
    {
        if (transform.root.name.Contains("Split")) return;
        GameObject prefab = IsBlue ? Prefabs.BlueBodyPart : Prefabs.BodyPart;
        GameObject newBody = Instantiate(prefab, targetPos.position - new Vector3(Grid.GridLength, 0, 0), targetPos.rotation, bodyParent.transform);
        SnakeList.AddLast(newBody);

        for (int i = 0; i < StartingLength; i++)
        {
            SpawnBodyPart(IsBlue);
        }
    }
    public void MoveBodyParts(float speed)
    {
        float bodyPartSpeed = (1 + speed * 0.1f) * 5; // determines how close together the bodyparts are
        float speedMultiplier = Time.deltaTime * bodyPartSpeed;
        ListNode<GameObject> current = SnakeList.First;
        Vector3 targetPosition = targetPos.position; // only used by the first bodypart after the head

        while (current.Next != null)
        {
            Transform currentTrans = current.Value.transform, nextTrans = current.Next.Value.transform;

            Vector3 position = Vector3.Lerp(nextTrans.position, targetPosition, speedMultiplier);
            Quaternion rotation = Quaternion.Lerp(nextTrans.rotation, currentTrans.rotation, speedMultiplier);

            current.Next.Value.transform.SetPositionAndRotation(position, rotation);

            targetPosition = nextTrans.position;
            current = current.Next;
        }
    }
    public void SpawnBodyPart(bool ofColorBlue = false)
    {
        GameObject bodyPrefab = ofColorBlue ? Prefabs.BlueBodyPart : Prefabs.BodyPart;
        GameObject newBody = Instantiate(bodyPrefab, targetPos.transform.position, targetPos.transform.rotation, bodyParent.transform);

        SnakeList.AddAfter(SnakeList.First.Value, newBody);
        newBody.GetComponent<BodyPart>().snakeHead = gameObject;
        newBody.transform.localScale = Vector3.zero;
        resizeSnakeEvent?.Invoke(SnakeList.Count);
    }
    public void DrawSnakeBody()
    {
        // sets the draw positions of the <Linerenderer> to the positions of all bodyparts
        // and adjusts the scale of each bodypart to fit inside the line
        Vector3[] bodyPartPositions = new Vector3[SnakeList.Count];
        bodyPartPositions[0] = targetPos.position;
        ListNode<GameObject> current = SnakeList.First.Next;
        for (int i = 1; i < SnakeList.Count; i++, current = current.Next)
        {
            bodyPartPositions[i] = current.Value.transform.position;
            float f1 = (float)i / ((float)SnakeList.Count - 1);
            float ballSize = Mathf.Lerp(0.001f, 0.3f, widthCurve.Evaluate(f1));
            if (i > 0)
                current.Value.transform.localScale = new Vector3(ballSize, ballSize, 1);
        }
        SnakeList.Last.Value.transform.localScale = Vector3.zero;
        snakeLine.widthCurve = widthCurve;
        snakeLine.positionCount = SnakeList.Count;
        snakeLine.SetPositions(bodyPartPositions);
    }
    public void ConvertIntoFood(GameObject bodyPart)
    {
        // converts all the body part after this ^ bodypart into food
        ListNode<GameObject> current = SnakeList.IndexOf(bodyPart) <= 1 ? SnakeList.First.Next.Next : SnakeList.Find(bodyPart);
        int counter = 0;
        while (current != null)
        {
            if (counter % 2 == 0)
            {
                SpawnManager.Instance.SpawnFoodAtPosition(current.Value.transform.position, transform.parent.name.Substring(0, 4) != "Blue");
            }
            SnakeList.Remove(current.Value);
            Destroy(current.Value);

            current = current.Next;
            counter++;
        }
        resizeSnakeEvent.Invoke(SnakeList.Count);
        DrawSnakeBody();
    }
    public void CompleteConversionToFood()
    {
        // converts all bodyparts into food
        ListNode<GameObject> current = SnakeList.First;
        int counter = 0;
        while (current != null)
        {
            if (counter % 2 == 0)
            {
                SpawnManager.Instance.SpawnFoodAtPosition(current.Value.transform.position, transform.parent.name.Substring(0, 4) != "Blue");
            }
            SnakeList.Remove(current.Value);
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
    public void SplitSnake(GameObject bodyPart)
    {
        // splits the snake at the position of this ^ bodypart and creates a new NPC snake
        if (splittingIsActive || bodyPart == null)
            return;
        splittingIsActive = true;

        int index = SnakeList.IndexOf(bodyPart);
        MyLinkedList<GameObject> newSnakeBody = SnakeList.Split(index);

        new SnakeBuilder().SetBody(newSnakeBody).SetScale(0.75f).SetSpawnPosition(bodyPart.transform.position).Build();

        if (SnakeList.Count == 1)
        {
            Destroy(transform.parent.gameObject);
        }
        resizeSnakeEvent.Invoke(SnakeList.Count);
        StartCoroutine(SplittingCooldown(2));
    }

    private IEnumerator SplittingCooldown(int delay)
    {
        yield return new WaitForSeconds(delay);
        splittingIsActive = false;
    }
}
