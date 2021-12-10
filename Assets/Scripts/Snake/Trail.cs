using System.Collections;
using UnityEngine;

/// <summary>
/// Handles the trails on the player snake
/// Activates when the player dashes (unity event)
/// </summary>
public class Trail : MonoBehaviour
{
    TrailRenderer trailRenderer;

    private void Awake()
    {
        trailRenderer = GetComponent<TrailRenderer>();
    }

    public void PulseOfVisibility()
    {
        trailRenderer.emitting = true;
        trailRenderer.time = 1;
        StopAllCoroutines();
        StartCoroutine(FadeOutRoutine(1.5f));
    }

    private IEnumerator FadeOutRoutine(float duration)
    {
        float startTime = Time.time;
        while (Time.time - startTime < duration)
        {
            float t = (Time.time - startTime) / duration;
            trailRenderer.time = Mathf.SmoothStep(1, 0, t);
            yield return new WaitForSeconds(0.01f);
        }
        trailRenderer.emitting = false;
    }

}
