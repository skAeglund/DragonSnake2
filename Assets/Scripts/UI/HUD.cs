using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    static TMPro.TextMeshProUGUI scoreText;
    [SerializeField] Image[] imagesToFade;
    [SerializeField] Text[] textToFade;
    Material outlineMaterial;
    Material materialCopy;

    void Start()
    {
        scoreText = transform.Find("Score").GetComponent<TMPro.TextMeshProUGUI>();
        outlineMaterial = imagesToFade[0].material;
        materialCopy = new Material(outlineMaterial.shader);
        materialCopy.CopyPropertiesFromMaterial(outlineMaterial);
        StartCoroutine(FadeImages(5, 5));
    }

    public static void UpdateScore(int score)
    {
        scoreText.text = "Score: " + score.ToString();
        if (score > GameManager.highestScore)
        {
            GameManager.highestScore = score;
        }
    }

    private IEnumerator FadeImages(float delay, float timeToFade)
    {
        yield return new WaitForSeconds(delay);
        float startTime = Time.time;
        float originalOpacity = imagesToFade[0].color.a;
        Color outlineColor = outlineMaterial.GetVector("_OutlineColor");
        while (Time.time -startTime < timeToFade)
        {
            float t = (Time.time - startTime) / timeToFade;
            float a = Mathf.Lerp(originalOpacity, 0, t);
            float a2 = Mathf.Lerp(1, 0, t);
            for (int i = 0; i < imagesToFade.Length; i++)
            {
                imagesToFade[i].color = new Color(imagesToFade[i].color.r, imagesToFade[i].color.g, imagesToFade[i].color.b, a);
            }
            for (int i = 0; i < textToFade.Length; i++)
            {
                textToFade[i].color = new Color(textToFade[i].color.r, textToFade[i].color.g, textToFade[i].color.b, a2);
            }
            outlineMaterial.SetVector("_OutlineColor", new Color(outlineColor.r, outlineColor.g, outlineColor.b, a2));
            yield return new WaitForEndOfFrame();
        }
        for (int i = 0; i < imagesToFade.Length; i++)
        {
            Destroy(imagesToFade[i].gameObject);
        }
        outlineMaterial.CopyPropertiesFromMaterial(materialCopy);
    }
    private void OnApplicationQuit()
    {
        outlineMaterial.CopyPropertiesFromMaterial(materialCopy);
    }
}
