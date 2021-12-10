using UnityEngine;

public class BodyPart : MonoBehaviour
{
    public GameObject snakeHead { get; set; }
    public SnakeBody snakeBody { get; set; }

    private void Awake()
    {
        if (snakeHead == null)
        {
            snakeHead = transform.root.GetChild(0).gameObject;
        }
        if (snakeBody == null)
        {
            snakeBody = transform.root.GetComponentInChildren<SnakeBody>();
        }
    }
    public void SplitSnake()
    {
        snakeHead.SendMessage("SplitSnake", gameObject);
    }
    public void ConvertIntoFood()
    {
        snakeHead.SendMessage("ConvertIntoFood", gameObject);
    }
}
