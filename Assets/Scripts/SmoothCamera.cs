using UnityEngine;

public class SmoothCamera : MonoBehaviour
{
    [SerializeField] float camSpeed = 4f;
    [SerializeField] PlayerController playerSnake;
    float targetZPosition;
    float maxDistance = 20;
    float zMultiplier = 1;

    void Update()
    {
        if (playerSnake == null) return;

        if (Input.GetKeyDown(KeyCode.KeypadPlus))
            zMultiplier = Mathf.Clamp(zMultiplier + 0.2f, 1, 2);
        else if (Input.GetKeyDown(KeyCode.KeypadMinus))
            zMultiplier = Mathf.Clamp(zMultiplier - 0.2f, 1, 2);

        float distanceToPlayer = (transform.position - playerSnake.transform.position).magnitude;
        float f = distanceToPlayer / maxDistance;
        camSpeed = Mathf.Lerp(1, 5, f);
        Vector3 targetPos = new Vector3(playerSnake.transform.position.x, playerSnake.transform.position.y, targetZPosition *zMultiplier);
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * camSpeed);

        if (!playerSnake.SnakeList.Last.Value.GetComponent<Renderer>().isVisible)
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.01f);
    }
    public void UpdateDistance(int snakeSize)
    {
        targetZPosition = Mathf.Clamp(-20 +(-snakeSize), -80, -20);
    }

}
