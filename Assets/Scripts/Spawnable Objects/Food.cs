using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    SphereCollider sphereCollider;
    float immunityTime = 0.1f;
    

    void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.enabled = false;
        StartCoroutine(DelayedCollider(immunityTime));
        //StartCoroutine(FloatingRoutine());
    }
    private IEnumerator FloatingRoutine()
    {
        float duration = Random.Range(0.5f, 1f);

        int sign = -1;
        while (true)
        {
            sign = sign == -1 ? 1 : -1;
            Vector3 startPosition = transform.position;
            Vector3 endPosition = startPosition - new Vector3(0, duration * sign, 0);
            float startTime = Time.time;
            while (Time.time -startTime < duration)
            {
                float t = (Time.time - startTime) / duration;
                transform.position = Vector3.Lerp(startPosition, endPosition, AnimationCurves.EaseInOut2.Evaluate(t));
                yield return new WaitForEndOfFrame();
            }

        }
    }

    private IEnumerator DelayedCollider(float secondsOfDelay)
    {
        yield return new WaitForSeconds(secondsOfDelay);
        sphereCollider.enabled = true;
    }

}
