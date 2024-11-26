using UnityEngine;

public class Target : MonoBehaviour
{
    private float _pheromoneStrength = 1.0f; // strength of pheromones to deposit
    private float _pheromoneDecay = 0.1f; // rate of decay for pheromones
    private float _pheromoneLevel = 0.0f; // current level of pheromones
    public bool DeadEnd {get; set; }


    void Start()
    {

        DeadEnd = false;
        InvokeRepeating("PheromoneDecay", 0.5f, 0.5f); // Call PheromoneDecay every 0.5 seconds
    }

    // function to deposit pheromones on the target
    public void DepositPheromones(float multiplier)
    {
        _pheromoneLevel += _pheromoneStrength * multiplier;
    }

    public void FoundFoodSource()
    {
        _pheromoneLevel = 100;
    }

    // function to get the current pheromone level
    public float GetPheromoneLevel()
    {
        return _pheromoneLevel;
    }

    // function to update the pheromone level based on decay rate
    private void PheromoneDecay()
    {
        if (_pheromoneLevel > 0)
        {
            _pheromoneLevel = _pheromoneLevel - _pheromoneDecay;
        } else
        {
            _pheromoneLevel= 0;
        }

    }
}
