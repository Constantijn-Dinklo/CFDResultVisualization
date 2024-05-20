using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PreProcessing : MonoBehaviour
{
    public int splitSize = 0;

    public int start_num_OP = 0;
    public int start_num_NP = 0;

    public int amount_OP = -1;
    public int amount_NP = -1;

    public int[] depths = {};
    // Start is called before the first frame update
    void Start()
    {
        DateTime before = DateTime.Now;
        SplitArea.splitArea(splitSize);
        PointsToArea.pointsToArea(splitSize, start_num_OP, amount_OP);
        // WriteVoxelValues.writeVoxelValues(splitSize, start_num_NP, amount_NP, depths);
        DateTime after = DateTime.Now; 
        TimeSpan duration = after.Subtract(before);
        Debug.Log("Duration in milliseconds: " + duration.TotalMilliseconds);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
