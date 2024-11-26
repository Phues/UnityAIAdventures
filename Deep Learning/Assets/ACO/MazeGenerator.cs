using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField] private MazeCell _mazeCellPrefab;
    [SerializeField] private int _mazeWidth;
    [SerializeField] private int _mazeDepth;

    private MazeCell[,] _mazeGrid;
    private bool _isMazeGenerationComplete = false; // Added boolean to track completion
    [SerializeField] private float _generationTime;

    public int Width => _mazeWidth;
    public int Depth => _mazeDepth;
    public bool IsMazeGenerationComplete => _isMazeGenerationComplete;
    public MazeCell[,] MazeGrid => _mazeGrid;


    public List<Vector2Int> GetNeighboringCells(Vector2Int position)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();
        int x = position.x;
        int z = position.y;

        if (x > 0) neighbors.Add(new Vector2Int(x - 1, z));
        if (x < _mazeWidth - 1) neighbors.Add(new Vector2Int(x + 1, z));
        if (z > 0) neighbors.Add(new Vector2Int(x, z - 1));
        if (z < _mazeDepth - 1) neighbors.Add(new Vector2Int(x, z + 1));

        return neighbors;
    }


    public Vector2Int GetExitPosition()
    {
        return new Vector2Int(_mazeWidth - 1, _mazeDepth - 1); // Exit is top-right corner.
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    IEnumerator Start()
    {
        _mazeGrid = new MazeCell[_mazeWidth, _mazeDepth];

        // Create the grid
        for (int x = 0; x < _mazeWidth; x++)
        {
            for (int z = 0; z < _mazeDepth; z++)
            {
                MazeCell mazeCell = Instantiate(_mazeCellPrefab, new Vector3(x, 0, z), Quaternion.identity);
                _mazeGrid[x, z] = mazeCell;
            }
        }

        // Generate the maze
        yield return GenerateMaze(null, _mazeGrid[0, 0]);

        // Set maze generation completion flag
        _isMazeGenerationComplete = true;
    }

    public bool HasParrallelWalls(MazeCell mazeCell)
    {
        // Vertical parallel walls (back and front active, left and right inactive)
        bool verticalParallel = mazeCell.IsBackWallActive() &&
                                 mazeCell.IsFrontWallActive() &&
                                 !mazeCell.IsLeftWallActive() &&
                                 !mazeCell.IsRightWallActive();

        // Horizontal parallel walls (left and right active, back and front inactive)
        bool horizontalParallel = mazeCell.IsLeftWallActive() &&
                                   mazeCell.IsRightWallActive() &&
                                   !mazeCell.IsBackWallActive() &&
                                   !mazeCell.IsFrontWallActive();

        return verticalParallel || horizontalParallel;
    }

    private IEnumerator GenerateMaze(MazeCell previousCell, MazeCell currentCell)
    {
        currentCell.Visit();
        ClearWalls(previousCell, currentCell);

        yield return new WaitForSeconds(_generationTime);

        MazeCell nextCell;
        do
        {
            nextCell = GetNextUnvisitedCell(currentCell);
            if (nextCell != null)
            {
                yield return GenerateMaze(currentCell, nextCell);
            }
        } while (nextCell != null);
    }

    private MazeCell GetNextUnvisitedCell(MazeCell currentCell)
    {
        var unvisitedCells = GetUnvisitedCells(currentCell);
        var randomIndex = Random.Range(0, unvisitedCells.Count());
        return unvisitedCells.OrderBy(_ => Random.Range(1, 10)).FirstOrDefault();
    }

    private IEnumerable<MazeCell> GetUnvisitedCells(MazeCell currentCell)
    {
        int x = (int)currentCell.transform.position.x;
        int z = (int)currentCell.transform.position.z;

        if (x + 1 < _mazeWidth)
        {
            var cellToRight = _mazeGrid[x + 1, z];
            if (!cellToRight.IsVisited)
            {
                yield return cellToRight;
            }
        }

        if (x - 1 >= 0)
        {
            var cellToLeft = _mazeGrid[x - 1, z];
            if (!cellToLeft.IsVisited)
            {
                yield return cellToLeft;
            }
        }

        if (z + 1 < _mazeDepth)
        {
            var cellToFront = _mazeGrid[x, z + 1];
            if (!cellToFront.IsVisited)
            {
                yield return cellToFront;
            }
        }

        if (z - 1 >= 0)
        {
            var cellToBack = _mazeGrid[x, z - 1];
            if (!cellToBack.IsVisited)
            {
                yield return cellToBack;
            }
        }
    }

    private void ClearWalls(MazeCell previousCell, MazeCell currentCell)
    {
        if (previousCell == null)
        {
            return;
        }

        if (previousCell.transform.position.x < currentCell.transform.position.x)
        {
            previousCell.ClearRightWall();
            currentCell.ClearLeftWall();
            return;
        }

        if (previousCell.transform.position.x > currentCell.transform.position.x)
        {
            previousCell.ClearLeftWall();
            currentCell.ClearRightWall();
            return;
        }

        if (previousCell.transform.position.z < currentCell.transform.position.z)
        {
            previousCell.ClearFrontWall();
            currentCell.ClearBackWall();
            return;
        }

        if (previousCell.transform.position.z > currentCell.transform.position.z)
        {
            previousCell.ClearBackWall();
            currentCell.ClearFrontWall();
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
