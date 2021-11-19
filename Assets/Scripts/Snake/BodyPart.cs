using UnityEngine;

public class BodyPart : MonoBehaviour
{
    public GameObject snakeHead { get; set; }

    public void SplitSnake()
    {
        snakeHead.SendMessage("SplitSnake", gameObject);
    }
    public void ConvertIntoFood()
    {
        snakeHead.SendMessage("ConvertIntoFood", gameObject);
    }
}
