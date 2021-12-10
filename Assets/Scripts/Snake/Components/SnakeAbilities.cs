using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Enums;

public class SnakeAbilities : MonoBehaviour
{
    private BoxCollider boxCollider;         // regular collider
    private CapsuleCollider capsuleCollider; // collider during whirlwind skill
    private LineRenderer snakeLine;
    private SpriteRenderer spriteRenderer;
    Movement movement;
    SnakeBody snakeBody;

    [SerializeField] private GameObject whirlWindObject;
    [SerializeField] private GameObject stunCircles;

    public UnityEvent OnDashStart; 

    public Skill Fireball = new Skill(0, 3, 15);
    public Skill Whirlwind = new Skill(1, 5, 10);
    public Skill StoneState = new Skill(0.5f, 2, 12);
    public Skill Dash = new Skill(1, 1, 5);

    public bool StunImmuneState { get; protected set; } = false;
    public bool StunnedState { get => movement.StunnedState; }

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        snakeLine = GetComponent<LineRenderer>();
        spriteRenderer = transform.Find("Sprite")?.GetComponent<SpriteRenderer>();
        movement = GetComponent<Movement>();
        snakeBody = GetComponent<SnakeBody>();
    }
    public void UseAbility(Ability ability)
    {
        if (StunnedState || ability == Ability.Null) return;

        switch(ability)
        {
            case Ability.Fireball:
                if (!Fireball.IsOnCooldown && !StoneState.IsActive)
                    StartCoroutine(FireballRoutine());
                break;
            case Ability.Whirlwind:
                if (!Whirlwind.IsOnCooldown && !StoneState.IsActive)
                    StartCoroutine(WhirlwindRoutine());
                break;
            case Ability.StoneState:
                if (!StoneState.IsOnCooldown)
                    StartCoroutine(StoneStateRoutine());
                break;
            case Ability.Dash:
                if (!Dash.IsOnCooldown && !StoneState.IsActive)
                    StartCoroutine(DashRoutine());
                break;
        }
    }
    #region Skill Routines
    public IEnumerator StoneStateRoutine()
    {
        Material originalMaterial = snakeLine.material;
        Material originalHeadMaterial = spriteRenderer.material;

        snakeLine.material = Materials.StoneLine;
        spriteRenderer.material = Materials.StoneHead;

        StoneState.Activate();
        while (StoneState.IsActive)
        {
            StoneState.UpdateTimeLeftOnCooldown();
            yield return new WaitForEndOfFrame();
        }
        snakeLine.material = originalMaterial;
        spriteRenderer.material = originalHeadMaterial;
        while (StoneState.UpdateTimeLeftOnCooldown() > 0)
        {
            yield return new WaitForEndOfFrame();
        }
        StoneState.EndCooldown();
        StartCoroutine(ImmuneRoutine(0.25f));
    }

    public IEnumerator WhirlwindRoutine()
    {
        StunImmuneState = false;
        ActivateWhirlWind();
        CreateSkillLight();
        while (Whirlwind.IsActive)
        {
            Whirlwind.UpdateTimeLeftOnCooldown();
            yield return new WaitForEndOfFrame();
        }
        if (capsuleCollider.enabled == true) // checks if the whirlwind has been canceled by a collision already
            DeActivateWhirlWind();
        while (Whirlwind.UpdateTimeLeftOnCooldown() > 0)
        {
            yield return new WaitForEndOfFrame();
        }
        Whirlwind.EndCooldown();
    }
    public IEnumerator FireballRoutine()
    {
        Fireball.Activate();
        CreateSkillLight();

        GameObject fireball = Instantiate(Prefabs.Fireball, transform.position, Quaternion.identity, transform.parent);
        fireball.GetComponent<Fireball>().SetOwnerAndDirection(transform.right, gameObject);
        float timeLeftOnCooldown = Fireball.UpdateTimeLeftOnCooldown();
        while (timeLeftOnCooldown > 0)
        {
            timeLeftOnCooldown = Fireball.UpdateTimeLeftOnCooldown();
            yield return new WaitForEndOfFrame();
        }
        Fireball.EndCooldown();
    }
    private IEnumerator DashRoutine()
    {
        
        CreateSkillLight();
        Dash.Activate();
        OnDashStart.Invoke();
        float startTime = Time.time;
        float duration = Mathf.Clamp(0.5f - (snakeBody.SnakeList.Count * 0.01f), 0.3f, 0.5f);
        float startSpeed = movement.Speed;
        float startRotationSpeed = movement.RotationSpeed;

        while (Time.time - startTime < duration)
        {
            float elapsed = Time.time - startTime;
            float t = elapsed / duration;
            movement.Speed = Mathf.Lerp(startSpeed, startSpeed * 4, t);
            movement.RotationSpeed = (Mathf.Lerp(startRotationSpeed, startRotationSpeed * 2, t));
            Dash.UpdateTimeLeftOnCooldown();
            yield return new WaitForEndOfFrame();
        }
        float topSpeed = movement.Speed;
        //yield return new WaitForSeconds(duration);
        while //(Time.time - startTime - (duration/**2*/) < duration)
            (Dash.IsActive)
        {
            float elapsed = Time.time - startTime - duration/**2*/;
            float t = elapsed / (duration /**2*/);
            movement.Speed = Mathf.SmoothStep(topSpeed, startSpeed, t);
            movement.RotationSpeed = (Mathf.SmoothStep(startRotationSpeed * 2, startRotationSpeed, t));
            Dash.UpdateTimeLeftOnCooldown();
            yield return new WaitForEndOfFrame();
        }
        Dash.UpdateTimeLeftOnCooldown();
        Dash.EndCooldown();
        movement.UpdateSpeed(snakeBody.SnakeList.Count);
    }
    #endregion

    public IEnumerator ImmuneRoutine(float immuneTime)
    {
        StunImmuneState = true;
        yield return new WaitForSeconds(immuneTime);
        StunImmuneState = false;
    }
    private void ActivateWhirlWind()
    {
        boxCollider.enabled = false;
        capsuleCollider.enabled = true;
        whirlWindObject.SetActive(true);
        Whirlwind.Activate();
    }
    public void DeActivateWhirlWind(float delay = 0)
    {
        if (whirlWindObject == null) return;

        boxCollider.enabled = true;
        capsuleCollider.enabled = false;
        Whirlwind.IsActive = false;
        if (delay > 0)
            StartCoroutine(DelayedWhirlwindEnd(delay));
        else
            whirlWindObject.SetActive(false);
    }
    private IEnumerator DelayedWhirlwindEnd(float delay)
    {
        // Lets the whirlwind animation linger for a while, in case of collisions
        yield return new WaitForSeconds(delay);
        whirlWindObject.SetActive(false);
    }

    public void CreateSkillLight()
    {
        Instantiate(Prefabs.SkillLight, transform.position + new Vector3(-2, -2, -3), Quaternion.identity, transform.parent);
    }
    public IEnumerator StunRoutine(float stunTime)
    {
        if (StunImmuneState || movement.StunnedState || StoneState.IsActive)
            yield break;
        movement.StunnedState = true;
        stunCircles.SetActive(true);
        DeActivateWhirlWind();
        yield return new WaitForSeconds(stunTime);
        movement.StunnedState = false;
        stunCircles.SetActive(false);
        StartCoroutine(ImmuneRoutine(1));
    }
}
