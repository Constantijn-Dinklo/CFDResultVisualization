using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Cubing : MonoBehaviour
{
    public int splitSize = 0;

    public int start_num_OP = 0;

    public int amount_OP = -1;
    // Start is called before the first frame update
    void Start()
    {
        DateTime before = DateTime.Now;
        SplitArea.splitArea(splitSize);
        PointsToArea.pointsToArea(splitSize, start_num_OP, amount_OP);
        DateTime after = DateTime.Now; 
        TimeSpan duration = after.Subtract(before);
        Debug.Log("Cubing duration in milliseconds: " + duration.TotalMilliseconds);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
