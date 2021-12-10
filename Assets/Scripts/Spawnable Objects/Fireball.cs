using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    Animator animator;
    public Vector3 direction { get; set; }
    public GameObject owner { get; set; }
    private SphereCollider sphereCollider;
    private float speed = 50;
    private float lifeTime = 10;

    void Start()
    {
        animator = GetComponent<Animator>();
        sphereCollider = GetComponent<SphereCollider>();
        if (direction != null)
            transform.up = direction;

        StartCoroutine(DeathByTime(lifeTime));
        //if (transform.parent.name.Substring(0, 4) == "Blue")
        //{
        //    speed *= 0.7f;
        //    speed += transform.parent.GetComponentInChildren<SnakeBody>().SnakeList.Count * 0.2f;
        //}
        //else
        speed += transform.parent.GetComponentInChildren<SnakeBody>().SnakeList.Count *0.5f;
    }
    private void Update()
    {
        transform.position += transform.up * Time.deltaTime * speed;
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Complete"))
            Destroy(gameObject);
    }

    // This method needs to be called when instantiating, to get the correct direction
    // and for the shooter to not kill themselves
    public void SetOwnerAndDirection(Vector3 direction, GameObject isInstantiatedBy) 
    {
        this.direction = direction;
        owner = isInstantiatedBy;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (owner == null || other.gameObject == null || other.transform.parent == null)
            return;
        else if (other.gameObject.Equals(owner) || other.transform.IsChildOf(owner.transform) || other.transform.parent.IsChildOf(owner.transform) || other.gameObject.transform.root.Equals(owner))
            return;
        if (other.CompareTag("BodyPart") || other.CompareTag("SnakeHead"))
        {
            sphereCollider.enabled = false;
            animator.SetTrigger("DidCollide");
            speed = 0;
            StartCoroutine(FadeOut());
            
            if (other.CompareTag("SnakeHead"))
            {
                if (other.transform.parent.TryGetComponent(out SnakeAbilities otherAbilities))
                {
                    if (!otherAbilities.StoneState.IsActive)
                        other.transform.parent.gameObject.SendMessage("CompleteConversionToFood");
                }
                else // split snake doesn't have abilities
                {
                    other.transform.parent.gameObject.SendMessage("CompleteConversionToFood");
                }
            }
        }
    }

    private IEnumerator FadeOut()
    {
        Light light = GetComponentInChildren<Light>();
        float duration = 0.4f;
        float startTime = Time.time;
        float startIntensity = light.intensity;
        while (Time.time-startTime < duration)
        {
            float t = (Time.time - startTime) / duration;
            light.intensity = Mathf.Lerp(startIntensity, 0, t);
            yield return null;
        }
    }
    private IEnumerator DeathByTime(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }
}
