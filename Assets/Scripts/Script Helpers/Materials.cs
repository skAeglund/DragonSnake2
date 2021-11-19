using UnityEngine;

public class Materials : MonoBehaviour
{
    [SerializeField] private Material stoneLine;
    [SerializeField] private Material stoneHead;
    [SerializeField] private Material originalLine;
    [SerializeField] private Material originalHead;
    [SerializeField] private Material fireTest;

    public static Material StoneLine    { get; private set; }
    public static Material StoneHead    { get; private set; }
    public static Material OriginalLine { get; private set; }
    public static Material OriginalHead { get; private set; }
    public static Material FireTest     { get; private set; }

    void Awake()
    {
        StoneLine = stoneLine;
        StoneHead = stoneHead;
        OriginalLine = originalLine;
        OriginalHead = originalHead;
        FireTest = fireTest;
    }
}
