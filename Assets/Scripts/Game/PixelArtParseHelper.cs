using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class PixelArtParseHelper
{
    public static PixelPieceData[,] Parse(Texture2D tex, out Dictionary<Color, int> colorCountDict)
    {
        int width = tex.width;
        int height = tex.height;

        PixelPieceData[,] pixelPieceGrid = new PixelPieceData[width, height];
        Color32[] pixels = tex.GetPixels32();

        colorCountDict = new Dictionary<Color, int>();
        Dictionary<Color, int> colorIdDict = new Dictionary<Color, int>();

        int nextColorId = 0;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color color = (Color)pixels[x + y * width];

                if (colorCountDict.ContainsKey(color))
                    colorCountDict[color]++;
                else
                    colorCountDict.Add(color, 1);

                if (!colorIdDict.TryGetValue(color, out var colorID))
                {
                    colorID = nextColorId;
                    colorIdDict.Add(color, colorID);
                    nextColorId++;
                }

                pixelPieceGrid[x, y] = new PixelPieceData(new Vector2Int(x, y), color, colorID);
            }
        }

        return pixelPieceGrid;
    }
}