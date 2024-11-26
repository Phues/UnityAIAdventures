using UnityEngine;

public class MazeCell : MonoBehaviour
{
    [SerializeField] private GameObject _leftWall;
    [SerializeField] private GameObject _rightWall;
    [SerializeField] private GameObject _frontWall;
    [SerializeField] private GameObject _backWall;
    [SerializeField] private GameObject _unvisitedBlock;

    public bool DeadEnd { get; set; }
    
    public bool IsVisited { get; private set; }
    
    public void Visit()
    {
        IsVisited = true;
        _unvisitedBlock.SetActive(false);
    }

    public bool IsLeftWallActive()
    {
        return _leftWall.activeSelf;
    }

    public bool IsRightWallActive()
    {
        return _rightWall.activeSelf;
    }

    public bool IsFrontWallActive()
    {
        return _frontWall.activeSelf;
    }

    public bool IsBackWallActive()
    {
        return _backWall.activeSelf;
    }
    
    public void ClearLeftWall()
    {
        _leftWall.SetActive(false);
    }
    
    public void ClearRightWall()
    {
        _rightWall.SetActive(false);
    }
    
    public void ClearFrontWall()
    {
        _frontWall.SetActive(false);
    }
    
    public void ClearBackWall()
    {
        _backWall.SetActive(false);
    }
    
    public void ActivateTarget()
    {
        var target = transform.Find("Target").gameObject;
        if (target != null)
        {
            target.SetActive(true);
        }
    }
    
}
