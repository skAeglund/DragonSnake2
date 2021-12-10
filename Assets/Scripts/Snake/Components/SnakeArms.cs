using System.Collections;
using UnityEngine;

/// <summary>
/// Handles the positioning and scaling of the arms & feet of the player snake
/// </summary>
public class SnakeArms : MonoBehaviour
{
    [SerializeField] SnakeBody snakeBody;
    private SnakeAbilities abilities;
    private GameObject connectedBodyPart;
    private bool actuallyFeet = false;
    private Vector3 targetPosition;
    private bool isChangingTarget;

    private void Start()
    {
        snakeBody.resizeSnakeEvent += UpdatePositions;
        actuallyFeet = name.ToLower().Contains("feet");
        gameObject.SetActive(false);
        abilities = snakeBody?.gameObject.GetComponent<SnakeAbilities>();
    }

    public void SetBodyPart(GameObject bodypart)
    {
        GameObject oldBodyPart = connectedBodyPart;
        connectedBodyPart = bodypart;

        if (oldBodyPart != null)
        {
            StopAllCoroutines();
            StartCoroutine(ChangeTarget(oldBodyPart));
        }
        else
        {
            connectedBodyPart = bodypart;
        }
    }
    private void LateUpdate()
    {
        if (connectedBodyPart == null || abilities.StoneState.IsActive)
            return;
        float feetModifier = actuallyFeet ? 0.7f : 0.9f;
        float size = Mathf.Clamp(0.5f + snakeBody.SnakeList.Count * 0.015f, 0.5f, 1.05f);

        transform.localScale = new Vector3(size , size * feetModifier, 1);

        if (!isChangingTarget)
            targetPosition = connectedBodyPart.transform.position;
       
        transform.position = targetPosition;
        float rotationSpeed = actuallyFeet ? 10 : 4;
        float rightSideMult = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) ? 2 : 1;
        Quaternion rotation = actuallyFeet ? connectedBodyPart.transform.rotation : snakeBody.transform.rotation;
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation,  (Time.deltaTime * rotationSpeed ) * rightSideMult);
    }
    private IEnumerator ChangeTarget(GameObject oldBodyPart)
    {
        isChangingTarget = true;
        float startTime = Time.time;
        float duration = 0.6f;
        Vector3 oldPosition = oldBodyPart.transform.position;
        while (Time.time - startTime < duration)
        {
            float t = (Time.time - startTime) / duration;
            targetPosition = Vector3.Lerp(oldPosition, connectedBodyPart.transform.position,  AnimationCurves.Arms.Evaluate(t));
            if (oldBodyPart != null)
                oldPosition = oldBodyPart.transform.position;
            yield return new WaitForEndOfFrame();
        }
        targetPosition = connectedBodyPart.transform.position;
        isChangingTarget = false;
    }
    public void UpdatePositions(int count)
    {
        bool activeState = count <= 10 && actuallyFeet ? false :
                           count <= 8 && !actuallyFeet ? false : true;

        if (gameObject.activeSelf != activeState)
        {
            gameObject.SetActive(activeState);
        }
        if (activeState)
        {
            int index = actuallyFeet ? (int)((snakeBody.SnakeList.Count - 1) * 0.4f) : snakeBody.SnakeList.Count < 20 ? 1 : 2;
            if (connectedBodyPart != snakeBody.SnakeList[index])
            {
                SetBodyPart(snakeBody.SnakeList[index]);
            }
        }
    }
}
