using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Whirlwind : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    [SerializeField] GameObject headSprite;
    [SerializeField] GameObject headSprite2;
    SpriteRenderer headSpriteRenderer;
    SpriteRenderer headSpriteRenderer2;
    Vector3 originalHeadPosition;
    GameObject nextMiniTornado;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        headSpriteRenderer = headSprite.GetComponent<SpriteRenderer>();
        if (headSprite2 != null)
            headSpriteRenderer2 = headSprite2.GetComponent<SpriteRenderer>();
        originalHeadPosition = headSprite.transform.localPosition;

    }

    private void OnEnable()
    {
        nextMiniTornado = transform.parent.GetComponent<SnakeBody>().SnakeList.First.Next.Value.transform.GetChild(1).gameObject;
        StartCoroutine(StartFlippingX());
    }
    private IEnumerator StartFlippingX()
    {
        float delay = 0.085f;
        headSpriteRenderer.flipY = true;
        headSprite.transform.localPosition = new Vector3(originalHeadPosition.x, -1f, 0);
        if (headSprite2 != null)
        {
            headSprite2.transform.localPosition = new Vector3(originalHeadPosition.x, -1f, 0);
            headSpriteRenderer2.flipY = true;
        }
        yield return new WaitForSeconds(delay);
        if (nextMiniTornado != null)
        {
            nextMiniTornado.SetActive(true);
        }
        while (true)
        {
            spriteRenderer.flipX = true;
            yield return new WaitForSeconds(delay);
            headSpriteRenderer.flipY = false;
            headSprite.transform.localPosition = originalHeadPosition;
            if (headSprite2 != null)
            {
                headSpriteRenderer2.flipY = false;
                headSprite2.transform.localPosition = originalHeadPosition;
            }
            yield return new WaitForSeconds(delay);
            spriteRenderer.flipX = false;
            yield return new WaitForSeconds(delay);
            headSpriteRenderer.flipY = true;
            headSprite.transform.localPosition = new Vector3(originalHeadPosition.x, -1f, 0);
            if (headSprite2 != null)
            {
                headSprite2.transform.localPosition = new Vector3(originalHeadPosition.x, -1f, 0);
                headSpriteRenderer2.flipY = true;
            }
            yield return new WaitForSeconds(delay);

        }
    }
    private void OnDisable()
    {
        headSpriteRenderer.flipY = false;
        headSprite.transform.localPosition = originalHeadPosition;
        if (headSprite2 != null)
        {
            headSpriteRenderer2.flipY = false;
            headSprite2.transform.localPosition = originalHeadPosition;
        }
        if (nextMiniTornado != null)
        {
            nextMiniTornado.GetComponentInChildren<MiniTornado>().DelayedDisable();
        }
    }
}
