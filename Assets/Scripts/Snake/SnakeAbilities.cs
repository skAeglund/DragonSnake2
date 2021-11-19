using System.Collections;
using UnityEngine;

public abstract class SnakeAbilities : MonoBehaviour
{
    [SerializeField] protected GameObject whirlWindObject;
    [SerializeField] protected GameObject stunCircles;
    [SerializeField] protected SkillTree skillTree;
    protected BoxCollider boxCollider;         // regular collider
    protected CapsuleCollider capsuleCollider; // collider during whirlwind skill

    public Skill Fireball = new Skill(0, 3, 15);
    public Skill Whirlwind = new Skill( 1, 5, 10);
    public Skill StoneState = new Skill( 0.5f, 2, 12);
    public Skill Dash = new Skill(1, 1, 5);

    public bool FreeMovementActive { get; set; } = true;
    public bool StunImmuneState { get; protected set; } = false;


    #region Skill Routines
    public IEnumerator StoneStateRoutine(LineRenderer snakeLine, SpriteRenderer spriteRenderer)
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
    public IEnumerator FireballRoutine(Vector3 direction)
    {
        Fireball.Activate();
        CreateSkillLight();
        GameObject prefab;
        if (skillTree != null)
             prefab = skillTree.IsActivated("BiggerFireball") ? Prefabs.BigFireball : Prefabs.Fireball;
        else
             prefab = Prefabs.Fireball;


        GameObject fireball = Instantiate(prefab, transform.position, Quaternion.identity,transform.parent);
        fireball.GetComponent<Fireball>().SetOwnerAndDirection(direction, gameObject);
        float timeLeftOnCooldown = Fireball.UpdateTimeLeftOnCooldown();
        while (timeLeftOnCooldown > 0)
        {
            timeLeftOnCooldown = Fireball.UpdateTimeLeftOnCooldown();
            yield return new WaitForEndOfFrame();
        }
        Fireball.EndCooldown();
    }
    #endregion
    protected IEnumerator ImmuneRoutine(float immuneTime)
    {
        StunImmuneState = true;
        yield return new WaitForSeconds(immuneTime);
        StunImmuneState = false;
    }
    protected void ActivateWhirlWind()
    {
        boxCollider.enabled = false;
        capsuleCollider.enabled = true;
        whirlWindObject.SetActive(true);
        Whirlwind.Activate();
    }
    protected void DeActivateWhirlWind(float delay = 0)
    {
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

}
