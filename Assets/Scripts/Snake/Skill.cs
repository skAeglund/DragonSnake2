using UnityEngine;
using UnityEngine.UI;
using System;

public class Skill
{
    private float duration;
    private float cooldownTime;
    private float startTime;
    private float timeOfCooldownEnd;
    private Image availableUI;
    private Text hudTimer;

    public bool IsActive { get; set; } = false;
    public bool IsOnCooldown { get; set; } = true;
    public bool HasLearned { get; set; } = false;
    public float RequiredLevel { get; set; }

    public Skill (float duration, float cooldownTime, float requiredLevel = 0)
    {
        this.duration = duration;
        this.cooldownTime = cooldownTime;
        this.RequiredLevel = requiredLevel;
    }
    public void Activate()
    {
        IsActive = true;
        IsOnCooldown = true;
        startTime = Time.time;
        timeOfCooldownEnd = startTime + cooldownTime;
        if (availableUI != null)
            availableUI.color = Color.white * 0.75f;
    }
    public void Unlearn()
    {
        IsActive = false;
        HasLearned = false;
        IsOnCooldown = true;
        if (availableUI != null)
            availableUI.color = Color.clear;
    }
    public void Learn()
    {
        HasLearned = true;
        IsOnCooldown = false;
        if (availableUI != null)
            availableUI.color = Color.white;
    }
    public void EndCooldown()
    {
        if (!HasLearned) 
            return;
        IsOnCooldown = false;
        IsActive = false;
        if (availableUI != null)
        {
            availableUI.fillAmount = 1;
            availableUI.color = Color.white;
        }
    }
    public float UpdateTimeLeftOnCooldown()
    {
        float timeLeft = (float)Math.Round(timeOfCooldownEnd - Time.time, 1);
        float elapsedTime = Time.time - startTime;
        if (availableUI != null)
        {
            availableUI.fillAmount = elapsedTime / cooldownTime;
            hudTimer.text = timeLeft > 0 ? timeLeft.ToString() : "";
        }
        if (elapsedTime >= duration && IsActive)
            IsActive = false;
        return timeLeft;
    }

    // this only gets called by the player snake
    public void ConnectToHUD(GameObject HUDobject)
    {
        availableUI = HUDobject.transform.GetChild(1).GetComponent<Image>();
        hudTimer = HUDobject.GetComponentInChildren<Text>();
    }
}
