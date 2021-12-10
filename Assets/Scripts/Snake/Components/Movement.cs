using UnityEngine;
using System.Collections;
using Enums;

public class Movement : MonoBehaviour
{
    private float speed = 7;
    private float maxSpeed = 40;
    private float maxBoostSpeed = 80;
    private float rotationSpeed = 100;
    private float maxRotationSpeed = 295;
    private float rotationBuffAmount = 1.4f;
    private float slowDownAmount = 0.5f;

    public bool StunnedState { get; set; } = false;
    public float Speed 
    { 
        get => speed;
        set => speed = Mathf.Clamp(value, 0, maxBoostSpeed); 
    }
    public float RotationSpeed 
    { 
        get => rotationSpeed; 
        set => rotationSpeed = Mathf.Clamp(value, 0, maxRotationSpeed); 
    }

    public void Move(bool slowState)
    {
        if (StunnedState) return;
        // moves the snake head in the direction of transform.right

        float multiplier = slowState ? slowDownAmount : 1;
        transform.position += (transform.right * Time.deltaTime * speed) * multiplier;
    }
    public void Rotate(RotationDirection direction, bool slowState)
    {
        if (StunnedState || direction == RotationDirection.None) return;

        float multiplier = slowState ? rotationBuffAmount : 1;
        float maxLimit = maxRotationSpeed * 1.2f;
        float rotation = Mathf.Clamp(rotationSpeed * multiplier, rotationSpeed, maxLimit) * ((float)direction);

        transform.Rotate(0, 0, Time.deltaTime * rotation);
    }
    public void UpdateSpeed(int snakeSize)
    {
        rotationSpeed = Mathf.Clamp(100 + (snakeSize * 5), 0, maxRotationSpeed);
        speed = Mathf.Clamp(snakeSize + 5, 5, maxSpeed);
    }
}
