using Enums;

public interface IState
{
    public void Start();
    public void Execute(out RotationDirection rD, out Ability pendingAbility, out bool slowState);
    public void Exit();
}
