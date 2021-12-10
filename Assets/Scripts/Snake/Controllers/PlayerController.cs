using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;

public class PlayerController : SnakeManager
{
    private PlayerInput playerInput;
    private SmoothCamera cameraScript;

    public SnakeAbilities Abilities { get => abilities; set => abilities = value; }
    public Movement Movement { get => movement; }


    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        movement = GetComponent<Movement>();
        snakeBody = GetComponent<SnakeBody>();
        abilities = GetComponent<SnakeAbilities>();
        cameraScript = Camera.main.GetComponent<SmoothCamera>();
        snakeList = snakeBody.SnakeList;

        snakeBody.resizeSnakeEvent += cameraScript.UpdateDistance;
        snakeBody.resizeSnakeEvent += movement.UpdateSpeed;
        snakeBody.resizeSnakeEvent += HUD.UpdateScore;
    }
    private void Start()
    {
        movement.UpdateSpeed(snakeList.Count);
        HUDComponents.ConnectSkillsToHud(abilities);
    }

    void Update()
    {
        if (movement.StunnedState || abilities.StoneState.IsActive) return;

        playerInput.CheckInputs(out rotationDirection, out pendingAbility, out slowState);
        movement.Rotate(rotationDirection, slowState);
        movement.Move(slowState);
        snakeBody.DrawSnakeBody();

        if (pendingAbility != Ability.Null)
            abilities.UseAbility(pendingAbility);

        if (snakeList.Count > 1)
            snakeBody.MoveBodyParts(movement.Speed);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Food") && !snakeBody.IsEatingBomb)
        {
            snakeBody.SpawnBodyPart(other.name.Substring(0, 4) == "Blue" ? true : false);
            SpawnManager.Instance.ReturnFoodToPool(other.gameObject);
        }
        else if (other.CompareTag("BodyPart"))
        {
            if (snakeList.Contains(other.gameObject))
                return;

            GameObject head = other.transform.parent.parent.Find("Head").gameObject;
            head.TryGetComponent(out SnakeAbilities otherAbilities);
            if (abilities.Whirlwind.IsActive)
            {
                if (otherAbilities != null)
                {
                    if (otherAbilities.StoneState.IsActive)
                    {
                        StartCoroutine(abilities.StunRoutine(2));
                        return;
                    }
                }
                other.GetComponent<BodyPart>().ConvertIntoFood();
                abilities.DeActivateWhirlWind(0.33f);
                StartCoroutine(abilities.ImmuneRoutine(1));
            }
            else if (!movement.StunnedState && !abilities.StunImmuneState)
            {
                // colliding with stunned player/NPC doesn't stun you
                if (otherAbilities != null) if (otherAbilities.StunnedState)
                        return;
                StartCoroutine(abilities.StunRoutine(1));
            }
        }
        else if (other.CompareTag("SnakeHead") && !abilities.Whirlwind.IsActive)
        {
            if (!other.transform.IsChildOf(transform))
                StartCoroutine(abilities.StunRoutine(1));
        }
    }
}
