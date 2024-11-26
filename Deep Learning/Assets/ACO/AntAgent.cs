using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

public class AntAgent : MonoBehaviour
{
    [SerializeField] private float _speed = 0.5f;
    private ACOManager _acoManager;

    private List<GameObject> _targets = new List<GameObject>();
    private List<GameObject> _visitedTargets = new List<GameObject>();
    private List<GameObject> _returnPath = new List<GameObject>();

    private GameObject _currentTarget = null;
    private GameObject _lastTarget = null;
    private bool _hasFood;

    void Start()
    {
        _acoManager = GameObject.FindFirstObjectByType<ACOManager>();
        InitializeAgent();
    }

    void InitializeAgent()
    {
        _hasFood = false;
        //randomly decrease or increase the speed of the ant
        float speedOffset = Random.Range(-0.2f, 0.2f);
        _speed += speedOffset;

        // Get all targets
        /*_targets.AddRange(GameObject.FindGameObjectsWithTag("target"));
        _targets.AddRange(GameObject.FindGameObjectsWithTag("colony"));
        _targets.AddRange(GameObject.FindGameObjectsWithTag("food"));*/

        // Get all targets
        GameObject[] allTargets = GameObject.FindGameObjectsWithTag("target");
        GameObject[] colony = GameObject.FindGameObjectsWithTag("colony");
        GameObject[] food = GameObject.FindGameObjectsWithTag("food");

        foreach (GameObject target in allTargets)
        {
            _targets.Add(target);
        }
        foreach (GameObject target in colony)
        {
            _targets.Add(target);
        }
        foreach (GameObject target in food)
        {
            _targets.Add(target);
        }

        // Find the closest target to the ant's starting position
        float minDistance = float.MaxValue;
        foreach (GameObject target in _targets)
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                _currentTarget = target;
            }
        }
        _returnPath.Add(_currentTarget);
        FindNewTarget();
    }

    void Update()
    {

        if (_currentTarget != null)
        {
            transform.LookAt(_currentTarget.transform.position);
            transform.Translate(Vector3.forward * _speed * Time.deltaTime);

            // Check if the ant has reached the current target
            if (Vector3.Distance(transform.position, _currentTarget.transform.position) < 0.2f)
            {
                VisitTarget();
                FindNewTarget();
            }
        }
        else
        {
            FindNewTarget();
        }
    }

    void FindNewTarget()
    {
        _lastTarget = _currentTarget;
        if (_hasFood)
        {
            FindReturnTarget();
            _currentTarget.GetComponent<Renderer>().material.color = Color.yellow;
            _lastTarget.GetComponent<Renderer>().material.color = Color.white;
        }
        else
        {
            // Find a new target
            List<GameObject> visibleTargets = new List<GameObject>();
            float totalPheromones = 0f;
            foreach (GameObject target in _targets)
            {
                if (_visitedTargets.Contains(target) || target.GetComponent<Target>().DeadEnd)
                {
                    continue; // Skip targets that have already been visited
                }
                float distance = Vector3.Distance(transform.position, target.transform.position);
                if (distance <= 1.30f && CanSeeTarget(target))
                {
                    float pheromoneLevel = target.GetComponent<Target>().GetPheromoneLevel();
                    pheromoneLevel = Mathf.Clamp(pheromoneLevel, 0f, 1.0f);
                    pheromoneLevel = 1 - pheromoneLevel;
                    visibleTargets.Add(target);
                    totalPheromones += pheromoneLevel;
                }
            }

            if (visibleTargets.Count > 0)
            {
                // Select target based on pheromone levels
                float randomValue = Random.Range(0f, totalPheromones);
                float cumulativePheromones = 0f;

                foreach (GameObject target in visibleTargets)
                {
                        float pheromoneLevel = target.gameObject.GetComponent<Target>().GetPheromoneLevel();
                        pheromoneLevel = Mathf.Clamp(pheromoneLevel, 0f, 1.0f);
                        pheromoneLevel = 1 - pheromoneLevel;
                        cumulativePheromones += pheromoneLevel;
                        if (cumulativePheromones >= randomValue)
                        {
                            _currentTarget = target;
                            break;
                        }
                }
            }
            else if (_lastTarget != null)
            {
                if (_currentTarget.CompareTag("target"))
                {
                    _currentTarget.GetComponent<Target>().DeadEnd = true;
                }
                if (_lastTarget.GetComponent<Target>().DeadEnd == false)
                {
                    _currentTarget = _lastTarget;
                }
                else
                {
                    foreach (GameObject target in _targets)
                    {
                        float distance = Vector3.Distance(transform.position, target.transform.position);
                        if (distance <= 1.30f && CanSeeTarget(target))
                        {
                            visibleTargets.Add(target);
                        }
                    }
                    int randomTarget = Random.Range(0, visibleTargets.Count);
                    _currentTarget = visibleTargets[randomTarget];
                }
                _visitedTargets.Clear();
            }
        }
        _visitedTargets.Add(_currentTarget);
    }

    void FindReturnTarget()
    {
        if (_returnPath.Last().GetComponent<Target>().DeadEnd == false)
        {
            _currentTarget = _returnPath.Last();
            _returnPath.RemoveAt(_returnPath.Count - 1);
        }
        else
        {
           // Debug.Log("Found dead end, returning to previous target");
            _returnPath.RemoveAt(_returnPath.Count - 1);
            FindReturnTarget();
        }
        //Debug.Log("Returning path count: " + _returnPath.Count);
    }

    bool CanSeeTarget(GameObject target)
    {
        Vector3 targetDirection = target.transform.position - _currentTarget.transform.position;
        Debug.DrawRay(_currentTarget.transform.position, targetDirection, Color.green);
        if (Physics.Raycast(_currentTarget.transform.position, targetDirection, out RaycastHit hit))
        {
            if (hit.collider.gameObject != target)
            {
                return false;
            }
        }
        return true;
    }

    void VisitTarget()
    {
        if (_hasFood == false)
        {
            _returnPath.Add(_currentTarget);
        }

        Target target = _currentTarget.GetComponent<Target>();
            if (_currentTarget.CompareTag("food"))
            {
                Debug.Log("Ant"+ gameObject.GetInstanceID() + " found food");
                _hasFood = true;
                target.FoundFoodSource();
                _visitedTargets.Clear();
            }
            else if (_currentTarget.CompareTag("colony") && _hasFood)
            {
                Debug.Log("Food deposited");
                _hasFood = false;
                _visitedTargets.Clear();
            }

            // Deposit pheromones with higher strength when carrying food
            target.DepositPheromones(_hasFood ? 10.0f : 1.0f);
    }
}