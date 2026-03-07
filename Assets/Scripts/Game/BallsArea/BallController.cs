using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class BallController : MonoBehaviour
{
    [Title("Ball Area References")]
    [SerializeField] private BigBall _bigBallPrefab;
    [SerializeField] private Transform _bigBallParent;
    [SerializeField] private LinkObject _linkObjectPrefab;
    public bool IsInitialized { get; private set; }

    private readonly List<int> _colorIdInOrder = new List<int>();
    private readonly List<BigBallData> _bigBallDataList = new List<BigBallData>();
    private BallLaneController _ballLaneController;
    private float _startZPosition;

    public void Initialize(LevelData levelData, float startZPosition)
    {
        IsInitialized = false;
        _startZPosition = startZPosition;
        _colorIdInOrder.Clear();
        _bigBallDataList.Clear();

        CreateOrderedColorIdList(levelData);
        CreateBigBallDataInChunks(GameConfigs.Instance.BallChunkSize);
        //LogBigBallDataList();
        
        _ballLaneController = new BallLaneController(_bigBallDataList, _bigBallPrefab, _bigBallParent, startZPosition);
        CreateLinks();
        
        IsInitialized = true;
    }

    private void CreateLinks()
    {
        var balls = _ballLaneController.AllBigBalls;
        foreach (BigBall bigBall1 in balls)
        {
            if (bigBall1.Data.linkID == -1)
                continue;

            int linkId = bigBall1.Data.linkID;
            foreach (var bigBall2 in balls)
            {
                if (bigBall2.Data.linkID == linkId && bigBall2 != bigBall1)
                {
                    var link = Instantiate(_linkObjectPrefab, transform);
                    link.SetLink(bigBall1, bigBall2);
                }
            }
        }
    }

    #region Ordered Color ID Creation

    private void CreateOrderedColorIdList(LevelData levelData)
    {
        PixelPieceData[,] pixelDataGrid = levelData.PixelPieceDataGrid;

        int width = pixelDataGrid.GetLength(0);
        int height = pixelDataGrid.GetLength(1);

        int layerCount = (Mathf.Min(width, height) + 1) / 2;

        for (int layerIndex = 0; layerIndex < layerCount; layerIndex++)
            AddLayerColorIds(pixelDataGrid, width, height, layerIndex);
    }

    private void AddLayerColorIds(PixelPieceData[,] pixelDataGrid, int width, int height, int layerIndex)
    {
        int left = layerIndex;
        int right = width - 1 - layerIndex;
        int bottom = layerIndex;
        int top = height - 1 - layerIndex;

        for (int x = left; x <= right; x++)
        {
            AddColorId(pixelDataGrid, x, bottom);
        }

        for (int y = bottom + 1; y <= top; y++)
        {
            AddColorId(pixelDataGrid, right, y);
        }

        if (top > bottom)
        {
            for (int x = right - 1; x >= left; x--)
            {
                AddColorId(pixelDataGrid, x, top);
            }
        }

        if (left < right)
        {
            for (int y = top - 1; y > bottom; y--)
            {
                AddColorId(pixelDataGrid, left, y);
            }
        }
    }

    private void AddColorId(PixelPieceData[,] pixelDataGrid, int x, int y)
    {
        PixelPieceData cellData = pixelDataGrid[x, y];

        if (cellData == null)
            return;

        _colorIdInOrder.Add(cellData.colorID);
    }

    #endregion

    #region BigBallData Creation

    private void CreateBigBallDataInChunks(int chunkSize)
    {
        if (_colorIdInOrder.Count == 0)
            return;

        if (chunkSize <= 0)
            chunkSize = _colorIdInOrder.Count;

        Dictionary<int, int> pendingCountByColor = new Dictionary<int, int>();
        List<int> firstSeenColorOrder = new List<int>();

        int startIndex = 0;

        while (startIndex < _colorIdInOrder.Count)
        {
            int endIndex = Mathf.Min(startIndex + chunkSize, _colorIdInOrder.Count);

            List<int> uniqueColorsInChunk = new List<int>();
            HashSet<int> uniqueColorSet = new HashSet<int>();

            for (int i = startIndex; i < endIndex; i++)
            {
                int colorID = _colorIdInOrder[i];

                if (!pendingCountByColor.TryAdd(colorID, 1))
                    pendingCountByColor[colorID]++;
                else
                    firstSeenColorOrder.Add(colorID);

                if (uniqueColorSet.Add(colorID))
                    uniqueColorsInChunk.Add(colorID);
            }

            foreach (int colorID in uniqueColorsInChunk)
                TryCreateBigBallsForColor(colorID, pendingCountByColor);

            startIndex = endIndex;
        }

        FlushRemainingBigBalls(pendingCountByColor, firstSeenColorOrder);
    }

    private void TryCreateBigBallsForColor(int colorID, Dictionary<int, int> pendingCountByColor)
    {
        while (pendingCountByColor[colorID] >= GameConfigs.Instance.BallCapacityInOrder[0])
        {
            int chosenCapacity = GetRandomAvailableCapacity(pendingCountByColor[colorID]);

            if (chosenCapacity <= 0)
                break;

            pendingCountByColor[colorID] -= chosenCapacity;
            _bigBallDataList.Add(new BigBallData(colorID, chosenCapacity, -1));
        }
    }

    private int GetRandomAvailableCapacity(int currentCount)
    {
        List<int> availableCapacities = new List<int>();
        int[] capacitiesInOrder = GameConfigs.Instance.BallCapacityInOrder;

        for (int i = 0; i < capacitiesInOrder.Length; i++)
        {
            int capacity = capacitiesInOrder[i];

            if (capacity <= 0)
                continue;

            if (capacity <= currentCount)
                availableCapacities.Add(capacity);
        }

        if (availableCapacities.Count == 0)
            return -1;

        int randomIndex = UnityEngine.Random.Range(0, availableCapacities.Count);
        return availableCapacities[randomIndex];
    }

    private void FlushRemainingBigBalls(Dictionary<int, int> pendingCountByColor, List<int> firstSeenColorOrder)
    {
        for (int i = 0; i < firstSeenColorOrder.Count; i++)
        {
            int colorID = firstSeenColorOrder[i];
            TryCreateBigBallsForColor(colorID, pendingCountByColor);
        }

        for (int i = 0; i < firstSeenColorOrder.Count; i++)
        {
            int colorID = firstSeenColorOrder[i];
            int remainingCount = pendingCountByColor[colorID];

            if (remainingCount > 0)
                _bigBallDataList.Add(new BigBallData(colorID, remainingCount, -1));
        }
    }

    #endregion

    #region Debug

    [Button]
    private void LogBigBallDataList()
    {
        for (int i = 0; i < _bigBallDataList.Count; i++)
        {
            BigBallData data = _bigBallDataList[i];
            Debug.Log($"Index: {i} | ColorID: {data.colorID} | Capacity: {data.capacity}");
        }
    }

    [Button]
    private void LogOrderedColorIds()
    {
        string text = "";

        for (int i = 0; i < _colorIdInOrder.Count; i++)
        {
            text += _colorIdInOrder[i];

            if (i < _colorIdInOrder.Count - 1)
                text += ", ";
        }

        Debug.Log(text);
    }

    #endregion
}