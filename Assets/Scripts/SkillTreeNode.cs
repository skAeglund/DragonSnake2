using System.Collections.Generic;

public class SkillTreeNode
{
    public Skill skill { get; private set; }
    private string name;
    private List<SkillTreeNode> childNodes;
    private SkillTreeNode parent;

    public SkillTreeNode(string name, Skill skill = null, SkillTreeNode parent = null)
    {
        this.name = name;
        this.skill = skill;
        if (parent != null)
        {
            this.parent = parent;
            parent.childNodes.Add(this);
        }
        childNodes = new List<SkillTreeNode>();
    }

    public bool IsVisible { get; set; } = false;
    public bool IsActivated { get; set; } = false;

    public void AddChild(SkillTreeNode child)
    {
        childNodes.Add(child);
    }
    public SkillTreeNode Find(string skillName)
    {
        SkillTreeNode node = null;
        if (name == skillName)
        {
            return this;
        }
        else if (childNodes.Count > 0)
        {
            foreach (SkillTreeNode childNode in childNodes)
            {
                node = childNode.Find(skillName);
                if (node != null)
                    break;
            }
        }
        return node;
    }

    public void UpdateSkillTree(int playerLevel)
    {
        if (childNodes.Count > 0)
        {
            foreach (SkillTreeNode node in childNodes)
            {
                if (node.skill.RequiredLevel > playerLevel)
                {
                    node.IsVisible = false;
                    node.DeActivate();
                }
                else if (playerLevel >= node.skill.RequiredLevel && !node.IsVisible)
                {
                    node.IsVisible = true;
                }
            }
        }
    }
    public void Activate()
    {
        IsActivated = true;
        if (skill != null)
        {
            skill.Learn();
        }
        if (childNodes.Count >0)
        {
            foreach (SkillTreeNode node in childNodes)
            {
                node.IsVisible = true;
            }
        }
    }
    public void DeActivate()
    {
        IsActivated = false;
        if (skill != null)
        {
            skill.Unlearn();
        }
        if (childNodes.Count > 0)
        {
            foreach (SkillTreeNode node in childNodes)
            {
                node.IsVisible = false;
                node.IsActivated = false;
                if (node.childNodes.Count > 0)
                {
                    node.DeActivate();
                }
            }
        }
    }
}
