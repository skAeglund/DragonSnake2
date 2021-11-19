using System.Collections;
using UnityEngine;

public class Clouds : MonoBehaviour
{
    [SerializeField] float cycleLength = 5;
    private int cycleCount = 0;
    private float spriteWidth = 87.5f;

    private void Start()
    {
        cycleCount = Random.Range(0, 4);
        spriteWidth *= transform.localScale.x;
        StartCoroutine(MoveClouds());
        BoxCollider collider = GetComponent<BoxCollider>();
        if (collider != null)
        collider.center -= new Vector3(0, 0, transform.position.z);

    }
    private IEnumerator MoveClouds()
    {
        float startTime = Time.time;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = new Vector3(startPosition.x - spriteWidth, startPosition.y, startPosition.z);
        while (Time.time - startTime < cycleLength)
        {
            float t = (Time.time - startTime) / cycleLength;
            transform.position = Vector3.Lerp(startPosition, targetPosition, AnimationCurves.CloudCycles[cycleCount].Evaluate(t));
            yield return null;
        }
        transform.position += new Vector3(spriteWidth, 0, 0);
        cycleCount = cycleCount < 3 ? cycleCount++ : 0;
        StartCoroutine(MoveClouds());
    }
}
