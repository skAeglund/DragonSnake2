using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    SphereCollider sphereCollider;
    float immunityTime = 0.1f;

    void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.enabled = false;
        StartCoroutine(DelayedCollider(immunityTime));
    }

    private IEnumerator DelayedCollider(float secondsOfDelay)
    {
        yield return new WaitForSeconds(secondsOfDelay);
        sphereCollider.enabled = true;
    }

}
