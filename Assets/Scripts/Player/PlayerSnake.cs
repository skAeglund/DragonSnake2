using System.Collections;
using UnityEngine;

public class PlayerSnake : Snake
{
    private int startingLength = 5;

    public bool IsEatingBomb { get; set; } = false;
    public MyLinkedList<GameObject> SnakeList { get => snakeList; set => snakeList = value; }


    #region Unity events
    public override void Awake()
    {
        base.Awake();
        snakeList.AddFirst(gameObject);
        resizeSnakeEvent.AddListener(HUD.UpdateScore);
    }
    private void Start()
    {
        GameObject newBody = Instantiate(Prefabs.BodyPart, targetPos.position - new Vector3(Grid.GridLength, 0, 0), targetPos.rotation, bodyParent.transform);
        snakeList.AddLast(newBody);
        GameManager.ActiveSnakes.Add(snakeList);

        for (int i = 0; i < startingLength; i++)
        {
            SpawnBodyPart();
        }
       
        resizeSnakeEvent.Invoke(snakeList.Count);
        ConnectSkillsToHUD();
    }
    void Update()
    {
        if (GameManager.isGameOver)
            return;
        if (FreeMovementActive)
        {
            MoveBodyParts();
        }
        if (!StoneState.IsActive)
        {
            DrawSnakeBody();
        }
    }
    #endregion
    private void ConnectSkillsToHUD()
    {
        Fireball.ConnectToHUD(HUDComponents.fireballUI);
        Whirlwind.ConnectToHUD(HUDComponents.whirlWindUI);
        StoneState.ConnectToHUD(HUDComponents.stoneSkillUI);
        Dash.ConnectToHUD(HUDComponents.dashUI);
    }
    #region Grid Movement - Press GRID in UI to activate
    public void GridBodyMovement(Vector3 lastGridPosition, Vector3 newGridPosition)
    {
        if (snakeList.Count <= 1 || StunnedState || StoneState.IsActive)
            return;

        ListNode<GameObject> current = snakeList.First.Next;
        StartCoroutine(MoveBodyPartToPosition(gameObject, newGridPosition, transform.rotation));      // head
        StartCoroutine(MoveBodyPartToPosition(current.Value, lastGridPosition, transform.rotation)); // first bodypart

        while (current.Next != null)
        {
            StartCoroutine(MoveBodyPartToPosition(current.Next.Value, current.Value.transform.position, current.Value.transform.rotation));    // rest of the bodyparts
            current = current.Next;
        }
    }
    private IEnumerator MoveBodyPartToPosition(GameObject bodypart, Vector3 targetPosition, Quaternion targetRotation)
    {
        float startTime = Time.time;
        float duration = GridMoveTime -0.1f;
        Vector3 startPosition = bodypart.transform.position;
        Quaternion startRotation = bodypart.transform.rotation;
        while (Time.time -startTime < duration && snakeList.Contains(bodypart) && !FreeMovementActive)
        {
            float t = (Time.time - startTime) / duration;
            bodypart.transform.position = Vector3.Lerp(startPosition, targetPosition, AnimationCurves.SmoothJump.Evaluate(t));
            if (bodypart != gameObject)
                bodypart.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, AnimationCurves.SmoothJump.Evaluate(t));
            yield return null;
        }
    }
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Food") && !IsEatingBomb)
        {
           SpawnBodyPart(other.name.Substring(0,4) == "Blue" ? true : false);
            SpawnManager.Foods.Remove(other.gameObject);
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("BodyPart"))
        {
            if (snakeList.Contains(other.gameObject))
                return;

            GameObject head = other.transform.parent.parent.Find("Head").gameObject;
            head.TryGetComponent(out PlayerSnake otherPlayer);
            head.TryGetComponent(out BlueDragon blueDragon);
            if (Whirlwind.IsActive)
            {
                if (otherPlayer != null)
                {
                    if (otherPlayer.StoneState.IsActive)
                    {
                        StartCoroutine(StunRoutine(2));
                        return;
                    }
                }
                Debug.Log("collided whirlwind");
                other.GetComponent<BodyPart>().ConvertIntoFood();
                DeActivateWhirlWind(0.33f);
                StartCoroutine(ImmuneRoutine(1));
            }
            else if (!StunnedState && !StunImmuneState)
            {
                // colliding with stunned player/NPC doesn't stun you
                if (otherPlayer != null) if (otherPlayer.StunnedState)
                        return;
                if (blueDragon != null)  if (blueDragon.StunnedState)
                        return;

                StartCoroutine(StunRoutine(1));
            }
        }
    }
}
