using System.Collections;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] GameObject explosionPrefab;
    public bool isInsideSnake { get; private set; } = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(MoveInsideSnake(collision.gameObject.GetComponent<SnakeBody>()));
            GetComponent<SphereCollider>().enabled = false;
        }
    }
    private IEnumerator MoveInsideSnake(SnakeBody snake)
    {
        // Makes the bomb move inside the snake and explode at a random position, splitting the snake into two
        if (snake.IsEatingBomb)
            yield break;
        MyLinkedList<GameObject> snakeList = snake.SnakeList;
        if (snakeList.Count <=2 /*|| snake.isEatingBomb*/)
        {
            GameManager.DelayedGameOver(0.75f);
            SpawnManager.Instance.Bombs.Remove(gameObject);
            Destroy(gameObject);
            yield break;
        }
        isInsideSnake = true;
        snake.IsEatingBomb = true;
        int randomExplodeIndex = snakeList.Count > 4 ? Random.Range(2, snakeList.Count - 2) : 2;
        float duration = Mathf.Clamp(0.5f - (float)(randomExplodeIndex * 0.02), 0.1f, 0.5f);
        ListNode<GameObject> current = snakeList.First;
        for (int i = 0; i < randomExplodeIndex && current.Next != null; i++, current = current.Next)
        {
            float startTime = Time.time;
            while (Time.time - startTime < duration)
            {
                if (current.Value != null)
                transform.position = current.Value.transform.position;
                yield return new WaitForEndOfFrame();
            }
        }
        // explode
        snake.IsEatingBomb = false;
        snake.SplitSnake(current.Value);
        SpawnManager.Instance.Bombs.Remove(gameObject);
        Destroy(gameObject);
    }
    private void OnDestroy()
    {
        if (!GameManager.isQuitting)
        {
            GameObject explosionObject = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            SpawnManager.Instance.DestroyAfterDelay(0.5f, explosionObject);
        }
    }
}
