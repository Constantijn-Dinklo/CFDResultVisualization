using System;
using System.Collections.Generic;
using UnityEngine;

public static class DataStatisticsVector
{
    
    public static Dictionary<string, float> minXValue = new Dictionary<string, float>();
    public static Dictionary<string, float> minYValue = new Dictionary<string, float>();
    public static Dictionary<string, float> minZValue = new Dictionary<string, float>();

    public static Dictionary<string, float> maxXValue = new Dictionary<string, float>();
    public static Dictionary<string, float> maxYValue = new Dictionary<string, float>();
    public static Dictionary<string, float> maxZValue = new Dictionary<string, float>();

    public static Dictionary<string, float> rangeX = new Dictionary<string, float>();
    public static Dictionary<string, float> rangeY = new Dictionary<string, float>();
    public static Dictionary<string, float> rangeZ = new Dictionary<string, float>();

    public static Dictionary<string, float> minMag = new Dictionary<string, float>();
    public static Dictionary<string, float> maxMag = new Dictionary<string, float>();

    public static Dictionary<string, float> magRange = new Dictionary<string, float>();

    public static void updateValue(string key, Vector3 value){
        // X
        if(value.x < getMinXValue(key))
        {
            setMinXValue(key, value.x);
        }
        if(value.x > getMaxXValue(key))
        {
            setMaxXValue(key, value.x);
        }

        // Y
        if(value.y < getMinYValue(key))
        {
            setMinYValue(key, value.y);
        }
        if(value.y > getMaxYValue(key))
        {
            setMaxYValue(key, value.y);
        }

        // Z
        if(value.z < getMinZValue(key))
        {
            setMinZValue(key, value.z);
        }
        if(value.z > getMaxZValue(key))
        {
            setMaxZValue(key, value.z);
        }

        float mag = value.magnitude;
        if(mag < getMinMag(key)){
            setMinMag(key, mag);
        }
        if(mag > getMaxMag(key)){
            setMaxMag(key, mag);
        }
    }

    public static void setMinXValue(string key, float newMin)
    {
        //minKValue = newMin;
        minXValue[key] = newMin;
        rangeX[key] = DataStatisticsVector.getMaxXValue(key) - DataStatisticsVector.getMinXValue(key);
    }
    public static void setMinYValue(string key, float newMin)
    {
        //minKValue = newMin;
        minYValue[key] = newMin;
        rangeY[key] = DataStatisticsVector.getMaxYValue(key) - DataStatisticsVector.getMinYValue(key);
    }
    public static void setMinZValue(string key, float newMin)
    {
        //minKValue = newMin;
        minZValue[key] = newMin;
        rangeZ[key] = DataStatisticsVector.getMaxZValue(key) - DataStatisticsVector.getMinZValue(key);
    }
    
    public static void setMaxXValue(string key, float newMax)
    {
        maxXValue[key] = newMax;
        rangeX[key] = DataStatisticsVector.getMaxXValue(key) - DataStatisticsVector.getMinXValue(key);
    }
    public static void setMaxYValue(string key, float newMax)
    {
        maxYValue[key] = newMax;
        rangeY[key] = DataStatisticsVector.getMaxYValue(key) - DataStatisticsVector.getMinYValue(key);
    }
    public static void setMaxZValue(string key, float newMax)
    {
        maxZValue[key] = newMax;
        rangeZ[key] = DataStatisticsVector.getMaxZValue(key) - DataStatisticsVector.getMinZValue(key);
    }

    public static float getMinXValue(string key)
    {
        if(!minXValue.ContainsKey(key)){
            return float.MaxValue;
        }
        return minXValue[key];
    }
    public static float getMinYValue(string key)
    {
        if(!minYValue.ContainsKey(key)){
            return float.MaxValue;
        }
        return minYValue[key];
    }
    public static float getMinZValue(string key)
    {
        if(!minZValue.ContainsKey(key)){
            return float.MaxValue;
        }
        return minZValue[key];
    }

    public static float getMaxXValue(string key)
    {
        if(!maxXValue.ContainsKey(key)){
            return float.MinValue;
        }
        return maxXValue[key];
    }
    public static float getMaxYValue(string key)
    {
        if(!maxYValue.ContainsKey(key)){
            return float.MinValue;
        }
        return maxYValue[key];
    }
    public static float getMaxZValue(string key)
    {
        if(!maxZValue.ContainsKey(key)){
            return float.MinValue;
        }
        return maxZValue[key];
    }

    public static float getXRange(string key)
    {
        if(!rangeX.ContainsKey(key)){
            return 0;
        }
        return rangeX[key];
    }

    public static float getYRange(string key)
    {
        if(!rangeY.ContainsKey(key)){
            return 0;
        }
        return rangeY[key];
    }

    public static float getZRange(string key)
    {
        if(!rangeZ.ContainsKey(key)){
            return 0;
        }
        return rangeZ[key];
    }

    public static void setMinMag(string key, float newMag)
    {
        minMag[key] = newMag;
        magRange[key] = DataStatisticsVector.getMaxMag(key) - DataStatisticsVector.getMinMag(key);
    }

    public static void setMaxMag(string key, float newMag)
    {
        maxMag[key] = newMag;
        magRange[key] = DataStatisticsVector.getMaxMag(key) - DataStatisticsVector.getMinMag(key);
    }

    public static float getMinMag(string key)
    {
        if(!minMag.ContainsKey(key)){
            return float.MaxValue;
        }
        return minMag[key];
    }

    public static float getMaxMag(string key)
    {
        if(!maxMag.ContainsKey(key)){
            return float.MinValue;
        }
        return maxMag[key];
    }

    public static float getMagRange(string key)
    {
        if(!magRange.ContainsKey(key)){
            return 1;
        }
        return magRange[key];
    }

}