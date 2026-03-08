using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class BallLaneController : IDisposable
{
    private List<BallLane> _ballLanes = new List<BallLane>();
    private List<BigBallData> _bigBallDataList;
    private BigBall _bigBallPrefab;
    private Transform _bigBallParent;
    private float _startZPosition;
    private readonly List<BigBall> _allBigBalls = new List<BigBall>();
    public List<BigBall> AllBigBalls => _allBigBalls;
    public int TotalLaneCount { get; private set; }

    public event Action<BigBall> OnBigBallJumpToHole;

    public BallLaneController(List<BigBallData> bigBallDataList, BigBall bigBallPrefab, Transform bigBallParent, float startZPosition)
    {
        _bigBallDataList = bigBallDataList;
        _bigBallPrefab = bigBallPrefab;
        _bigBallParent = bigBallParent;
        _startZPosition = startZPosition;
        CreateLanesAndLinks();
    }

    public void Dispose()
    {
        OnBigBallJumpToHole = null;
    }

    private void CreateLanesAndLinks()
    {
        TotalLaneCount = Random.Range(GameConfigs.Instance.BallLaneMinCount, GameConfigs.Instance.BallLaneMaxCount + 1);
        float distanceBetween = GameConfigs.Instance.BallLaneXDistance;
        float totalWidth = (TotalLaneCount - 1) * distanceBetween;
        float startX = -(totalWidth / 2f);

        for (int i = 0; i < TotalLaneCount; i++)
            _ballLanes.Add(new BallLane(i));

        for (int i = 0; i < _bigBallDataList.Count; i++)
        {
            BigBallData bigBallData = _bigBallDataList[i];
            TryCreateLinkData(i, bigBallData);

            int laneIndex = i % TotalLaneCount;
            BigBall ball = CreateBall(bigBallData, laneIndex);
            _ballLanes[laneIndex].AddBall(ball);
            _allBigBalls.Add(ball);
        }

        for (var i = 0; i < _ballLanes.Count; i++)
        {
            float x = startX + (i * distanceBetween);
            var ballLane = _ballLanes[i];
            ballLane.Initialize(x, _startZPosition);
        }
    }

    private BigBall CreateBall(BigBallData bigBallData, int laneIndex)
    {
        BigBall ball = Object.Instantiate(_bigBallPrefab, _bigBallParent);
        ball.Initialize(bigBallData, laneIndex);
        ball.OnJumpRequested += BigBall_OnOnJumpRequested;
        return ball;
    }

    private void BigBall_OnOnJumpRequested(BigBall bigBall)
    {
        if (CheckCanBigBallJump(bigBall, out BigBall linkedBigBall))
        {
            if (linkedBigBall != null)
            {
                OnBigBallJump(linkedBigBall);
                linkedBigBall.BreakLink();
            }

            OnBigBallJump(bigBall);
        }
    }

    private void OnBigBallJump(BigBall bigBall)
    {
        bigBall.OnDropStart();
        OnBigBallJumpToHole?.Invoke(bigBall);
        _ballLanes[bigBall.LaneIndex].BallLeaveTheLane();
    }

    private bool CheckCanBigBallJump(BigBall bigBall, out BigBall linkedBigBall)
    {
        linkedBigBall = null;
        if (bigBall.Data.linkID == -1)
            return true;

        foreach (BigBall otherBall in _allBigBalls)
        {
            if (bigBall == otherBall)
                continue;

            if (bigBall.Data.linkID == otherBall.Data.linkID)
            {
                linkedBigBall = otherBall;
                if (otherBall.LaneIndex == bigBall.LaneIndex)
                    return true;
                
                return otherBall.CanJumpToHole();
            }
        }

        return false;
    }

    private void TryCreateLinkData(int i, BigBallData bigBallData)
    {
        float linkChance = GameConfigs.Instance.BallLinkChance;
        int maxLinkDistance = GameConfigs.Instance.BallMaxLinkDistance;
        int distance = Random.Range(1, maxLinkDistance + 1);

        if (Random.Range(0f, 1f) > linkChance)
            return;

        for (int j = distance; j >= 1; j--)
        {
            if (CheckForLink(i, bigBallData, distance))
                break;
        }
    }

    private bool CheckForLink(int i, BigBallData bigBallData, int distance)
    {
        if (i + distance >= _bigBallDataList.Count)
            return false;

        BigBallData otherBallData = _bigBallDataList[i + distance];

        if (otherBallData.linkID != -1 || otherBallData.linkID == i)
            return false;

        //Lane Check---
        int firstBallLaneIndex = i % TotalLaneCount;
        int secondLaneLaneIndex = (i + distance) % TotalLaneCount;
        int laneDistance = Mathf.Abs(firstBallLaneIndex - secondLaneLaneIndex);

        //Dont Link if there is a lane between 
        if (laneDistance > 1)
            return false;

        //Dont Link if there is a ball between 
        if (laneDistance == 0 && distance > 1)
            return false;
        //----

        if (i + distance >= _bigBallDataList.Count)
            return false;

        otherBallData.linkID = i;
        bigBallData.linkID = i;
        return true;
    }
}