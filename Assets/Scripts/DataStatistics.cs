using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DataStatistics
{

    public static Dictionary<string, float> minValue = new Dictionary<string, float>();

    public static Dictionary<string, float> maxValue = new Dictionary<string, float>();

    public static Dictionary<string, float> range = new Dictionary<string, float>();

    public static bool dataChanged = false;


    public static void setMinValue(string key, float newMin)
    {
        //minKValue = newMin;
        minValue[key] = newMin;
        range[key] = DataStatistics.getMaxValue(key) - DataStatistics.getMinValue(key);
        dataChanged = true;
    }

    public static void setMaxValue(string key, float newMax)
    {
        maxValue[key] = newMax;
        range[key] = DataStatistics.getMaxValue(key) - DataStatistics.getMinValue(key);
        dataChanged = true;
    }

    public static float getMinValue(string key)
    {
        if(!minValue.ContainsKey(key)){
            return float.MaxValue;
        }
        return minValue[key];
    }

    public static float getMaxValue(string key)
    {
        if(!maxValue.ContainsKey(key)){
            return float.MinValue;
        }
        return maxValue[key];
    }

    public static float getRange(string key)
    {
        if(!range.ContainsKey(key)){
            return 0;
        }
        return range[key];
    }
}
