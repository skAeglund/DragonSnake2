using System.Collections;
using UnityEngine;
using SpriteGlow;

public class MiniTornado : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    MyLinkedList<GameObject> snakeList;
    GameObject nextTornado;

    private void OnEnable()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        snakeList = transform.parent.parent.parent.GetComponentInChildren<PlayerSnake>()?.SnakeList;
        ListNode<GameObject> nextNode = snakeList.Find(transform.parent.gameObject)?.Next;
        if (nextNode != null)
            nextTornado = nextNode.Value.transform.GetChild(1).gameObject;
        StartCoroutine(StartFlippingX());
    }
    public void DelayedDisable()
    {
        if (gameObject.activeSelf)
            StartCoroutine(DelayRoutine(0.1f));
    }
    private IEnumerator DelayRoutine(float delay)
    {
        float startTime = Time.time;
        Color originalColor = spriteRenderer.color;
        SpriteGlowEffect glow = GetComponent<SpriteGlowEffect>();
        float originalBrightness = glow.GlowBrightness;
        while (Time.time-startTime < delay)
        {
            float t = (Time.time - startTime) / delay;
            spriteRenderer.color = Color.Lerp(originalColor, Color.clear, AnimationCurves.EaseInOut.Evaluate(t));
            glow.GlowBrightness = Mathf.Lerp(originalBrightness, 0, AnimationCurves.EaseInOut.Evaluate(t));
            yield return null;
        }
        gameObject.SetActive(false);
        spriteRenderer.color = originalColor;
        glow.GlowBrightness = originalBrightness;
        if (nextTornado != null)
        {
             nextTornado.GetComponentInChildren<MiniTornado>()?.DelayedDisable();
        }
    }
    private IEnumerator StartFlippingX()
    {
        float delay = 0.1f;
        spriteRenderer.flipX = true;
        yield return new WaitForSeconds(delay/2);
        // enable next
        if (nextTornado != null)
            nextTornado.SetActive(true);
        while (true)
        {
            spriteRenderer.flipX = false;
            yield return new WaitForSeconds(delay);
            spriteRenderer.flipX = true;
            yield return new WaitForSeconds(delay);
        }
    }
}
