using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform canvas;
    static GameManager thisInstance;
    public static bool isQuitting = false;
    public static bool isGameOver = false;
    public static int highestScore = 0;

    public static List<MyLinkedList<GameObject>> ActiveSnakes {get;set;} = new List<MyLinkedList<GameObject>>();

    private void Start()
    {
        //gameOverEvent.AddListener(GameOver);
        thisInstance = this;
    }

    public static int GetLongestSnakeCount()
    {
        int highestCount = 0;
        foreach (var snakeList in ActiveSnakes)
        {
            if (snakeList.Count > highestCount)
                highestCount = snakeList.Count;
        }
        return highestCount;
    }
    public void GameOver()
    {
        GameObject gameOverParent = Instantiate(Prefabs.GameOverScreen, canvas);

        gameOverParent.transform.Find("Highscore").GetComponent<Text>().text += highestScore.ToString();

        isGameOver = true;
        Time.timeScale = 0;
    }
    public static void DelayedGameOver(float delay)
    {
        thisInstance.StartCoroutine(GameOverRoutine(delay));
    }
    private static IEnumerator GameOverRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        thisInstance.GameOver();
    }
    private void OnApplicationQuit()
    {
        isQuitting = true;
    }
}
