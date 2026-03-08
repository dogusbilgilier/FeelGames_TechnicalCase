using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BallLane
{
    private List<BigBall> _balls;
    private int _currentIndex;
    public int TotalBallCount => _balls.Count;
    public int RemainingBallCount => TotalBallCount - 1 - _currentIndex;
    private int _laneIndex;
    public bool IsInitialized { get; private set; }
    public bool IsCompleted {  get; private set; }
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
        for (var i = 0; i < _balls.Count; i++)
        {
            var ball = _balls[i];
            ball.SetIndexInLane(i);
        }

        ArrangeLane(true);
        IsInitialized = true;
    }

    public void AddBall(BigBall ball)
    {
        _balls ??= new List<BigBall>();

        if (_balls.Contains(ball))
            return;

        _balls.Add(ball);
    }

    public void BallLeaveTheLane()
    {
        _currentIndex++;
        if (_currentIndex >= TotalBallCount)
        {
            IsCompleted = true;
        }
        else
        {
            _balls[_currentIndex].SetAsNext();
            ArrangeLane();
        }
    }

    private void ArrangeLane(bool instant = false)
    {
        float distanceBetween = GameConfigs.Instance.BallLaneYDistance;

        int index = 0;
        for (var i = _currentIndex; i < _balls.Count; i++)
        {
            BigBall bigBall = _balls[i];
            float zPos = index * distanceBetween;
            Vector3 ballPos = new Vector3(_xPosition, 0, _startZPosition - zPos);
            if (instant)
                bigBall.transform.position = ballPos;
            else
                bigBall.transform.DOMove(ballPos, 0.5f);
            index++;
        }
    }
}