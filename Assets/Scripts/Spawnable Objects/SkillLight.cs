using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillLight : MonoBehaviour
{
    Light _light;
    MyLinkedList<GameObject> snakeList;
    float intensity;
    private void Start()
    {
        snakeList = transform.parent.GetComponentInChildren<SnakeBody>().SnakeList;

        _light = GetComponent<Light>();
        intensity = Mathf.Lerp(125, _light.intensity, snakeList.Count / 20);
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float duration = 0.4f;
        float startTime = Time.time;
        float startIntensity = intensity;
        while (Time.time - startTime < duration)
        {
            float t = (Time.time - startTime) / duration;
            _light.intensity = Mathf.Lerp(startIntensity, 0, t);
            yield return null;
        }
        Destroy(gameObject);
    }
}
