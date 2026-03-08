using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class GameplayController : MonoBehaviour
{
    [Title("References")]
    [SerializeField] private PixelArtAreaController _pixelArtAreaController;
    [SerializeField] private BallController _ballController;
    public List<Color> LevelColorsInOrder;
    public bool IsInitialized { get; private set; }

    public void Initialize()
    {
        LevelData levelData = LevelDataCreator.Instance.CreateLevelData();
        LevelColorsInOrder = levelData.LevelColorsInOrder;

        Debug.Assert(levelData != null, "LevelData creation failed");

        _pixelArtAreaController.Initialize(levelData);
        _ballController.Initialize(levelData, _pixelArtAreaController.MinZPosition);
        _ballController.OnBigBallJumpToHole += BallController_OnBigBallJumpToHole;
        IsInitialized = true;
    }

    private void BallController_OnBigBallJumpToHole(BigBall bigBall)
    {
        _pixelArtAreaController.OnBigBallJumpToHole(bigBall);
    }
}