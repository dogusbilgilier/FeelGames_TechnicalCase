using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "GameConfigs", menuName = "New GameConfigs", order = 0)]
public class GameConfigs : ScriptableObject
{
    private static GameConfigs s_Instance;
    public static GameConfigs Instance => s_Instance;

    public void Initialize()
    {
        Debug.Assert(s_Instance == null, "GameConfigs Instance already exist");
        s_Instance = this;
    }
    
    [Button(ButtonSizes.Gigantic, ButtonStyle.FoldoutButton)]
    [FoldoutGroup("GENERAL", Expanded = false)]
    [TitleGroup("GENERAL/Settings", alignment: TitleAlignments.Centered)]
    public int MaxBallCountInPixelArea = 50;
    
    [Button(ButtonSizes.Gigantic, ButtonStyle.FoldoutButton)]
    [FoldoutGroup("PIXEL AREA", Expanded = false)]
    [TitleGroup("PIXEL AREA/Settings", alignment: TitleAlignments.Centered)]
    public float PixelSize = 0.25f;
    [TitleGroup("PIXEL AREA/Settings", alignment: TitleAlignments.Centered)]
    public float HoleRadius = 3f;
    [TitleGroup("PIXEL AREA/Border", alignment: TitleAlignments.Centered)]
    public float BorderOffset = 2.5f;
    [TitleGroup("PIXEL AREA/Border", alignment: TitleAlignments.Centered)]
    public float BorderCornerRadius = 0.5f;
    [TitleGroup("PIXEL AREA/Border", alignment: TitleAlignments.Centered)]
    public int BorderArcSegments = 20;
    [TitleGroup("PIXEL AREA/Border", alignment: TitleAlignments.Centered)]
    public float BorderLineStep = 0.25f;
    [TitleGroup("PIXEL AREA/Border", alignment: TitleAlignments.Centered)]
    public float BorderThickness = 0.3f;

    [FoldoutGroup("BALL AREA", Expanded = false)]
    [TitleGroup("BALL AREA/General", alignment: TitleAlignments.Centered)]
    public int[] BallCapacityInOrder = { 10, 20 };
    [TitleGroup("BALL AREA/General", alignment: TitleAlignments.Centered)]
    public int BallChunkSize = 50;
    [TitleGroup("BALL AREA/Lane", alignment: TitleAlignments.Centered)]
    public int BallLaneMinCount = 3;
    [TitleGroup("BALL AREA/Lane", alignment: TitleAlignments.Centered)]
    public int BallLaneMaxCount = 6;
    [TitleGroup("BALL AREA/Lane", alignment: TitleAlignments.Centered)]
    public float BallLaneXDistance = 1.5f;
    [TitleGroup("BALL AREA/Lane", alignment: TitleAlignments.Centered)]
    public float BallLaneYDistance = 1f;
    [TitleGroup("BALL AREA/Link", alignment: TitleAlignments.Centered)]
    public float BallLinkChance = 0.15f;
    [TitleGroup("BALL AREA/Link", alignment: TitleAlignments.Centered)]
    public int BallMaxLinkDistance = 2;

    [FoldoutGroup("SMALL BALL", Expanded = false)]
    [TitleGroup("SMALL BALL/General", alignment: TitleAlignments.Centered)]
    public float SmallBallSpeed = 10;
    [TitleGroup("SMALL BALL/General", alignment: TitleAlignments.Centered)]
    public float SmallBallScaleMultiplier = 0.75f;

    [FoldoutGroup("TWEENS", Expanded = false)]
    [TitleGroup("TWEENS/BigBallToHole", alignment: TitleAlignments.Centered)]
    public float BigBallToHoleJumpDuration = 0.2f;
    [TitleGroup("TWEENS/BigBallToHole", alignment: TitleAlignments.Centered)]
    public float BigLinkedBallToHoleDelay = 0.1f;
   
    [FoldoutGroup("BALL BOUNCE", Expanded = false)]
    [TitleGroup("BALL BOUNCE/Settings", alignment: TitleAlignments.Centered)]
    public float Skin = 0.005f;
    [TitleGroup("BALL BOUNCE/Settings", alignment: TitleAlignments.Centered)]
    public int MaxHitsPerFrame = 6;
    [TitleGroup("BALL BOUNCE/Settings", alignment: TitleAlignments.Centered)]
    public float RetargetInterval = 0.25f; 
    [TitleGroup("BALL BOUNCE/Settings", alignment: TitleAlignments.Centered)]
    public float PostBounceMaxDeg = 12f; 
}