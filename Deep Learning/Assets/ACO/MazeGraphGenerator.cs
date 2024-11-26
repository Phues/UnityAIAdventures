using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGraphGenerator : MonoBehaviour
{
    private MazeGenerator _mazeGenerator;
    private Dictionary<Vector2Int, List<Vector2Int>> _adjacencyGraph;
    private Dictionary<(Vector2Int, Vector2Int), float> _distanceMatrix;

    void Start()
    {
        _mazeGenerator = GetComponent<MazeGenerator>();
        StartCoroutine(WaitAndGenerateMazeGraph());
    }

    private IEnumerator WaitAndGenerateMazeGraph()
    {
        // Wait until maze generation is complete
        while (!_mazeGenerator.IsMazeGenerationComplete)
        {
            yield return null;
        }

        GenerateMazeGraph();
        CalculateDistanceMatrix();
        PrintDistanceMatrix();
    }

    public void GenerateMazeGraph()
    {
        _adjacencyGraph = new Dictionary<Vector2Int, List<Vector2Int>>();

        for (int x = 0; x < _mazeGenerator.Width; x++)
        {
            for (int z = 0; z < _mazeGenerator.Depth; z++)
            {
                Vector2Int currentPos = new Vector2Int(x, z);
                List<Vector2Int> connectedCells = FindConnectedCells(currentPos);
                _adjacencyGraph[currentPos] = connectedCells;
            }
        }
    }

    private List<Vector2Int> FindConnectedCells(Vector2Int position)
    {
        List<Vector2Int> connectedCells = new List<Vector2Int>();
        MazeCell currentCell = _mazeGenerator.MazeGrid[position.x, position.y];

        // Check if walls are clear in each direction to determine connections
        if (!currentCell.IsLeftWallActive() && position.x > 0)
            connectedCells.Add(new Vector2Int(position.x - 1, position.y));

        if (!currentCell.IsRightWallActive() && position.x < _mazeGenerator.Width - 1)
            connectedCells.Add(new Vector2Int(position.x + 1, position.y));

        if (!currentCell.IsBackWallActive() && position.y > 0)
            connectedCells.Add(new Vector2Int(position.x, position.y - 1));

        if (!currentCell.IsFrontWallActive() && position.y < _mazeGenerator.Depth - 1)
            connectedCells.Add(new Vector2Int(position.x, position.y + 1));

        return connectedCells;
    }

    public void CalculateDistanceMatrix()
    {
        _distanceMatrix = new Dictionary<(Vector2Int, Vector2Int), float>();

        foreach (var startCell in _adjacencyGraph.Keys)
        {
            var distances = DijkstraShortestPath(startCell);
            foreach (var endCell in _adjacencyGraph.Keys)
            {
                _distanceMatrix[(startCell, endCell)] = distances[endCell];
            }
        }
    }

    private Dictionary<Vector2Int, float> DijkstraShortestPath(Vector2Int start)
    {
        var distances = new Dictionary<Vector2Int, float>();
        var unvisited = new HashSet<Vector2Int>();

        foreach (var cell in _adjacencyGraph.Keys)
        {
            distances[cell] = float.MaxValue;
            unvisited.Add(cell);
        }
        distances[start] = 0;

        while (unvisited.Count > 0)
        {
            var current = GetClosestUnvisitedCell(unvisited, distances);
            unvisited.Remove(current);

            foreach (var neighbor in _adjacencyGraph[current])
            {
                if (unvisited.Contains(neighbor))
                {
                    float alternatePath = distances[current] + 1;
                    if (alternatePath < distances[neighbor])
                    {
                        distances[neighbor] = alternatePath;
                    }
                }
            }
        }

        return distances;
    }

    private Vector2Int GetClosestUnvisitedCell(HashSet<Vector2Int> unvisited, Dictionary<Vector2Int, float> distances)
    {
        Vector2Int closest = default;
        float minDistance = float.MaxValue;

        foreach (var cell in unvisited)
        {
            if (distances[cell] < minDistance)
            {
                minDistance = distances[cell];
                closest = cell;
            }
        }

        return closest;
    }

    // Debugging methods
    public void PrintAdjacencyGraph()
    {
        foreach (var kvp in _adjacencyGraph)
        {
            Debug.Log($"Cell {kvp.Key}: Connected to {string.Join(", ", kvp.Value)}");
        }
    }

    public void PrintDistanceMatrix()
    {
        if (_distanceMatrix == null)
        {
            Debug.LogWarning("Distance matrix has not been calculated yet.");
            return;
        }

        // Determine matrix dimensions
        int width = _mazeGenerator.Width;
        int depth = _mazeGenerator.Depth;

        // Create a 2D float array to hold the matrix
        float[,] matrix = new float[width, depth];

        // Populate the matrix
        for (int x1 = 0; x1 < width; x1++)
        {
            for (int z1 = 0; z1 < depth; z1++)
            {
                Vector2Int start = new Vector2Int(x1, z1);

                for (int x2 = 0; x2 < width; x2++)
                {
                    for (int z2 = 0; z2 < depth; z2++)
                    {
                        Vector2Int end = new Vector2Int(x2, z2);
                        matrix[x2, z2] = _distanceMatrix.ContainsKey((start, end))
                            ? _distanceMatrix[(start, end)]
                            : float.PositiveInfinity;
                    }
                }
            }
        }

        // Print the matrix
        string matrixString = "Distance Matrix:\n";
        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                // Format each cell to 2 decimal places, right-aligned
                matrixString += string.Format("{0,6:F2} ", matrix[x, z]);
            }
            matrixString += "\n";
        }

        Debug.Log(matrixString);
    }
}