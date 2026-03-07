using System.Collections.Generic;
using UnityEngine;

public class BallLaneController
{
    private List<BallLane> _ballLanes = new List<BallLane>();
    private List<BigBallData> _bigBallDataList;
    private BigBall _bigBallPrefab;
    private Transform _bigBallParent;
    private float _startZPosition;
    public int TotalLaneCount { get; private set; }

    public BallLaneController(List<BigBallData> bigBallDataList, BigBall bigBallPrefab, Transform bigBallParent, float startZPosition)
    {
        _bigBallDataList = bigBallDataList;
        _bigBallPrefab = bigBallPrefab;
        _bigBallParent = bigBallParent;
        _startZPosition = startZPosition;
        CreateLanesAndLinks();
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
            TryCreateLink(i, bigBallData);

            int laneIndex = i % TotalLaneCount;
            BigBall ball = CreateBall(bigBallData);

            _ballLanes[laneIndex].AddBall(ball);
        }

        for (var i = 0; i < _ballLanes.Count; i++)
        {
            float x = startX + (i * distanceBetween);
            var ballLane = _ballLanes[i];
            ballLane.Initialize(x, _startZPosition);
        }
    }

    private BigBall CreateBall(BigBallData bigBallData)
    {
        BigBall ball = Object.Instantiate(_bigBallPrefab, _bigBallParent);
        ball.Initialize(bigBallData);
        return ball;
    }

    private void TryCreateLink(int i, BigBallData bigBallData)
    {
        float linkChance = GameConfigs.Instance.BallLinkChance;
        int maxLinkDistance = TotalLaneCount * 2 - 1;
        if (Random.Range(0f, 1f) >= linkChance)
        {
            int distance = Random.Range(1, maxLinkDistance);
            if (i + distance < _bigBallDataList.Count)
            {
                BigBallData otherBallData = _bigBallDataList[i + distance];
                if (otherBallData.connectedGroupId == -1)
                {
                    otherBallData.connectedGroupId = i;
                    bigBallData.connectedGroupId = i;
                }
            }
        }
    }
}