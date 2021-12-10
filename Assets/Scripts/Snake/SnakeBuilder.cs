using UnityEngine;

public class SnakeBuilder : MonoBehaviour
{
    private MyLinkedList<GameObject> snakeList = new MyLinkedList<GameObject>();
    private bool isBlue = false;
    private float scaleMultiplier = 1;
    private Vector3 spawnPosition;
    private int startingLength;
    
    public SnakeBuilder SetBody(MyLinkedList<GameObject> snakeList)
    {
        this.snakeList = snakeList;
        return this;
    }
    public SnakeBuilder SetBlueColor()
    {
        isBlue = true;
        return this;
    }
    public SnakeBuilder SetScale(float scaleMultiplier)
    {
        this.scaleMultiplier = scaleMultiplier;
        return this;
    }
    public SnakeBuilder SetSpawnPosition(Vector3 position)
    {
        spawnPosition = position;
        return this;
    }
    public SnakeBuilder SetStartingLength(int startingLength)
    {
        this.startingLength = startingLength;
        return this;
    }
    public GameObject Build()
    {
        if (spawnPosition == null)
            spawnPosition = snakeList.First.Value.transform.position;

        GameObject prefab = isBlue ? Prefabs.BlueDragonHead : Prefabs.SplitSnakeHead;
        GameObject snakeHeadParent = Instantiate(prefab, spawnPosition, Quaternion.identity);
        GameObject snakeHead = snakeHeadParent.transform.Find("Head").gameObject;
        Transform bodyParent = snakeHeadParent.transform.Find("Body");
        snakeHeadParent.transform.localScale *= scaleMultiplier;
        snakeList.AddFirst(snakeHead);
        if (snakeList.Count > 1)
        {
            UpdateBodyParts(snakeHead, bodyParent);
        }
        if (!isBlue)
        {
            NonAttackingAISnake splitSnakeScript = snakeHead.GetComponent<NonAttackingAISnake>();
            splitSnakeScript.SnakeList = snakeList;
        }
        else if (startingLength != 0)
        {
            SnakeBody attackingAI = snakeHead.GetComponent<SnakeBody>();
            attackingAI.StartingLength = startingLength;
        }
        snakeHead.SendMessage("UpdateSpeed", snakeList.Count);

        return snakeHead;
    }

    private void UpdateBodyParts(GameObject snakeHead, Transform parent, SnakeBody snakeBody = null)
    {
        ListNode<GameObject> current = snakeList.First.Next;
        while (current != null)
        {
            BodyPart bodyPart = current.Value.GetComponent<BodyPart>();
            bodyPart.snakeHead = snakeHead;
            bodyPart.snakeBody = snakeBody;
            bodyPart.transform.SetParent(parent);
            current = current.Next;
        }
    }

}
