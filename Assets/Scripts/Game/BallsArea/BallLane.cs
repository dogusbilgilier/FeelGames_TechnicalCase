using System.Collections.Generic;
using UnityEngine;

public class BallLane
{
    private List<BigBall> _balls;
    private int _currentIndex;
    public int TotalBallCount => _balls.Count;
    public int RemainingBallCount => TotalBallCount - 1 - _currentIndex;
    private int _laneIndex;
    public bool IsInitialized { get; private set; }

    private float _startZPosition;

    public BallLane(int laneIndex)
    {
        _laneIndex = laneIndex;
        _currentIndex = 0;
    }

    private float _xPosition;

    public void Initialize(float xPosition, float startZPosition)
    {
        _xPosition = xPosition;
        _balls[_currentIndex].SetAsNext();
        _startZPosition = startZPosition;
        ArrangeLane();
        IsInitialized = true;
    }

    public void AddBall(BigBall ball)
    {
        _balls ??= new List<BigBall>();

        if (_balls.Contains(ball))
            return;

        _balls.Add(ball);
    }

    public bool TryGetCurrentBall(out BigBall ball)
    {
        ball = null;

        if (RemainingBallCount > 0)
            ball = _balls[_currentIndex];

        return ball != null;
    }

    public void BallLeaveTheLane()
    {
        _currentIndex++;
        ArrangeLane();
    }

    private void ArrangeLane()
    {
        float distanceBetween = GameConfigs.Instance.BallLaneYDistance;

        for (var i = _currentIndex; i < _balls.Count; i++)
        {
            BigBall bigBall = _balls[i];
            float zPos = i * distanceBetween;
            bigBall.transform.position = new Vector3(_xPosition, 0, _startZPosition - zPos);
        }
    }
}