using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] GameObject player;
    PlayerSnake playerSnakeScript;
    private static SpawnManager instance;
    private float spawnDistanceFromPlayer = 7;
    [SerializeField] private float foodDelay = 2;
    [SerializeField] private float bombDelay = 2;
    GameObject blueDragon;
    private bool dragonHasBeenSpawnedOnce = false;

    public static LinkedList<GameObject> Bombs { get; set; } = new LinkedList<GameObject>();
    public static LinkedList<GameObject> Foods { get; set; } = new LinkedList<GameObject>();

    void Start()
    {
        instance =this;
        playerSnakeScript = player.GetComponent<PlayerSnake>();
        StartCoroutine(SpawnFood());
        StartCoroutine(SpawnBombs());
        StartCoroutine(BlueDragonRoutine());
    }

    public static void SpawnBlueDragon(float delay = 0)
    {
        if (instance != null)
            instance.StartCoroutine(instance.BlueDragonRoutine(delay));
    }
    public void SpawnBlueDragon()
    {
        instance.StartCoroutine(instance.BlueDragonRoutine());
    }
    private IEnumerator BlueDragonRoutine(float delay = 0)
    {
        while (playerSnakeScript.SnakeList.Count < 16)
        {
            yield return new WaitForSeconds(1);
        }
        yield return new WaitForSeconds(delay);
        if (blueDragon != null)
            yield break;

        
        Vector3 spawnPosition = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, Camera.main.transform.position.z));
        spawnPosition.z = 0;
        blueDragon = Instantiate(Prefabs.BlueDragonHead, spawnPosition, Quaternion.identity);
        BlueDragon blueDragonScript = blueDragon.GetComponentInChildren<BlueDragon>();
        blueDragonScript.DisableLevelSystem();

        if (!dragonHasBeenSpawnedOnce)
        {
            blueDragonScript.Fireball.IsOnCooldown = true;
            blueDragonScript.Fireball.HasLearned = false;
            blueDragonScript.StartingLength = playerSnakeScript.SnakeList.Count - 3;
        }
        else
        {
            blueDragonScript.StartingLength = (int)(playerSnakeScript.SnakeList.Count *0.6f);
        }
        dragonHasBeenSpawnedOnce = true;
    }
    private IEnumerator SpawnFood()
    {
        while (player != null)
        {
            int randomX = Random.Range(0, 16);
            int randomY = Random.Range(0, 9);
            Vector3 randomPosition = Grid.GridPositions[randomX, randomY];
            GameObject food = Instantiate(Prefabs.Food, randomPosition, Quaternion.identity, transform);
            Foods.AddLast(food);

            float startTime = Time.time;
            while (Time.time - startTime < foodDelay && Foods.Count != 0)
            {
                yield return new WaitForEndOfFrame();
            }
        }
    }
    public static void SpawnFoodAtPosition(Vector3 position, bool ofColorRed = true)
    {
        GameObject foodPrefab = ofColorRed ? Prefabs.Food : Prefabs.BlueFood;

        GameObject food = Instantiate(foodPrefab, position, Quaternion.identity, instance.transform);
        Foods.AddLast(food);
    }
    private IEnumerator SpawnBombs()
    {
        while (true && player != null)
        {
            yield return new WaitForSeconds(bombDelay);
            if (player == null) yield break;
            Vector3 randomPosition = player.transform.position;
            while (Vector3.Distance(player.transform.position, randomPosition) < spawnDistanceFromPlayer)
            {
                int randomX = Random.Range(0, 16);
                int randomY = Random.Range(0, 9);
                randomPosition = Grid.GridPositions[randomX, randomY];
            }
            GameObject bomb = Instantiate(Prefabs.Bomb, randomPosition, Quaternion.identity, transform);
                Bombs.AddLast(bomb);
            
            if(Bombs.Count >= 3)
            {
                if (!Bombs.First.Value.GetComponent<Bomb>().isInsideSnake)
                {
                    Destroy(Bombs.First.Value);
                    Bombs.RemoveFirst();
                }
                else
                {
                    Destroy(Bombs.First.Next.Value);
                    Bombs.Remove(Bombs.First.Next);
                }

            }    
        }
    }

    public static void DestroyAfterDelay(float delay, GameObject objectToDestroy)
    {
        instance.StartCoroutine(DelayedDestruction(delay, objectToDestroy));
    }
    public static IEnumerator DelayedDestruction(float delay, GameObject objectToDestroy)
    {
        yield return new WaitForSeconds(delay);
        Destroy(objectToDestroy);
    }
}
