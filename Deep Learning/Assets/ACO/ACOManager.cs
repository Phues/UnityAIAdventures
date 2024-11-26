using UnityEngine;
using System.Collections.Generic;

public class ACOManager : MonoBehaviour
{

    [SerializeField] private MazeGenerator _mazeGenerator;
    [SerializeField] private AntAgent _antPrefab;
    [SerializeField] private Transform _colony;
    [SerializeField] private int _antCount;

    void InitializeAnts(int numAnts)
    {
        for (int i = 0; i < numAnts; i++)
        {
            // Spawn ants at random positions inside the colony
            Vector3 antPos = new Vector3(_colony.position.x, 0.5f, _colony.position.z);
            Instantiate(_antPrefab, antPos, Quaternion.identity, transform);
        }
    }

    void InitializeTargets()
    {
        for (int x = 0; x < _mazeGenerator.Width; x++)
        {
            for (int z = 0; z < _mazeGenerator.Depth; z++)
            {
                MazeCell currentCell = _mazeGenerator.MazeGrid[x, z];
                currentCell.ActivateTarget();
            }
        }


        //remove wall from the start cell
        _mazeGenerator.MazeGrid[_mazeGenerator.Width - 1, _mazeGenerator.Depth - 1].ClearRightWall();
        GameObject startCell = _mazeGenerator.MazeGrid[_mazeGenerator.Width - 1, _mazeGenerator.Depth - 1].gameObject;
        GameObject startTarget = startCell.transform.Find("Target").gameObject;
        startTarget.tag = "colony";

        //get the exit cell and make its target x2 bigger
        GameObject exitCell = _mazeGenerator.MazeGrid[0, 0].gameObject;
        GameObject exitTarget = exitCell.transform.Find("Target").gameObject;
        exitTarget.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        //change the tag to food
        exitTarget.tag = "food";
        //exitTarget.GetComponent<SphereCollider>().isTrigger = false;
    }

    public void InitializeACO()
    {
        InitializeTargets();
        InitializeAnts(_antCount);
    }
}
