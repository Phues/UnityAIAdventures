using System;
using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{
    private Vector2 _direction = Vector2.right;
    private Vector2 _inputDirection = Vector2.right; // Store the input separately
    private List<Transform> _segments;
    public Transform segmentPrefab;

    private void Start()
    {
        _segments = new List<Transform>();
        _segments.Add(this.transform);
    }

    private void Update()
    {
        // Store the input but don't change direction immediately
        if (Input.GetKeyDown(KeyCode.W) && _direction != Vector2.down)
        {
            _inputDirection = Vector2.up;
        }
        else if (Input.GetKeyDown(KeyCode.S) && _direction != Vector2.up)
        {
            _inputDirection = Vector2.down;
        }
        else if (Input.GetKeyDown(KeyCode.A) && _direction != Vector2.right)
        {
            _inputDirection = Vector2.left;
        }
        else if (Input.GetKeyDown(KeyCode.D) && _direction != Vector2.left)
        {
            _inputDirection = Vector2.right;
        }
    }
    
    private void FixedUpdate()
    {
        // Apply the stored input direction
        _direction = _inputDirection;
        
        for (int i = _segments.Count - 1; i > 0; i--)
        {
            _segments[i].position = _segments[i - 1].position;
        }
        
        this.transform.position = new Vector3(
            Mathf.Round(this.transform.position.x) + _direction.x,
            Mathf.Round(this.transform.position.y) + _direction.y,
            0
        );
    }

    private void Grow()
    {
        Transform segment = Instantiate(this.segmentPrefab);
        segment.position = _segments[^1].position;
        _segments.Add(segment);
    }

    private void ResetState()
    {
        for (int i = 1; i < _segments.Count; i++)
        {
            Destroy(_segments[i].gameObject);
        }
        
        _segments.Clear();
        _segments.Add(this.transform);
        
        this.transform.position = Vector3.zero;
        _direction = Vector2.right;
        _inputDirection = Vector2.right;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Food")
        {
            Grow();
        }
        else if(col.tag == "Obstacle")
        {
            ResetState();
            Debug.Log("Game Over");
        }
    }
}