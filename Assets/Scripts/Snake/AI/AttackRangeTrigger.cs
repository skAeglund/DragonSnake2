using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class AttackRangeTrigger : MonoBehaviour
{
    public UnityEvent insideAttackRangeEvent;
    public UnityEvent outsideAttackRangeEvent;

    private void Awake()
    {
        if (insideAttackRangeEvent == null)
        {
            AttackingAIController snake = transform.root.GetComponentInChildren<AttackingAIController>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BodyPart"))
        {
            if (other.transform.root.CompareTag("Player"))
            {
                insideAttackRangeEvent?.Invoke();
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("BodyPart"))
        {
            if (other.transform.root.CompareTag("Player"))
            {
               outsideAttackRangeEvent.Invoke();
            }
        }
    }
}
