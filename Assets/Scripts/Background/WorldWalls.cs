using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldWalls : MonoBehaviour
{
    [SerializeField] Vector3 normalDirection;
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("collide with wall");
        if (collision.collider.CompareTag("Player"))
        {
            Vector3 direction = Vector3.Reflect(collision.transform.right, normalDirection);
            collision.transform.right =direction;
            Debug.Log("collide with wall");
        }
    }

    // this is just to be safe, in case the player manages to get inside the collider
    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            collision.transform.right = normalDirection;
        }
    }
}
