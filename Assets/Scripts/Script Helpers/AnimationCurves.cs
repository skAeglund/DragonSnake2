using UnityEngine;

public class AnimationCurves : MonoBehaviour
{
    [SerializeField] private AnimationCurve smoothJump;
    [SerializeField] private AnimationCurve easeInOut;
    [SerializeField] private AnimationCurve easeInOut2;
    [SerializeField] private AnimationCurve arms;
    [SerializeField] private AnimationCurve snakeSize;
    [SerializeField] private AnimationCurve gridSnakeSize;
    public AnimationCurve Cloud1;
    public AnimationCurve Cloud2;
    public AnimationCurve Cloud3;
    public AnimationCurve Cloud4;

    public static AnimationCurve SmoothJump     { get; private set; }
    public static AnimationCurve[] CloudCycles  { get; private set; }
    public static AnimationCurve SnakeSize      { get; private set; }
    public static AnimationCurve GridSnakeSize  { get; private set; }
    public static AnimationCurve EaseInOut      { get; private set; }
    public static AnimationCurve EaseInOut2     { get; private set; }
    public static AnimationCurve Arms           { get; private set; }

    private void Awake()
    {
        CloudCycles = new AnimationCurve[] { Cloud1, Cloud2, Cloud3, Cloud4 };
        SnakeSize = snakeSize;
        GridSnakeSize = gridSnakeSize;
        EaseInOut = easeInOut;
        EaseInOut2 = easeInOut2;
        SmoothJump = smoothJump;
        Arms = arms;
    }

}
