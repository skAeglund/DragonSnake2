using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using MyStructs;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] UnityEvent OnStartDashEvent;
    private PlayerSnake snake;
    private Point currentGrid;
    private Point lastGrid;

    private void Awake()
    {
        snake = GetComponent<PlayerSnake>();
        snake.UpdateSpeed();
        transform.position = Grid.GridPositions[8, 4];
        currentGrid.x =8;
        currentGrid.y = 4;
        transform.rotation = Quaternion.identity;
    }
    private void Start()
    {
        if (!snake.FreeMovementActive)
            StartCoroutine(GridMovementPush());
    }
    private void Update()
    {
        if (snake.StoneState.IsActive || snake.StunnedState)
            return;

        if (snake.FreeMovementActive)
        {
            FreeMovementInput();
            FreeMovement();
        }
        else
        {
            GridMovement();
        }
        if (Input.anyKeyDown)
            AbilityInput();
    }
    #region Grid Movement
    private void GridMovement()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            transform.Rotate(0, 0, transform.right == Vector3.up ? 90 : transform.right == Vector3.down ? -90 : 0);
        }
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            transform.Rotate(0, 0, transform.right == Vector3.up ? -90 : transform.right == Vector3.down ? 90 : 0);
        }
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (transform.right != Vector3.down)
                transform.right = Vector3.up;
        }
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (transform.right != Vector3.up)
                transform.right = Vector3.down;
        }
    }
   
    private IEnumerator GridMovementPush()
    {
        while (!snake.FreeMovementActive)
        {
            
            lastGrid = currentGrid;
            if (transform.right == Vector3.right)
            {
                currentGrid.x++;
            }
            else if (transform.right == Vector3.up)
            {
                currentGrid.y++;
            }
            else if (transform.right == Vector3.left)
            {
                currentGrid.x--;
            }
            else if (transform.right == Vector3.down)
            {
                currentGrid.y--;
            }
            // outside grid
            if (currentGrid.y > Grid.GridPositions.GetLength(1) - 1 || currentGrid.y < 0 ||
                currentGrid.x > Grid.GridPositions.GetLength(0) - 1 || currentGrid.x < 0)
            {
                currentGrid.y = Mathf.Clamp(currentGrid.y, 0, Grid.GridPositions.GetLength(1) - 1);
                currentGrid.x = Mathf.Clamp(currentGrid.x, 0, Grid.GridPositions.GetLength(0) - 1);
            }
            else // inside grid
            {
                Vector3 lastPos = Grid.GridPositions[lastGrid.x, lastGrid.y];
                Vector3 newPos = Grid.GridPositions[currentGrid.x, currentGrid.y];
                snake.GridBodyMovement(lastPos, newPos);
            }

            yield return new WaitForSeconds(snake.GridMoveTime);
            while (snake.StunnedState || snake.StoneState.IsActive)
            {
                yield return new WaitForEndOfFrame();
            }
        }
    }
    #endregion

    #region Free Movement
    private void FreeMovement()
    {
        // moves the snake head in the direction of it's transform.right
        float slowDown = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) ? 0.5f : 1;
        transform.position += (transform.right * Time.deltaTime * snake.Speed) * slowDown;
    }
    private void FreeMovementInput()
    {
        float rotationBuff = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) ? 1.4f : 1;

        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (!snake.Dash.IsOnCooldown)
                StartCoroutine(DashRoutine());
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(0, 0, Time.deltaTime * Mathf.Clamp(snake.RotationSpeed * rotationBuff, snake.RotationSpeed, snake.MaxRotationSpeed * 1.2f));
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(0, 0, Time.deltaTime * -Mathf.Clamp(snake.RotationSpeed * rotationBuff, snake.RotationSpeed, snake.MaxRotationSpeed * 1.2f));
        }
    }
    private void AbilityInput()
    {
        if (!snake.Whirlwind.IsOnCooldown && snake.FreeMovementActive)
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Alpha2))
            {
                StartCoroutine(snake.WhirlwindRoutine());
            }
        if (!snake.StoneState.IsOnCooldown)
            if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl) || Input.GetKeyDown(KeyCode.Alpha3))
            {
                snake.StartStoneState();
            }
        if (!snake.Fireball.IsOnCooldown)
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Alpha1))
            {
                StartCoroutine(snake.FireballRoutine(transform.right));
            }
    }

    #endregion
    private IEnumerator DashRoutine()
    {
        // this will be moved to SnakeAbilities.cs 
        snake.CreateSkillLight();
        snake.Dash.Activate();
        OnStartDashEvent.Invoke();
        float startTime = Time.time;
        float duration = Mathf.Clamp(0.5f - (snake.SnakeList.Count * 0.01f), 0.3f, 0.5f);
        float startSpeed = snake.Speed;
        float startRotationSpeed = snake.RotationSpeed;

        while (Time.time - startTime < duration)
        {
            float elapsed = Time.time - startTime;
            float t = elapsed / duration;
            snake.Speed = Mathf.Clamp(Mathf.Lerp(startSpeed, startSpeed * 4, t), startSpeed, snake.MaxBoostSpeed);
            snake.RotationSpeed = (Mathf.Lerp(startRotationSpeed, startRotationSpeed * 2, t));
            snake.Dash.UpdateTimeLeftOnCooldown();
            yield return new WaitForEndOfFrame();
        }
        float topSpeed = snake.Speed;
        //yield return new WaitForSeconds(duration);
        while //(Time.time - startTime - (duration/**2*/) < duration)
            (snake.Dash.IsActive)
        {
            float elapsed = Time.time - startTime - duration/**2*/;
            float t = elapsed / (duration /**2*/);
            snake.Speed = Mathf.SmoothStep(topSpeed, startSpeed, t);
            snake.RotationSpeed = (Mathf.SmoothStep(startRotationSpeed * 2, startRotationSpeed, t));
            snake.Dash.UpdateTimeLeftOnCooldown();
            yield return new WaitForEndOfFrame();
        }
        snake.Dash.UpdateTimeLeftOnCooldown();
        snake.Dash.EndCooldown();
        snake.UpdateSpeed();
    }
    public void SetFreeMovement()
    {
        // toggles between grid-based movement and free movement
        // activated by the button "GRID" on UI
        snake.FreeMovementActive = snake.FreeMovementActive ? false : true;
        if (!snake.FreeMovementActive)
        {
            float newZ = Mathf.Abs(transform.eulerAngles.z) < 90  || Mathf.Abs(transform.eulerAngles.z) > 270? 0 : 180;
            transform.rotation = new Quaternion(0, 0, newZ, 0);
            currentGrid = Grid.FindClosestGridPosition(transform.position);
            StartCoroutine(GridMovementPush());
        }
    }
}
