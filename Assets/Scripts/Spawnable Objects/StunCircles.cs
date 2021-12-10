using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunCircles : MonoBehaviour
{
    SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private IEnumerator FlipYRoutine()
    {
        float delay = 0.2f;
        while (true)
        {
            yield return new WaitForSeconds(delay);
            spriteRenderer.flipY = true;
            yield return new WaitForSeconds(delay);
            spriteRenderer.flipY = false;
        }
    }
    private void OnEnable()
    {
        StartCoroutine(FlipYRoutine());
    }
}
