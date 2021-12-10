using UnityEngine;

public class HUDComponents : MonoBehaviour
{
    [SerializeField] private GameObject _fireballUI;
    [SerializeField] private GameObject _whirlWindUI;
    [SerializeField] private GameObject _stoneSkillUI;
    [SerializeField] private GameObject _dashUI;

    public static GameObject fireballUI   { get; private set; }
    public static GameObject whirlWindUI  { get; private set; }
    public static GameObject stoneSkillUI { get; private set; }
    public static GameObject dashUI       { get; private set; }

    void Start()
    {
        fireballUI = _fireballUI;
        whirlWindUI = _whirlWindUI;
        stoneSkillUI = _stoneSkillUI;
        dashUI = _dashUI;
    }
    public static void ConnectSkillsToHud(SnakeAbilities abilities)
    {
        abilities.Fireball.ConnectToHUD(fireballUI);
        abilities.Whirlwind.ConnectToHUD(whirlWindUI);
        abilities.StoneState.ConnectToHUD(stoneSkillUI);
        abilities.Dash.ConnectToHUD(dashUI);
    }
}
