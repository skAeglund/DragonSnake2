using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class AbilityButton : MonoBehaviour
{
    Button button;
    static List<AbilityButton> abilityButtons = new List<AbilityButton>();
    [SerializeField] SkillTree skillTree;

    public Button Button { get => button; set => button = value; }

    void Start()
    {
        skillTree = GetComponentInParent<SkillTree>();
        if (skillTree == null)
            skillTree = transform.parent.GetComponentInParent<SkillTree>();
        Button = GetComponent<Button>();
        Button.onClick.AddListener(OnAllClicks);
        abilityButtons.Add(this);
        button.interactable = false;
        Button.interactable = skillTree.CheckVisibility(name);
    }
    private void CheckVisibility()
    {
        Button.interactable = skillTree.CheckVisibility(name);
    }
    private void OnEnable()
    {
        InvokeRepeating("CheckVisibility", 0, 0.5f);
    }

    public void OnAllClicks()
    {
        //Debug.Log(abilityButtons.Count);
        foreach (AbilityButton button in abilityButtons)
        {
            button.Button.interactable = skillTree.CheckVisibility(button.gameObject.name);
        }
    }
}
