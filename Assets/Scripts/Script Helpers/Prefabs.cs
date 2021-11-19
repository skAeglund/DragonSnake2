using UnityEngine;

public class Prefabs : MonoBehaviour
{
    [SerializeField] private GameObject bomb;
    [SerializeField] private GameObject food;
    [SerializeField] private GameObject bodyPart;
    [SerializeField] private GameObject explosion;
    [SerializeField] private GameObject snakeHead;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject splitSnakeHead;
    [SerializeField] private GameObject blueDragonHead;
    [SerializeField] private GameObject blueBodyPart;
    [SerializeField] private GameObject blueFood;
    [SerializeField] private GameObject fireball;
    [SerializeField] private GameObject bigFireball;
    [SerializeField] private GameObject dragonArm;
    [SerializeField] private GameObject skillLight;

    public static GameObject Bomb           { get; private set; }
    public static GameObject Food           { get; private set; }
    public static GameObject BodyPart       { get; private set; }
    public static GameObject Explosion      { get; private set; }
    public static GameObject SnakeHead      { get; private set; }
    public static GameObject GameOverScreen { get; private set; }
    public static GameObject SplitSnakeHead { get; private set; }
    public static GameObject BlueDragonHead { get; private set; }
    public static GameObject BlueBodyPart   { get; private set; }
    public static GameObject BlueFood       { get; private set; }
    public static GameObject Fireball       { get; set; }
    public static GameObject BigFireball    { get; set; }
    public static GameObject DragonArm      { get; private set; }
    public static GameObject SkillLight     { get; private set; }

    void Awake()
    {
        Bomb = bomb;
        Food = food;
        BodyPart = bodyPart;
        Explosion = explosion;
        SnakeHead = snakeHead;
        GameOverScreen = gameOverScreen;
        SplitSnakeHead = splitSnakeHead;
        BlueDragonHead = blueDragonHead;
        BlueBodyPart = blueBodyPart;
        BlueFood = blueFood;
        Fireball = fireball;
        BigFireball = bigFireball;
        DragonArm = dragonArm;
        SkillLight = skillLight;
    }

}
