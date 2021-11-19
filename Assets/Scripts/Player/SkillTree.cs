using UnityEngine;

/// <summary>
/// Contains the skills usable by the player
/// NOTE: this isn't meant to part of the actual game and was only made for the sake of the assignment
/// therefore, some things are here just to show how the system works (and is not actually implemented in the game)
/// </summary>
public class SkillTree : MonoBehaviour
{
    [SerializeField] private PlayerSnake player;
    public SkillTreeNode skillTree = new SkillTreeNode("Root");

    private void Awake()
    {
        AddSkillsToTree();
        UpdateSkillTree(player.SnakeList.Count);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            bool activeState = transform.GetChild(0).gameObject.activeSelf;
            transform.GetChild(0).gameObject.SetActive(activeState != true);
        }
        if (Input.GetKeyDown(KeyCode.Tab)) // cheatcode
        {
            DisableLevelSystem();
        }
    }
    protected void AddSkillsToTree()
    {
        SkillTreeNode dashNode = new SkillTreeNode("Dash", player.Dash, skillTree);
        SkillTreeNode wirlwindNode = new SkillTreeNode("Whirlwind", player.Whirlwind, skillTree);
        SkillTreeNode stoneStateNode = new SkillTreeNode("StoneState", player.StoneState, skillTree);
        SkillTreeNode fireBallNode = new SkillTreeNode("Fireball", player.Fireball, skillTree);

        // this is only to show that the skilltree system works, the actual skills below have not been implemented
        Skill BiggerFireBall = new Skill(0, 3, 20);
        new SkillTreeNode("BiggerFireball", BiggerFireBall, fireBallNode);
        Skill IceBall = new Skill(1, 5, 15);
        new SkillTreeNode("IceBall", IceBall, fireBallNode);
        Skill FasterWhirlwind = new Skill(1, 5, 15);
        new SkillTreeNode("FasterWhirlwind", FasterWhirlwind, wirlwindNode);
        Skill WhirlwindDuration = new Skill(1, 5, 15);
        new SkillTreeNode("WhirlwindDuration", WhirlwindDuration, wirlwindNode);

        skillTree.Activate();
    }

    public void UpdateSkillTree(int playerLevel)
    {
        skillTree.UpdateSkillTree(playerLevel);
    }
    public bool CheckVisibility(string skillName)
    {
        SkillTreeNode node = skillTree.Find(skillName);
        if (node == null) Debug.Log(skillName + " is null");
        return node.IsVisible;
    }
    public bool IsActivated(string skillName)
    {
        SkillTreeNode node = skillTree.Find(skillName);
        return node.IsActivated;
    }
    public void Activate(string skillName)
    {
        skillTree.Find(skillName)?.Activate();
    }
    public void DeActivate(string skillName)
    {
        skillTree.Find(skillName)?.Activate();
    }
    private void DisableLevelSystem()
    {
        player.Fireball.RequiredLevel = 0;
        player.Whirlwind.RequiredLevel = 0;
        player.StoneState.RequiredLevel = 0;
        player.Dash.RequiredLevel = 0;
        player.Fireball.Learn();
        player.Whirlwind.Learn();
        player.StoneState.Learn();
        player.Dash.Learn();
    }
}
