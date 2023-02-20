using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HexaGrid : MonoBehaviour
{
    const int MAP_WIDTH = 200;
    const int MAP_HEIGHT = 200;
    const float SPACING_WIDTH = 2f;
    const float SPACING_HEIGHT = 1.75f;

    [SerializeField]
    GameObject _hextilePrefab;
    [SerializeField]
    Transform _hexInstancesParent;

#if UNITY_EDITOR
    public void SetHextilePrefab(GameObject hextilePrefab)
    {
        _hextilePrefab = hextilePrefab;
    }
    public void SetInstancesParent(Transform instancesParent)
    {
        _hexInstancesParent = instancesParent;
    }
#endif

    /// <summary>
    /// Store all the hextile prefab instances renderer
    /// </summary>
    Renderer[][] _hexatilesInstances;

    /// <summary>
    /// Generate a new grid of hextiles. 
    /// Instantiate and place every hextiles.
    /// This method was based on this video : https://www.youtube.com/watch?v=EPaSmQ2vtek (Only the 'Creating Layout' section)
    /// </summary>
    /// <remarks>This method is very CPU (GetComponent) and RAM (Instanciation) intensive. Use it carefully (can take 1 full second to execute).</remarks>
    public void Generate()
    {
        _hexatilesInstances = new Renderer[MAP_WIDTH][];
        Quaternion quaternionRotation = Quaternion.Euler(-90, 0, 0);

        //The absoluteWidthHalf and absoluteheightHalf are used to shift negatively the spawn location of the instances so that the 0,0 (world position) is in the middle of the hex grid and not at the end
        float absoluteWidthHalf = MAP_WIDTH * SPACING_WIDTH / 2;
        float absoluteheightHalf = MAP_HEIGHT * SPACING_HEIGHT / 2;
        for (int x = 0; x < MAP_WIDTH; x++)
        {
            _hexatilesInstances[x] = new Renderer[MAP_HEIGHT];
            for (int y = 0; y < MAP_HEIGHT; y++)
            {
                GameObject hexTile;

                //The generation is the same as for generating squares except that one line out of two is shifted on the X axis (left if odd)
                if (y % 2 == 0)
                    hexTile = Instantiate(_hextilePrefab, new Vector3(x * SPACING_WIDTH - absoluteWidthHalf, y * SPACING_HEIGHT - absoluteheightHalf, 0), quaternionRotation);
                else
                    hexTile = Instantiate(_hextilePrefab, new Vector3((x * SPACING_WIDTH) - (SPACING_WIDTH / 2) - absoluteWidthHalf, y * SPACING_HEIGHT - absoluteheightHalf, 0), quaternionRotation);
                hexTile.transform.SetParent(_hexInstancesParent);
                _hexatilesInstances[x][y] = hexTile.GetComponent<Renderer>();
            }
        }
    }

    /// <summary>
    /// Set the hextiles to the default state
    /// </summary>
    public void Clear()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Convert a world spaced position to the nearest hex index
    /// </summary>
    /// <param name="worldPosition">The transform position</param>
    /// <returns>The nearest hextile index</returns>
    public Vector2Int WordPositionToHexIndexes(Vector2 worldPosition)
    {
        int y = Mathf.RoundToInt((worldPosition.y / SPACING_HEIGHT) + Mathf.Round(MAP_HEIGHT / 2));
        if (y % 2 == 1) worldPosition.x = worldPosition.x + SPACING_WIDTH / 2;
        int x = Mathf.RoundToInt((worldPosition.x / SPACING_WIDTH) + Mathf.Round(MAP_WIDTH / 2));
        return new Vector2Int(x, y);
    }

    /// <summary>
    /// Get the center of the hexindex on the map (world position)
    /// </summary>
    /// <param name="indexes">The index of the hextile</param>
    /// <returns>The center world position of the hextile</returns>
    public Vector2 HexIndexesToWorldPosition(Vector2Int indexes)
    {
        Vector2 worldPosition = new Vector2((indexes.x - Mathf.Round(MAP_WIDTH / 2)) * SPACING_WIDTH, (indexes.y - Mathf.Round(MAP_HEIGHT / 2)) * SPACING_HEIGHT);
        if (indexes.y % 2 == 1) worldPosition.x = worldPosition.x - SPACING_WIDTH / 2;
        return worldPosition;
    }

    /// <summary>
    /// Get the list of all the hexagons indexes inside a large hexagon
    /// </summary>
    /// <param name="center">The center of the bug hexagon</param>
    /// <param name="radius">The radius of the hexagon</param>
    /// <param name="outline">Get only the outile ? (Or also all the positions that fill it)</param>
    /// <returns>The list of all hextiles positions</returns>
    public List<Vector2Int> GetBigHexagonPositions(Vector2Int center, int radius, bool outline)
    {
        List<Vector2Int> allPositions = new List<Vector2Int>();

        //This is necessary to shift the upper and lower parts (by one to the left) if you start with an even line (shifted to the right).
        int xDecal = (center.y + 1) % 2;

        //Loop through the Y axis lines by lines from -radius (-1) to +radius (-1)
        for (int y = -(radius - 1); y < radius; y++)
        {
            int yAbsolute = FasterAbs(y);

            //Used to determine the size of the line to use (large if in the center, small if at the beginning or end)
            int xLenth = ((radius * 2) - 1) - yAbsolute;
            int xDrawStart = (int)FasterAbs(Mathf.Floor(y / 2));

            //Increment by one (to draw a full line) if the outline is false OR if the y is starting or ending value.
            //Else, increment to go directly to last point to draw (if outline ant the y is not the start or end value)
            int incrementor = ((!outline) || FasterAbs(y) == (radius - 1)) ? 1 : xLenth - 1;
            for (int x = 0; x < xLenth; x = x + incrementor)
            {
                allPositions.Add(new Vector2Int(center.x - (radius - 1) + x + xDrawStart + ((yAbsolute % 2 == 1) ? xDecal : 0), center.y + y));
            }
        }
        return allPositions;
    }

    /// <summary>
    /// Return the absolute of a number
    /// </summary>
    /// <param name="x">The number</param>
    /// <returns>The absolute of the number</returns>
    /// <remarks>For some reasons, this is faster than the default Math.Abs method</remarks>
    float FasterAbs(float x)
    {
        return x < 0 ? x * -1 : x;
    }

    /// <summary>
    /// Return the absolute of a number
    /// </summary>
    /// <param name="x">The number</param>
    /// <returns>The absolute of the number</returns>
    /// <remarks>For some reasons, this is faster than the default Math.Abs method</remarks>
    int FasterAbs(int x)
    {
        return x < 0 ? x * -1 : x;
    }
}