using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class LevelDataCreator : Singleton<LevelDataCreator>
{
    [Title("References")]
    [SerializeField] private Texture2D pixelArtTexture;


    public LevelData CreateLevelData()
    {
        Debug.Assert(pixelArtTexture != null, "Pixel art is null");

        if (pixelArtTexture == null)
            return null;

        PixelPieceData[,] pixelPieceDataGrid = PixelArtParseHelper.Parse(pixelArtTexture, out var colorCountDict);
        List<Color> levelColorsInOrder = colorCountDict.Keys.ToList();
        LevelData levelData = new LevelData(pixelPieceDataGrid, levelColorsInOrder, colorCountDict);

        return levelData;
    }
}

public class LevelData
{
    public PixelPieceData[,] PixelPieceDataGrid;
    public List<Color> LevelColorsInOrder;
    public Dictionary<Color, int> ColorCountDict;

    public LevelData(PixelPieceData[,] pixelPieceDataGrid, List<Color> levelColorsInOrder, Dictionary<Color, int> colorCountDict)
    {
        PixelPieceDataGrid = pixelPieceDataGrid;
        LevelColorsInOrder = levelColorsInOrder;
        ColorCountDict = colorCountDict;
    }
}


public class BigBallData
{
    public int colorID;
    public int capacity;
    public int connectedGroupId;

    public BigBallData(int colorID, int capacity, int connectedGroupId = -1)
    {
        this.colorID = colorID;
        this.capacity = capacity;
        this.connectedGroupId = connectedGroupId;
    }
}

public class QueueLaneData
{
    public List<BigBallData> balls = new();
}

public class PixelPieceData
{
    public Vector2Int coords;
    public Color color;
    public int colorID;

    public PixelPieceData(Vector2Int coords, Color color, int colorID)
    {
        this.coords = coords;
        this.color = color;
        this.colorID = colorID;
    }

    public override string ToString()
    {
        return $"{coords.x},{coords.y} | {colorID} |  {color}";
    }
}