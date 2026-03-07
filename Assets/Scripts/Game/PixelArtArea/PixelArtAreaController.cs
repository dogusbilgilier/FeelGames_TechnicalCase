using System.Collections.Generic;
using Dreamteck.Splines;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

public class PixelArtAreaController : MonoBehaviour
{
    [Title("References")]
    [SerializeField] private PixelPiece _pixelPiecePrefab;
    [SerializeField] private Transform _pixelPieceParent;
    [SerializeField] private SplineComputer _borderSpline;
    [SerializeField] private HoleObject _holeObjectPrefab;

    public float MinZPosition { get; private set; }
    public bool IsInitialized { get; private set; }

    public void Initialize(LevelData levelData)
    {
        CreatePixelArt(levelData);
        IsInitialized = true;
    }

    private void CreateHoleObject(Vector3 position)
    {
        var hole = Instantiate(_holeObjectPrefab, transform);
        hole.transform.position = position-Vector3.forward*0.75f;
        float holeScale = GameConfigs.Instance.HoleRadius;
        hole.transform.localScale = new Vector3(holeScale, 0.2f, holeScale);
        MinZPosition = hole.transform.position.z - holeScale;
    }

    private void CreatePixelArt(LevelData levelData)
    {
        PixelPieceData[,] pixelPieceDataGrid = levelData.PixelPieceDataGrid;

        if (pixelPieceDataGrid == null)
            return;

        int width = pixelPieceDataGrid.GetLength(0);
        int height = pixelPieceDataGrid.GetLength(1);

        float size = GameConfigs.Instance.PixelSize;
        Vector3 centerOffset = new Vector3((width - 1) * size, 0f, (height - 1) * size) * 0.5f;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                PixelPieceData pieceData = pixelPieceDataGrid[x, y];

                if (pieceData == null)
                    continue;

                Color pixelColor = pieceData.color;
                Vector3 pos = new Vector3(x, size * 0.5f, y) * size - centerOffset;

                PixelPiece piece = Instantiate(_pixelPiecePrefab, _pixelPieceParent);
                piece.transform.localScale = Vector3.one * size;
                piece.transform.localPosition = pos;
                piece.SetData(pieceData, pixelColor);
            }
        }


        float totalWidth = (width * size) + GameConfigs.Instance.BorderOffset;
        float totalHeight = (height * size) + GameConfigs.Instance.BorderOffset;
        Vector3 center = transform.position;
        CreateBorderSpline(totalWidth, totalHeight, center);
    }

    private void CreateBorderSpline(float width, float height, Vector3 center)
    {
        SplineComputer spline = _borderSpline;

        float halfWidth = width * 0.5f;
        float halfHeight = height * 0.5f;
        int arcSegments = GameConfigs.Instance.BorderArcSegments;
        float holeRadius = GameConfigs.Instance.HoleRadius;

        float radius = Mathf.Clamp(GameConfigs.Instance.BorderCornerRadius, 0f, Mathf.Min(halfWidth, halfHeight));

        Vector3 start = new Vector3(-(holeRadius * 0.5f), 0f, -halfHeight + center.z);
        Vector3 end = new Vector3((holeRadius * 0.5f), 0f, -halfHeight + center.z);

        Vector3 blCenter = new Vector3(-halfWidth + radius, center.y, center.z - halfHeight + radius); // bottom-left
        Vector3 tlCenter = new Vector3(-halfWidth + radius, center.y, center.z + halfHeight - radius); // top-left
        Vector3 trCenter = new Vector3(halfWidth - radius, center.y, center.z + halfHeight - radius); // top-right
        Vector3 brCenter = new Vector3(halfWidth - radius, center.y, center.z - halfHeight + radius); // bottom-right

        List<Vector3> points = new List<Vector3> { start };

        AddLineSampled(points,
            start,
            new Vector3(-halfWidth + radius, 0f, center.z - halfHeight),
            GameConfigs.Instance.BorderLineStep);

        AddArc(points, blCenter, radius, 180f, 270f, GameConfigs.Instance.BorderArcSegments);

        AddLineSampled(points,
            new Vector3(-halfWidth, 0f, center.z - halfHeight + radius),
            new Vector3(-halfWidth, 0f, center.z + halfHeight - radius),
            GameConfigs.Instance.BorderLineStep);

        AddArc(points, tlCenter, radius, 90f, 180f, arcSegments);

        AddLineSampled(points,
            new Vector3(-halfWidth + radius, 0f, center.z + halfHeight),
            new Vector3(halfWidth - radius, 0f, center.z + halfHeight),
            GameConfigs.Instance.BorderLineStep);

        AddArc(points, trCenter, radius, 0f, 90f, arcSegments);

        AddLineSampled(points,
            new Vector3(halfWidth, 0f, center.z + halfHeight - radius),
            new Vector3(halfWidth, 0f, center.z - halfHeight + radius),
            GameConfigs.Instance.BorderLineStep);

        AddArc(points, brCenter, radius, -90f, 0f, arcSegments);

        AddLineSampled(points,
            new Vector3(halfWidth - radius, 0f, center.z - halfHeight),
            end,
            GameConfigs.Instance.BorderLineStep);


        //CREATE SPLİNE
        SplinePoint[] splinePoints = new SplinePoint[points.Count];
        for (int i = 0; i < points.Count; i++)
        {
            splinePoints[i] = new SplinePoint
            {
                position = points[i],
                normal = Vector3.up,
                size = GameConfigs.Instance.BorderThickness,
                color = Color.white
            };
        }

        spline.type = Spline.Type.Linear;
        spline.SetPoints(splinePoints);

        spline.RebuildImmediate();
        spline.GetComponent<SplineMesh>().RebuildImmediate();

        // CREATE HOLE OBJECT
        CreateHoleObject(Vector3.forward * (center.z - halfHeight));
    }

    private void AddLineSampled(List<Vector3> points, Vector3 from, Vector3 to, float step)
    {
        float dist = Vector3.Distance(from, to);
        int segments = Mathf.Max(1, Mathf.CeilToInt(dist / Mathf.Max(0.0001f, step)));

        for (int i = 1; i <= segments; i++)
        {
            float t = i / (float)segments;
            Vector3 p = Vector3.Lerp(from, to, t);
            points.Add(p);
        }
    }

    private static void AddArc(List<Vector3> points, Vector3 center, float radius, float startDeg, float endDeg, int segments)
    {
        segments = Mathf.Max(1, segments);

        float startRad = startDeg * Mathf.Deg2Rad;
        float endRad = endDeg * Mathf.Deg2Rad;

        for (int i = segments; i >= 1; i--)
        {
            float t = i / (float)segments;
            float a = Mathf.Lerp(startRad, endRad, t);

            float x = Mathf.Cos(a) * radius;
            float z = Mathf.Sin(a) * radius;

            Vector3 p = new Vector3(center.x + x, center.y, center.z + z);
            points.Add(p);
        }
    }
}