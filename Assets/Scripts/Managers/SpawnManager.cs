using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] private float foodDelay = 2;
    [SerializeField] private float bombDelay = 2;
    private PlayerController playerSnakeScript;
    private Queue<GameObject> foodPool = new Queue<GameObject>();
    private Transform foodParent;
    private Transform bombParent;
    private int foodAmount = 10;
    
    private float spawnDistanceFromPlayer = 7;
    
    GameObject blueDragon;

    public delegate void FoodSpawnEvent(GameObject newFood);

    public event FoodSpawnEvent onFoodSpawn;

    public static SpawnManager Instance;
    public LinkedList<GameObject> Bombs { get; set; } = new LinkedList<GameObject>();
    public LinkedList<GameObject> Foods { get; set; } = new LinkedList<GameObject>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

        playerSnakeScript = player.GetComponent<PlayerController>();
        foodParent = transform.Find("Foods");
        bombParent = transform.Find("Bombs");
        StartCoroutine(SpawnFood());
        StartCoroutine(SpawnBombs());
        StartCoroutine(BlueDragonRoutine(1));
    }
    #region Food region
    private IEnumerator SpawnFood()
    {
        for (int i = 0; i < foodAmount; i++)
        {
            // fill the pool
            GameObject foodObject = Instantiate(Prefabs.Food, foodParent);
            foodPool.Enqueue(foodObject);
        }
        while (player != null)
        {
            // spawns new food at the specified interval OR when only 1 food is active
            ReleaseFoodFromPool();
            float startTime = Time.time;
            while (Time.time - startTime < foodDelay && Foods.Count > 1)
            {
                yield return new WaitForEndOfFrame();
            }
        }
    }
    private void ReleaseFoodFromPool()
    {
        int randomX = Random.Range(0, 16);
        int randomY = Random.Range(0, 9);
        Vector3 randomPosition = Grid.GridPositions[randomX, randomY];

        GameObject food = foodPool.Dequeue();
        food.transform.position = randomPosition;
        food.SetActive(true);
        Foods.AddLast(food);
        onFoodSpawn?.Invoke(food);
    }
    public void ReturnFoodToPool(GameObject food)
    {
        food.SetActive(false);
        foodPool.Enqueue(food);
        Foods.Remove(food);
    }
    public void SpawnFoodAtPosition(Vector3 position, bool ofColorRed = true)
    {
        GameObject foodPrefab = ofColorRed ? Prefabs.Food : Prefabs.BlueFood;

        GameObject food = Instantiate(foodPrefab, position, Quaternion.identity, Instance.transform);
        if (!food.activeSelf)
            food.SetActive(true);
        Foods.AddLast(food);
    }
    public GameObject GetClosestFood(Vector3 position, out float shortestDistance)
    {
        if (Foods.Count == 0)
        {
            shortestDistance = -1;
            return null;
        }
        shortestDistance = float.MaxValue;
        GameObject closestFood = Foods.First.Value;
        foreach (GameObject food in Foods)
        {
            float distance = Vector3.Distance(position, food.transform.position);
            if (distance < shortestDistance)
            {
                closestFood = food;
                shortestDistance = distance;
            }
        }
        return closestFood;
    }
    #endregion

    #region DragonSpawn
    public void SpawnBlueDragon(float delay = 0)
    {
        if (Instance != null)
            Instance.StartCoroutine(Instance.BlueDragonRoutine(delay));
    }
    public void SpawnBlueDragon()
    {
        Instance.StartCoroutine(Instance.BlueDragonRoutine());
    }
    private IEnumerator BlueDragonRoutine(float delay = 0)
    {
        yield return new WaitForSeconds(delay);
        while (playerSnakeScript.SnakeList.Count < 16)
        {
            yield return new WaitForSeconds(1);
        }
        if (blueDragon != null)
            yield break;

        
        Vector3 spawnPosition = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, Camera.main.transform.position.z));
        spawnPosition.z = 0;
        blueDragon = new SnakeBuilder().SetBlueColor().SetSpawnPosition(spawnPosition).SetStartingLength((int)(playerSnakeScript.SnakeList.Count * 0.6f)).Build();
        //blueDragon = Instantiate(Prefabs.BlueDragonHead, spawnPosition, Quaternion.identity);
        //SnakeBody body = blueDragon.GetComponentInChildren<SnakeBody>();
        //body.StartingLength = (int)(playerSnakeScript.SnakeList.Count *0.6f);
    }
    #endregion


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
            GameObject bomb = Instantiate(Prefabs.Bomb, randomPosition, Quaternion.identity, bombParent);
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
   
    public void DestroyAfterDelay(float delay, GameObject objectToDestroy)
    {
        Instance.StartCoroutine(DelayedDestruction(delay, objectToDestroy));
    }
    public IEnumerator DelayedDestruction(float delay, GameObject objectToDestroy)
    {
        yield return new WaitForSeconds(delay);
        Destroy(objectToDestroy);
    }
}
