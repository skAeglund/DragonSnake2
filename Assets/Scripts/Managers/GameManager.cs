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

    //public static List<MyLinkedList<GameObject>> ActiveSnakes {get;set;} = new List<MyLinkedList<GameObject>>();

    public List<SnakeManager> ActiveSnakes { get; set; } = new List<SnakeManager>();

    private void Start()
    {
        thisInstance = this;
    }

    public int GetLongestSnakeCount()
    {
        int highestCount = 0;
        foreach (SnakeManager snakeManager in ActiveSnakes)
        {
            if (snakeManager.SnakeList.Count > highestCount)
                highestCount = snakeManager.SnakeList.Count;
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
