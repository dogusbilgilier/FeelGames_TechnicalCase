using System.Collections.Generic;
using DG.Tweening;
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
        _pixelArtAreaController.OnAllPixelsCleared += PixelArtAreaController_OnAllPixelsCleared;
        _ballController.Initialize(levelData, _pixelArtAreaController.MinZPosition);
        _ballController.OnBigBallJumpToHole += BallController_OnBigBallJumpToHole;
        IsInitialized = true;
    }

    private void PixelArtAreaController_OnAllPixelsCleared()
    {
        if (_ballController.IsCompleted)
        {
            DOVirtual.DelayedCall(0.15f, () =>
            {
                Debug.Log("WIN");
                Debug.Break();
            });
        }
    }

    private void BallController_OnBigBallJumpToHole(BigBall bigBall)
    {
        _pixelArtAreaController.OnBigBallJumpToHole(bigBall);
    }

    public PixelPiece GetNearestPixel(int colorId, Vector3 fromPos)
    {
        PixelPiece best = null;
        float bestDist = float.PositiveInfinity;

        for (int i = 0; i < _pixelArtAreaController.AllPixels.Count; i++)
        {
            var piece = _pixelArtAreaController.AllPixels[i];
            if (piece == null || piece.IsCleared || !piece.gameObject.activeInHierarchy) continue;
            if (piece.ColorID != colorId) continue;

            float d = (piece.transform.position - fromPos).sqrMagnitude;
            if (d < bestDist)
            {
                bestDist = d;
                best = piece;
            }
        }

        return best;
    }
}