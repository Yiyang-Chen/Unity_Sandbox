using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleMetrics : MonoBehaviour
{
    [SerializeField] private string levelName;

    public void PuzzleSolved()
    {
        if (MetricManagerScript.instance != null)
        {
            MetricManagerScript.instance.LogString("Puzzle Solved", levelName);
        }
    }
}